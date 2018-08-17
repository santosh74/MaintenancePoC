using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour, IComponent
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnNotify(Message message)
    {
        Debug.Log("OnNotify : " +
            "\nName: " + message.name + 
            " State: " + message.state);

        if(message.state == 1)
        {
            Debug.Log("Destroy this");
            DestroyObject(gameObject);
        }

    }
}
