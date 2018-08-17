using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class PartDecomposition : MonoBehaviour, IInputHandler, IFocusable {

    public AnimationCurve MovementCurve;

    float LerpDuration = 3f;
    float TargetX = 2f;

    bool objectMoved = false;
    bool movingPhase = false;

    public void OnFocusEnter()
    {
        InputManager.Instance.PushModalInputHandler(gameObject);
    }

    public void OnFocusExit()
    {
        InputManager.Instance.ClearModalInputStack();
    }

    public void OnInputDown(InputEventData eventData)
    {
        Debug.Log("BoxModelV4");
        if(objectMoved && !movingPhase)
        {
            movingPhase = true;
            Debug.Log("Moving object back");
            StopAllCoroutines();
            StartCoroutine(MoveObject(0));

        }
        else if(!objectMoved && !movingPhase)
        {
            movingPhase = true;
            Debug.Log("Moving object");
            StopAllCoroutines();
            StartCoroutine(MoveObject(TargetX));
        }

        eventData.Use();
    }

    public void OnInputUp(InputEventData eventData)
    {
        eventData.Use();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator MoveObject(float _TargetX)
    {
        float startTime = Time.time;

        while(Time.time - startTime <= LerpDuration)
        {
            float percentageComplete = (Time.time - startTime) / LerpDuration;
            percentageComplete = MovementCurve.Evaluate(percentageComplete);
            if(objectMoved)
            {
                percentageComplete = 1f - percentageComplete;
                _TargetX += transform.localPosition.x;
            }
            transform.localPosition = new Vector3(_TargetX * percentageComplete, 0f, 0f);
            yield return null;
        }
        transform.localPosition = new Vector3(_TargetX, 0f, 0f);
        if(objectMoved)
        {
            objectMoved = false;
        }
        else
        {
            objectMoved = true;
        }

        movingPhase = false;
        yield return null;
    }

}
