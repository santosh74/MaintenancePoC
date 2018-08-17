using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnNotify(Message message)
    {
        Debug.Log("OnNotify : state " + message.state + " name : " + message.name);

        if (message.state == 1)
        {
            Debug.Log("Destroyed hand animation");
            DestroyObject(this);
        }

    }
}
