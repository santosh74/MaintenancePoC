using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelDescription : MonoBehaviour {

    public Text titleText;
    public Text descriptionText;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setTitle(string title)
    {
        titleText.text = title;
    }

    public void setDescription(string description)
    {
        descriptionText.text = description;
    }
}
