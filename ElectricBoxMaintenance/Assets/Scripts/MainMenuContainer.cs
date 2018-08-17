using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuContainer : MonoBehaviour {

    FloatingButton[] buttons;

    public delegate void ClickDelegate(GameObject button);
    public event ClickDelegate ButtonClicked;



    // Use this for initialization
    void Start () {

        buttons = gameObject.GetComponentsInChildren<FloatingButton>();

        foreach (FloatingButton button in buttons)
        {
            button.Clicked += OnButtonClicked;
        }
	}

    private void OnButtonClicked(GameObject button)
    {
        //Debug.Log(button.name + " menu container=>");
        ButtonClicked(button);

    }

    // Update is called once per frame
    void Update () {
		
	}

   public void SetActiveStatus(bool status)
    {
        gameObject.SetActive(status);
    }
}
