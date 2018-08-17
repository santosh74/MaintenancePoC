using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class OnFocusInformation : MonoBehaviour, IFocusable, IInputClickHandler {

    bool Box = false;

    GameObject PreviousComponent;

    public void OnFocusEnter()
    {
        InputManager.Instance.PushModalInputHandler(gameObject);

        //CHANGE EMISSION
    }

    public void OnFocusExit()
    {
        //CHANGE EMISSION BACK
        InputManager.Instance.PopModalInputHandler();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {

        if (PreviousComponent)
        {
            ChangeEmissive(PreviousComponent, 0.0f);
        }

        GameObject selectedComponent = eventData.selectedObject.transform.parent.gameObject;;
        PreviousComponent = selectedComponent;

        switch (selectedComponent.name)
        {
            case "Box001":
                ComponentWindow.Instance.SetPanelText("FUSE", "• Status: OFF", "• Action: Immediate REPLACEMENT");
                break;
            case "Switch1":
                ComponentWindow.Instance.SetPanelText("UPS MAINS INPUT", "• Status: ON", "• 230V, 1 Phase, 2 Wire 50Hz");
                break;
            case "Switch2":
                ComponentWindow.Instance.SetPanelText("MAINTENANCE BYPASS", "• Status: OFF", "• Battery: 230V, 1 Phase, 2 Wire 50Hz");
                break;
            case "Switch3":
                ComponentWindow.Instance.SetPanelText("UPS OUTPUT", "• Status: ON", "• 230V, 1 Phase, 2 Wire 50Hz");
                break;
            default:
                // instruction message
                ComponentWindow.Instance.SetPanelText("Tap on components", "to display more information", "");
                break;
        }

        ComponentWindow.Instance.SetColourPanel(selectedComponent);

        ChangeEmissive(selectedComponent, 0.1f);

        eventData.Use();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void ChangeEmissive(GameObject selectedComponent, float emision)
    {
        Color baseColour;

        if (selectedComponent.tag == "Working")
        {
            baseColour = Color.green;
        }
        else if (selectedComponent.tag == "Faulty")
        {
            baseColour = Color.red;
        }
        else
        {
            baseColour = Color.white;
        }

        Renderer render = selectedComponent.GetComponentInChildren<Renderer>();
        Material mat = render.material;
        Color finalColour = baseColour * Mathf.LinearToGammaSpace(emision);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", finalColour);

    }

}
