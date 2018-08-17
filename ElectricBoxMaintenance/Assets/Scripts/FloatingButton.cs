using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

using UnityEngine.UI;


public class FloatingButton : MonoBehaviour, IInputClickHandler {
    
    Vector3 initialScale;
    Vector3 targetScale;

    public delegate void ClickDelegate(GameObject button);
    public event ClickDelegate Clicked;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        //Debug.Log("OnInputClicked" + eventData.selectedObject);
        Clicked(gameObject);
        eventData.Use();
    }

    public void setActiveStatus(bool status)
    {
        gameObject.SetActive(status);
    }

    // Use this for initialization
    void Start () {
        // setup text and icon of the button

	}
	
	// Update is called once per frame
	void Update () {
       
	}

}
