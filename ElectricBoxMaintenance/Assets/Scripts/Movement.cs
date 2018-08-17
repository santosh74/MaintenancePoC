using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class Movement : MonoBehaviour, IInputHandler, ISourceStateHandler, IFocusable
{
    Vector3 initialPosition;
    Vector3 pointerPosition;
    Vector3 positionZ;


    bool clicked = false;
    IInputSource inputSource;
    uint sourceID;


    public void OnInputDown(InputEventData eventData)
    {
        
        clicked = true;
        sourceID = eventData.SourceId;
        inputSource = eventData.InputSource;
        initialPosition = gameObject.transform.position;
        inputSource.TryGetGripPosition(sourceID, out previousPosition);
    }

    public void OnInputUp(InputEventData eventData)
    {
        
        clicked = false;
        inputSource = null;
    }


    // Use this for initialization
    void Start()
    {
        positionZ.z = initialPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    Vector3 previousPosition;

    public void Move()
    {
        if (inputSource != null)
        {
            if (clicked && inputSource.TryGetGripPosition(sourceID, out pointerPosition))
            {
                Vector3 handMovementDirection = pointerPosition - previousPosition;
                handMovementDirection = transform.InverseTransformDirection(handMovementDirection);
                handMovementDirection.z = 0f;
                handMovementDirection = transform.TransformDirection(handMovementDirection);
                previousPosition = pointerPosition;
                gameObject.transform.position += handMovementDirection*3f;

            }
        }
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
        clicked = false;
    }

    public void OnFocusEnter()
    {
        InputManager.Instance.PushModalInputHandler(gameObject);
        clicked = true;
    }

    public void OnFocusExit()
    {if (!clicked) { 
        InputManager.Instance.PopModalInputHandler();
            clicked = false;
        }
        else
        {
            clicked = true;
        }
        //clicked = false;
    }
}
