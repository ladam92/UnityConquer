using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    public float rotatePerSec = 0.2f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        float rotationAngle = rotatePerSec * 360 * Time.deltaTime;
        this.gameObject.transform.rotation *= Quaternion.Euler(0, 0, rotationAngle);
	}
}
