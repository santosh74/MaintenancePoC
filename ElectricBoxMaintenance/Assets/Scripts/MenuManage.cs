using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class MenuManage : MonoBehaviour, IInputHandler
{
    string choice;
    public GameObject liveInfo;
    public GameObject myGuide;
    public GameObject remoteAssist;
    public GameObject collab;
    bool isActive = true;

    public void OnInputDown(InputEventData eventData)
    {
        choice = gameObject.name.ToString();
        switch (choice)
        {
            case "Live Information":
                {
                    if (isActive)
                    {
                        myGuide.SetActive(false);
                        remoteAssist.SetActive(false);
                        collab.SetActive(false);
                        isActive = !isActive;
                    } else
                    {
                        myGuide.SetActive(true);
                        remoteAssist.SetActive(true);
                        collab.SetActive(true);
                        isActive = !isActive;
                    }
                    break;
                }
            case "My Guide":
                {
                    if (isActive)
                    {
                        liveInfo.SetActive(false);
                        remoteAssist.SetActive(false);
                        collab.SetActive(false);
                        isActive = !isActive;
                    }
                    else
                    {
                        liveInfo.SetActive(true);
                        remoteAssist.SetActive(true);
                        collab.SetActive(true);
                        isActive = !isActive;
                    }
                    break;
                    
                   
                }
            case "Remote Assistance":
                {
                    myGuide.SetActive(false);
                    liveInfo.SetActive(false);
                    collab.SetActive(false);
                    break;
                }
        }
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
