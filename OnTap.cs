using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class OnTap : MonoBehaviour, IInputHandler, IFocusable {

    Vector3 currentObjectPos;
    Vector3 objectRestPos;
    public float objectExtend = 0.2f;

    bool objectFocus;
    bool moveObject = false;

    float speedLerp = 5f;
    public float holdTimer = 1f;


    void Start()
    {
        objectRestPos = transform.position;
    }

    public void OnFocusEnter()
    {

        InputManager.Instance.PushModalInputHandler(gameObject);
        objectFocus = true;



    }

    public void OnFocusExit()
    {
        InputManager.Instance.PopModalInputHandler();


    }

    public void OnInputDown(InputEventData eventData)
    {
        StartCoroutine(CubeMovementTimer());
        eventData.Use();
    }

    public void OnInputUp(InputEventData eventData)
    {
        eventData.Use();
    }

    void MoveObjectForwards()
    {
        Debug.Log("Moving object forward");
        transform.position = Vector3.Lerp(currentObjectPos, new Vector3(transform.position.x, transform.position.y, objectRestPos.z  - objectExtend), speedLerp * Time.deltaTime);
    }

    void MoveObjectBackwards()
    {
        Debug.Log("Moving object forward");
        transform.position = Vector3.Lerp(currentObjectPos, objectRestPos, speedLerp * Time.deltaTime);
    }

    void Update () {

        currentObjectPos = transform.position;
        if(objectFocus && moveObject)
        {
            MoveObjectForwards();
        }
        else if(objectFocus && !moveObject)
        {
            MoveObjectBackwards();
        }

	}

    IEnumerator CubeMovementTimer()
    {
        moveObject = true;
        yield return new WaitForSeconds(holdTimer);
        moveObject = false;
        yield return new WaitForSeconds(holdTimer);
    }
}
