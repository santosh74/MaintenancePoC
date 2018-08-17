using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertxNodeHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnUpdate(object message)
    {
        // Deserialize the message received by IOT
        Message _message = JsonConvert.DeserializeObject<Message>(message.ToString());
        Debug.Log("OnUpdate: " + _message.name + " status => " + _message.state);

        // Update component 
        // UpdateComponentStatus(_message);

        //Validate message received from IoT and then Start next set of instruction base on message
        //ValidateUserAction(_message);
    }
}
