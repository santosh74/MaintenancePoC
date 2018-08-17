using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelWindow : MonoBehaviour {

    //close button
    private FloatingButton closeButton;

    private PanelDescription panelDescription;
    // var to hold the panel width
    public float PanelWidth;
    // var to hold the panel height
    public float PanelHeight;

    public TextMesh titleTextField;
    public TextMesh descriptionTextField;

   
    private string titleText;

    
    private string descriptionText;



    void Start () {

        //Get the panel description container and set the description
        //panelDescription = gameObject.GetComponentInChildren<PanelDescription>();
        //panelDescription.setTitle(TitleText);
        //panelDescription.setDescription(DescriptionText);

        // Get the close button and listen for close events
        closeButton = gameObject.GetComponentInChildren<FloatingButton>();
        closeButton.Clicked += CloseButton_Activated;
       

    }

    private void CloseButton_Activated(GameObject source)
    {
        Hide();
    }

    // hide panel windows
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // show panel window
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public string DescriptionText
    {
        get
        {
            return descriptionText;
        }

        set
        {
            descriptionText = value;

            descriptionTextField.text = descriptionText;

        }
    }

    public string TitleText
    {
        get
        {
            return titleText;
        }

        set
        {
            titleText = value;
            titleTextField.text = titleText;
           
        }
    }

}
