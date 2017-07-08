using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 第三人称2D相机
/// </summary>
public class MainCamera : MonoBehaviour
{
    private Transform target;
    public Transform Target
    {
        get { return target; }
        set
        {
            if (value != null)
            {
                target = value;

                // 更新相机位置，避免出现闪屏
                UpdatePos();
            }
        }
    }

    public Camera Camera { get; private set; }

    public float DampTime = 0f;

    // 地图左下角 (0,0) 是左边原点，一屏范围（世界坐标，不是像素）
    float screenWidth = 11.36f;
    float screenHeight = 6.4f;

    public int MapWidth
    {
        get { return mapWidth; }
        set
        {
            mapWidth = value;
        }
    }
    int mapWidth = 0;

    public int MapHeight
    {
        get { return mapHeight; }
        set
        {
            mapHeight = value; ;
        }
    }
    int mapHeight;

    float CalcOrthoSize()
    {
        float standardRatio = screenHeight / screenWidth;
        float currRatio = (float)Screen.height / (float)Screen.width;

        // screenHeight / 2f 是标准的 orthographicSize（即当 curr == standard）
        float orthoSize = screenHeight / 2f;

        if (currRatio > standardRatio)
        {
            // 高度较大，即较接近正方形，让左右少看点东西
        }
        else
        {
            // 宽度较大，即更为瘦长型，让上下少看点东西
            orthoSize *= (currRatio / standardRatio);
        }

        return orthoSize;
    }

    void Start()
    {
        Camera = GetComponent<Camera>();
    }

    // 计算相机位置
    Vector3 CalcCameraPos(float orthoSize)
    {
        float currRatio = (float)Screen.height / (float)Screen.width;

        // 这2个是相机可见区域
        float visibleHalfHeight = orthoSize;
        float visibleHalfWidth = visibleHalfHeight / currRatio;

        // 主角的位置，如果x和y设置和主角一样，那么主角刚好居中
        Vector3 tarPos = target.position;

        // 这里保持相机的z不变，不去改他
        Vector3 camPos = new Vector3(tarPos.x, tarPos.y, transform.position.z);

        return camPos;
    }

    // 更新相机位置
    public void UpdatePos()
    {
        float orthoSize = CalcOrthoSize();
        if (Camera.orthographicSize != orthoSize)
        {
            Camera.orthographicSize = orthoSize;
        }

        if (target != null)
        {
            Vector3 camPos = CalcCameraPos(orthoSize);
            if (transform.position != camPos)
            {
                Vector3 velocity = Vector3.zero;
                camPos = Vector3.SmoothDamp(transform.position, camPos, ref velocity, DampTime);
                transform.position = camPos;
            }
        }
    }

    void LateUpdate()
    {
        UpdatePos();
    }
}