using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSceneLinkPosition : MonoBehaviour {

    public GameObject Box;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = Box.transform.position;
        transform.rotation = Box.transform.rotation;
    }
}
