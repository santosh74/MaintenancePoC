using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertxMessageHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ButtonEventHandler(string button)
    {
        Debug.Log("Message Received" + button);
        if (button == "LiveInformation")
        {
            Player.Instance.LiveInfo();
        }
        else if (button  == "InteractiveGuide") {

            Player.Instance.InteractiveGuide();
        }
        else if(button == "Collab")
        {
            Player.Instance.Collaboration();
        }
        else if (button == "HomeButton")
        {
            Player.Instance.GoingHome();

        }
        else if(button == "RemoteAssistance")
        {
            Player.Instance.Remote();
        }
    }

    public void DisableClipping(string name)
    {
        Player.Instance.DisableSnapping();
    }
}
