using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class Zmovement : MonoBehaviour, IInputHandler, ISourceStateHandler, IFocusable
{

    IInputSource inputSource;
    uint sourceID;

    GameObject box;
    bool inFocus;
    bool clicked;

    Vector3 initialPosition;
    Vector3 pointerPosition;
    Vector3 previousPosition;

    public void OnSourceDetected(SourceStateEventData eventData)
    {
    }
    
    void Start()
    {
        box = gameObject.transform.parent.parent.transform.gameObject;
    }
    
    void Update()
    {
        if (clicked && (inputSource != null))
        {
            Move();
        }
    }

    public void Move()
    {
            if (clicked && inputSource.TryGetGripPosition(sourceID, out pointerPosition))
            {
                Vector3 handMovementDirection = pointerPosition - previousPosition;
                handMovementDirection = transform.InverseTransformDirection(handMovementDirection);
                handMovementDirection.z = 0f;
                handMovementDirection.x = 0f;
                handMovementDirection = transform.TransformDirection(handMovementDirection);
                previousPosition = pointerPosition;
                box.transform.position += handMovementDirection * 3f;
            }
    }

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
        InputManager.Instance.PopModalInputHandler();
        clicked = false;
        inputSource = null;
    }
    
    public void OnFocusEnter()
    {
        InputManager.Instance.PushModalInputHandler(gameObject);
        inFocus = true;
    }

    public void OnFocusExit()
    {
        inFocus = false;
        
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
        clicked = false;
        inputSource = null;
    }
}
