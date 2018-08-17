using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

public class ChangeOnTouch : MonoBehaviour, IInputHandler, IFocusable, ISourceStateHandler {

    public float LerpSpeed = 2.5f;
    bool cubeOutward = true;
    InputEventData gazeData;
    float alpha;

    bool tooClose = false;


    public void OnFocusEnter()
    {
        InputManager.Instance.PushModalInputHandler(gameObject);
    }

    public void OnFocusExit()
    {
        InputManager.Instance.PopModalInputHandler();
    }

    public void OnInputDown(InputEventData eventData)
    {
        cubeOutward = false;

    }

    public void OnInputUp(InputEventData eventData)
    {
        Debug.Log("OnInputDown");

        cubeOutward = true;

        Debug.Log(cubeOutward);

    }

    // Use this for initialization
    void Start () {



	}

    // Update is called once per frame
    void Update()
    {
        MoveCube();
        
        //cubeOutward = false;
    }

    void MoveCube()
    {
        Debug.Log("MoveCube");

        Debug.Log(cubeOutward);


        float distance = Vector3.Distance(transform.position, CameraCache.Main.transform.position);


        if (!cubeOutward && distance > 2f)
        {
            Vector3 camPos = CameraCache.Main.transform.position;
            transform.position = Vector3.Lerp(transform.position, camPos, LerpSpeed * Time.deltaTime);

        }
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {

    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        cubeOutward = true;
    }

    
}
