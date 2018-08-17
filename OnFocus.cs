using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

public class OnFocus : MonoBehaviour, IFocusable
{
    //CAMERA ONFOCUS RANGE ~ 4.5m



    bool onFocus = false;


    Vector3 currentCubePos;
    Vector3 restCubePos;
    Vector3 offsetVector;

    bool moveBack = false;

    //Vector3 closeCubePos = new Vector3(0f, 0f, 10f);


    float lerpSpeed = 5f;

    //Vector3 Close;
    //Vector3 Far = new Vector3(0, 0, 6);

    //bool closeUp = false;

    public void OnFocusEnter()
    {
        InputManager.Instance.PushModalInputHandler(gameObject);
        onFocus = true;
        StartCoroutine(CubeMovement());   

        //Close = transform.position - CameraCache.Main.transform.position;
        Debug.Log("Focusing on " + gameObject);
        //transform.position = Close;
    }

    public void OnFocusExit()
    {
        InputManager.Instance.PopModalInputHandler();

        onFocus = false;
        moveBack = false;



        Debug.Log("Lost foucs on  " + gameObject);
        InputManager.Instance.PopModalInputHandler();
        //transform.position = Far;
    }


    void Start()
    {
        //transform.position = new Vector3(0, 0, 6);
        restCubePos = transform.position;
        offsetVector = new Vector3(0, 0, 10f);

    }

    void Update()
    {
        currentCubePos = transform.position;
        //cubeOutward = false;
        if (onFocus && !moveBack)
        {
            objectMoveForward();
        }
        else if(moveBack)
        {
            objectMoveBackward();
        }
    }

    void objectMoveForward()
    {
        Debug.Log("Moving object forward");
        transform.position = Vector3.Lerp(currentCubePos, offsetVector, lerpSpeed * Time.deltaTime);
    }

    void objectMoveBackward()
    {
        Debug.Log("Moving object backward");
        transform.position = Vector3.Lerp(currentCubePos, restCubePos, lerpSpeed * Time.deltaTime);
    }

    IEnumerator CubeMovement()
    {
        yield return new WaitForSeconds(2f);
        moveBack = true;
        yield return new WaitForSeconds(2f);
        moveBack = false;
    }
}
