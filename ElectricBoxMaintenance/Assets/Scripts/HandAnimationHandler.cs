using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimationHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public class UpdateMsg
    {
        public string name { get; set; }
        public string state { get; set; }
    }

    public void OnUpdate(object message)
    {

        UpdateMsg messageReceived=  JsonConvert.DeserializeObject<UpdateMsg>((message.ToString()));
        Debug.Log("OnUpdate: " + messageReceived.name + " status => " + messageReceived.state);

        //Destroy(gameObject, 2f);
    }
}
