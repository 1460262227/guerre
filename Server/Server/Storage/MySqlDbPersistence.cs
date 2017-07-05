/*
 * creator(s): chenm
 * reviser(s): chenm
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Swift;
using System.Data;
using System.Data.Common;
using System.Data.Sql;
using System.Text;
using System.Data.SqlClient;
using System.Threading;
using MySql;
using MySql.Data;
using MySql.Data.Types;
using MySql.Data.MySqlClient;

namespace Server
{
    /// <summary>
    /// MySqlDb 序列化器基类
    /// </summary>
    public class MySqlDbPersistence<T, IDType> : Component, ISqlPersistence<T, IDType>, IFrameDrived where T : DataItem<IDType>, new()
    {
        // 构造器，需要指明数据映射工具
        public MySqlDbPersistence(string dbName, string dbServer, string username, string password, string tableName, string createTableCommander,
            string[] additionalCols, Func<T, string, object> col2ValueMap)
        {
            tbName = tableName;
            connStr = string.Format(@"Server={0};Database={1};User Id={2};Password={3};charset=utf8;pooling=true",
                dbServer, dbName, username, password);
            cvm = col2ValueMap;
            cols = additionalCols;

            if (!TableExists(tbName))
            {
                using (var conn = OpenConnection())
                {
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = createTableCommander;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #region 应用接口

        // 做同步 int 查询
        public int QueryInt(string sql)
        {
            using (var conn = OpenConnection())
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                var waiter = cmd.BeginExecuteReader();
                while (!waiter.IsCompleted)
                    ; // just waiting

                using (var r = cmd.EndExecuteReader(waiter))
                    return r.GetInt32(0);
            }
        }

        // 执行裸的非查询 sql 语句
        public void ExecuteNonQuery(string sql)
        {
            opQueue.Enqueue(ExecuteNonQueryImpl(sql));
        }

        // 执行裸的查询 sql 语句
        public void ExecuteRawQuery(string sql, Action<DbDataReader> cb)
        {
            opQueue.Enqueue(ExecuteQueryImpl(sql, cb));
        }

        // 添加数据
        public void Add(T d)
        {
            WriteBuffer buff = new WriteBuffer();
            d.Serialize(buff);
            opQueue.Enqueue(AddImpl(d.ID, d, buff.Data));
        }

        // 更新数据
        public void Update(T d)
        {
            WriteBuffer buff = new WriteBuffer();
            d.Serialize(buff);
            opQueue.Enqueue(UpdateImpl(d.ID, d, buff.Data));
        }

        // 删除数据
        public void Delete(IDType id)
        {
            opQueue.Enqueue(DeleteImpl(id));
        }

        // 加载数据
        public void Load(IDType id, Action<T> cb)
        {
            opQueue.Enqueue(LoadImpl(id, (data) =>
            {
                if (data != null)
                {
                    T d = new T();
                    d.Deserialize(new RingBuffer(data));
                    cb(d);
                }
                else
                    cb(null);
            }));
        }

        // 加载数据
        public void LoadAll(string whereClause, Action<T[]> cb)
        {
            opQueue.Enqueue(LoadAllImpl(whereClause, (dataLst) =>
            {
                var lst = new List<T>();
                foreach (var data in dataLst)
                {
                    T d = new T();
                    d.Deserialize(new RingBuffer(data));
                    lst.Add(d);
                }

                cb(lst.ToArray());
            }));
        }

        // 加载数据
        public void LoadAllID(string whereClause, Action<IDType[]> cb)
        {
            opQueue.Enqueue(LoadAllIDImpl(whereClause, cb));
        }

        // 关闭序列化器并处理完所有待处理数据
        public override void Close()
        {
            while (opQueue.Count > 0 || curOp != null)
                NextStep();
        }

        #endregion

        ConcurrentQueue<IEnumerator> opQueue = new ConcurrentQueue<IEnumerator>();

        IEnumerator curOp = null;
        public void OnTimeElapsed(int te)
        {
            NextStep();
        }

        void NextStep()
        {
            if (curOp != null)
            {
                if (!curOp.MoveNext())
                    curOp = null;
            }

            if (curOp == null && opQueue.Count > 0)
                opQueue.TryDequeue(out curOp);
        }

        // 执行裸的非查询 sql 语句
        IEnumerator ExecuteNonQueryImpl(string sql)
        {
            yield return null;

            using (var conn = OpenConnection())
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                var waiter = cmd.BeginExecuteNonQuery();
                while (!waiter.IsCompleted)
                    yield return null;

                cmd.EndExecuteNonQuery(waiter);
            }
        }

        // 执行裸的查询 sql 语句
        IEnumerator ExecuteQueryImpl(string sql, Action<DbDataReader> cb)
        {
            yield return null;

            using (var conn = OpenConnection())
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                var waiter = cmd.BeginExecuteReader();
                while (!waiter.IsCompleted)
                    yield return null;

                using (var r = cmd.EndExecuteReader(waiter))
                    cb(r);
            }
        }

        // 将数据落地保存
        IEnumerator AddImpl(IDType id, T d, byte[] buff)
        {
            yield return null;

            string addCols = "";
            string addNamedPs = "";

            if (cols != null && cols.Length > 0)
            {
                foreach (string c in cols)
                {
                    object v = cvm(d, c);
                    if (v != null)
                    {
                        addCols += ", " + c;
                        addNamedPs += ", " + NamedParam(c);
                    }
                }
            }

            using (var conn = OpenConnection())
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(@"insert into {0} (ID, Data{1}) values ({2}, {3}{4})", tbName, addCols, NamedParam("ID"), NamedParam("Data"), addNamedPs);

                IDbDataParameter idParam = cmd.CreateParameter();
                idParam.ParameterName = RealParam("ID");
                idParam.DbType = GetDbType(id, "ID");
                idParam.Value = id;
                cmd.Parameters.Add(idParam);

                IDbDataParameter dataParam = cmd.CreateParameter();
                dataParam.ParameterName = RealParam("Data");
                dataParam.DbType = DbType.Binary;
                dataParam.Value = buff;
                cmd.Parameters.Add(dataParam);

                if (cols != null && cols.Length > 0)
                {
                    foreach (string c in cols)
                    {
                        object v = cvm(d, c);
                        if (v == null)
                            continue;

                        IDbDataParameter p = cmd.CreateParameter();
                        p.ParameterName = RealParam(c);
                        p.DbType = GetDbType(v, c);
                        p.Value = v;
                        cmd.Parameters.Add(p);
                    }
                }

                var waiter = cmd.BeginExecuteNonQuery();
                while (!waiter.IsCompleted)
                    yield return null;

                cmd.EndExecuteNonQuery(waiter);
            }
        }

        // 将缓冲同步落地保存
        IEnumerator UpdateImpl(IDType id, T d, byte[] buff)
        {
            yield return null;

            string addCols = "";
            if (cols != null && cols.Length > 0)
            {
                foreach (string c in cols)
                {
                    object v = cvm(d, c);
                    if (v != null)
                        addCols += ", " + c + "=" + NamedParam(c);
                }
            }

            using (var conn = OpenConnection())
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(@"update {0} set Data = {1}{2} where ID = {3}", tbName, NamedParam("Data"), addCols, NamedParam("ID"));

                IDbDataParameter idParam = cmd.CreateParameter();
                idParam.ParameterName = RealParam("ID");
                idParam.DbType = GetDbType(id, "ID");
                idParam.Value = id;
                cmd.Parameters.Add(idParam);

                IDbDataParameter dataParam = cmd.CreateParameter();
                dataParam.ParameterName = RealParam("Data");
                dataParam.DbType = DbType.Binary;
                dataParam.Value = buff;
                cmd.Parameters.Add(dataParam);

                if (cols != null && cols.Length > 0)
                {
                    foreach (string c in cols)
                    {
                        object v = cvm(d, c);
                        if (v == null)
                            continue;

                        IDbDataParameter p = cmd.CreateParameter();
                        p.ParameterName = RealParam(c);
                        p.DbType = GetDbType(v, c);
                        p.Value = v;
                        cmd.Parameters.Add(p);
                    }
                }

                var waiter = cmd.BeginExecuteNonQuery();
                while (!waiter.IsCompleted)
                    yield return null;

                cmd.EndExecuteNonQuery(waiter);
            }
        }

        // 删除指定 id 的数据
        IEnumerator DeleteImpl(IDType id)
        {
            yield return null;

            using (var conn = OpenConnection())
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(@"Delete from {0} where ID = {1}", tbName, NamedParam("ID"));

                IDbDataParameter idParam = cmd.CreateParameter();
                idParam.ParameterName = RealParam("ID");
                idParam.DbType = GetDbType(id, "ID");
                idParam.Value = id;
                cmd.Parameters.Add(idParam);

                var waiter = cmd.BeginExecuteNonQuery();
                while (!waiter.IsCompleted)
                    yield return null;

                cmd.EndExecuteNonQuery(waiter);
            }
        }

        // 同步指定 id 的数据
        IEnumerator LoadImpl(IDType id, Action<byte[]> cb)
        {
            yield return null;

            using (var conn = OpenConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(@"select Data from {0} where ID = {1}", tbName, NamedParam("ID"));

                IDbDataParameter idParam = cmd.CreateParameter();
                idParam.ParameterName = RealParam("ID");
                idParam.DbType = GetDbType(id, "ID");
                idParam.Value = id;
                cmd.Parameters.Add(idParam);

                var waiter = cmd.BeginExecuteReader();
                while (!waiter.IsCompleted)
                    yield return null;

                using (var r = cmd.EndExecuteReader(waiter))
                {
                    if (r.Read())
                        cb((byte[])r.GetValue(0));
                    else
                        cb(null);
                }
            }
        }

        // 加载所有数据
        IEnumerator LoadAllImpl(string whereClause, Action<byte[][]> cb)
        {
            yield return null;

            using (var conn = OpenConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(@"select Data from {0} {1}", tbName, whereClause);

                var waiter = cmd.BeginExecuteReader();
                while (!waiter.IsCompleted)
                    yield return null;

                using (var r = cmd.EndExecuteReader(waiter))
                {
                    List<byte[]> dataLst = new List<byte[]>();
                    while (r.Read())
                    {
                        object dataObj = r.GetValue(0);
                        byte[] data = dataObj is DBNull ? null : (byte[])dataObj;
                        dataLst.Add(data);
                        yield return null;
                    }

                    cb(dataLst.ToArray());
                }
            }
        }

        // 加载所有数据
        IEnumerator LoadAllIDImpl(string whereClause, Action<IDType[]> cb)
        {
            yield return null;

            using (var conn = OpenConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(@"select ID from {0} {1}", tbName, whereClause);

                var waiter = cmd.BeginExecuteReader();
                while (!waiter.IsCompleted)
                    yield return null;

                using (var r = cmd.EndExecuteReader(waiter))
                {
                    List<IDType> dataLst = new List<IDType>();
                    while (r.Read())
                    {
                        IDType uid = (IDType)r.GetValue(0);
                        dataLst.Add(uid);
                        yield return null;
                    }

                    cb(dataLst.ToArray());
                }
            }
        }

        #region 保护部分

        // 数据库连接字串
        public string ConnStr
        {
            get { return connStr; }
        }
        string connStr = null;

        protected MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(ConnStr);
            conn.Open();
            return conn;
        }

        // 根据字段名取存放到数据库中的对应值
        protected Func<T, string, object> cvm = null;

        // 额外的列
        protected string[] cols = null;

        // 数据表名
        protected string tbName = null;

        // 检查指定表是否存在
        protected bool TableExists(string tableName)
        {
            using (var conn = OpenConnection())
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("show tables like '{0}';", tableName);
                MySqlDataReader r = cmd.ExecuteReader();
                bool exists = r.Read();
                r.Close();
                return exists;
            }
        }

        #region 需要继承实现的部分

        // 包装形参名
        protected string NamedParam(string name)
        {
            return "?" + name;
        }

        // 包装实参名
        protected string RealParam(string name)
        {
            return name;
        }

        #endregion

        // 获取对应的 MySql 类型
        public static DbType GetDbType(object d, string paraName)
        {
            if (d == null)
                throw new Exception("unsupported null type by GetDbType: " + paraName);
            else if (d is bool)
                return DbType.Boolean;
            else if (d is string)
                return DbType.String;
            else if (d is DateTime)
                return DbType.DateTime;
            else if (d is float)
                return DbType.Single;
            else if (d is Int16)
                return DbType.Int16;
            else if (d is Int32)
                return DbType.Int32;
            else if (d is Int64)
                return DbType.Int64;
            else if (d is UInt16)
                return DbType.UInt16;
            else if (d is UInt32)
                return DbType.UInt32;
            else if (d is UInt64)
                return DbType.UInt64;
            else if (d is byte)
                return DbType.Byte;
            else if (d is byte[])
                return DbType.Binary;
            else
                throw new Exception("unsupported type: " + paraName + ":" + d.GetType().Name);
        }

        // 获取对应的 MySql 类型名称
        public static string GetDbTypeName(Type t, string paraName)
        {
            if (t == null)
                throw new Exception("unsupported null type by GetDbTypeName: " + paraName);
            else if (t == typeof(bool)) // if (d is bool)
                return "Bool";
            else if (t == typeof(string)) // if (d is string)
                return "Blob";
            else if (t == typeof(DateTime)) // if (d is DateTime)
                return "DateTime";
            else if (t == typeof(float)) // else if (d is float)
                return "Float";
            else if (t == typeof(Int16)) // else if (d is Int16)
                return "SmallInt";
            else if (t == typeof(Int32)) // else if (d is Int32)
                return "Int";
            else if (t == typeof(Int64)) // else if (d is Int64)
                return "BigInt";
            else if (t == typeof(Int16)) // else if (d is UInt16)
                return "SmallInt";
            else if (t == typeof(UInt32)) // else if (d is UInt32)
                return "Int";
            else if (t == typeof(UInt64)) // else if (d == typeof(UInt64)) // else if (d is UInt64)
                return "BigInt";
            else if (t == typeof(byte)) // else if (d is byte)
                return "TinyInt";
            else if (t == typeof(byte[])) // else if (d is byte[])
                return "Blob";
            else
                throw new Exception("unsupported type: " + paraName + ":" + t.Name);
        }

        #endregion
    }
}
