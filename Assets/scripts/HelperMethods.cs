using UnityEngine;
using System.Collections;

public class HelperMethods : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static bool IsShiftKeysDown()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); //controlKey
    }
}
