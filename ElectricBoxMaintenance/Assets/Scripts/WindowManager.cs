using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using UnityEngine.UI;

public class WindowManager : MonoBehaviour {

    public PanelWindow recordWindow;
    public PanelWindow propertyWindow;
    public PanelWindow statusWindow;
    public ComponentWindow ComponentWindow;
    ResponseObject responseObject;


    string response = "{\"WindowText\":[{\"WindowName\":\"PropertyWindow\",\"WindowTitle\":\"Properties:\",\"WindowDescription\":\"• Fuse Box ID: (0001A)\"},{\"WindowName\":\"RecordWindow\",\"WindowTitle\":\"Previous Record\",\"WindowDescription\":\"• Last maintained on: (17/04/2018 14:57 GMT)\"},{\"WindowName\":\"StatusWindow\",\"WindowTitle\":\"• Current Status\",\"WindowDescription\":\"• (Isolated/Mains Bypass/Battery Backup/Battery)\"}]}";

    // Use this for initialization
    void Start () {

        recordWindow.Show();
        propertyWindow.Show();
        statusWindow.Show();

        // Desearilize the json string
        //ResponseObject responseObject = new ResponseObject();

        responseObject = JsonConvert.DeserializeObject<ResponseObject>(response);

        Debug.Log(responseObject.WindowText.Count);
        //SetWindowText();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SetWindowText()
    {

        List<WindowText> windowTextList = responseObject.WindowText;

        //Debug.Log("SetWindowText" + windowTextList.Count);
        
        foreach (WindowText windowText in windowTextList)
        {
            Debug.Log("SettingWindowText" + windowText.WindowName);
           
            if(windowText.WindowName == "PropertyWindow")
            {
                propertyWindow.TitleText = windowText.WindowTitle;
                propertyWindow.DescriptionText = windowText.WindowDescription;
            } else if (windowText.WindowName == "RecordWindow")
            {
                recordWindow.TitleText = windowText.WindowTitle;
                recordWindow.DescriptionText = windowText.WindowDescription;
            } else if (windowText.WindowName == "StatusWindow")
            {
                statusWindow.TitleText = windowText.WindowTitle;
                statusWindow.DescriptionText = windowText.WindowDescription;
            }
        }
    }


}
