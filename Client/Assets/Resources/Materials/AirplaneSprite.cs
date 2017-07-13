using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneSprite : MonoBehaviour {

    public float TurnAngle = 0;

    // Use this for initialization
    Material airplaneMat = null;
	void Start () {
        airplaneMat = GetComponent<SpriteRenderer>().material;
    }
	
	// Update is called once per frame
	void Update () {
        airplaneMat.SetFloat("_TurnAngle", TurnAngle);
    }
}
