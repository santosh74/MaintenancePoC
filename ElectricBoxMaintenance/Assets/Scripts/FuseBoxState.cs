using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBoxState : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StateUpdate(string state)
    {
        Debug.Log(state);
        GameObject.Instantiate(prefab, transform.position, transform.rotation);
    }
}
