using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRaycast : MonoBehaviour, IFocusable
{
    public GameObject Quad;

    public void OnFocusEnter()
    {
        Quad.GetComponent<RaycastPositioningV1>().enabled = false;
    }

    public void OnFocusExit()
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
