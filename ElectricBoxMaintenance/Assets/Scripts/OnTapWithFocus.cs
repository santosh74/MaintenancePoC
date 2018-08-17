using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class OnTapWithFocus : MonoBehaviour, IInputHandler, IFocusable
{

    Vector3 currentObjectPos;
    Vector3 objectRestPos;
    public float objectExtend = 0f;
    


    Vector3 currentScale;
    Vector3 restSize;
    public float scaleFactor = 1.2f;

    //bool objectFocus;
    bool moveObject = false;
    bool resizeObject = false;
    float resizeLerp = 5f;

    float speedLerp = 5f;
    public float holdTimer = 0.5f;



    void Start()
    {
        objectRestPos = transform.position;
        restSize = transform.localScale;
    }

    public void OnFocusEnter()
    {

        InputManager.Instance.PushModalInputHandler(gameObject);
        //objectFocus = true;
        resizeObject = true;

    }

    public void OnFocusExit()
    {
        InputManager.Instance.PopModalInputHandler();
        resizeObject = false;


    }

    public void OnInputDown(InputEventData eventData)
    {
        StartCoroutine(CubeMovementTimer());
        //transform.localScale = transform.localScale * 0.8f;
        eventData.Use();
    }

    public void OnInputUp(InputEventData eventData)
    {
        eventData.Use();
    }

    void MoveObjectForwards()
    {
        Debug.Log("Moving object forward");
        transform.position = Vector3.Lerp(currentObjectPos, new Vector3(transform.position.x, transform.position.y, objectRestPos.z - objectExtend), speedLerp * Time.deltaTime);
    }

    void MoveObjectBackwards()
    {
        Debug.Log("Moving object forward");
        transform.position = Vector3.Lerp(currentObjectPos, objectRestPos, speedLerp * Time.deltaTime);
    }

    void ReSizeUp()
    {
        transform.localScale = Vector3.Lerp(currentScale, new Vector3(scaleFactor, scaleFactor, scaleFactor), resizeLerp * Time.deltaTime);
    }

    void ReSizeDown()
    {
        transform.localScale = Vector3.Lerp(currentScale, restSize, resizeLerp * Time.deltaTime);
    }

    void Update()
    {

        currentObjectPos = transform.position;
        currentScale = transform.localScale;

        if (moveObject)
        {
            MoveObjectForwards();
        }
        else if (!moveObject)
        {
            MoveObjectBackwards();
        }

        if(resizeObject)
        {
            ReSizeUp();
        }
        else
        {
            ReSizeDown();
        }
    }

    IEnumerator CubeMovementTimer()
    {
        moveObject = true;
        yield return new WaitForSeconds(holdTimer);
        moveObject = false;
        yield return new WaitForSeconds(holdTimer);
    }

    IEnumerator ReSizeTimer()
    {
        resizeObject = true;
        yield return new WaitForSeconds(2);
        resizeObject = false;
        yield return new WaitForSeconds(2);
    }
}
