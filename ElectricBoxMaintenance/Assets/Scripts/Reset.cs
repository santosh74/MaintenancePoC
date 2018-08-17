using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class Reset : MonoBehaviour,IInputHandler
{
    public GameObject Camera;
    public GameObject Box;

    public void OnInputDown(InputEventData eventData)
    {
        Camera.GetComponent<RaycastPositioningV1>().enabled = true;
        Box.SetActive(false);
        
    }

    public void OnInputUp(InputEventData eventData)
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
