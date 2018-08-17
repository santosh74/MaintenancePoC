using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class ObjectDecomposition : MonoBehaviour {

    public Transform[] ObjectList;
    public Vector3[] InitialPositions;

    [Header("Timing variables")]
    public float LerpDuration = 0.5f;
    [Header("Spatial variables")]
    public float MaxDistance = 1f;
    public AnimationCurve MovementCurve;

    private bool ExtendedObjects = false;
    private bool MovingPhase = false;

    public void MoveObjectsForwards()
    {
        StopAllCoroutines();
        StartCoroutine(BoxMovementForward());
    }

    IEnumerator BoxMovementForward()
    {

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < ObjectList.Length; i++)
        {
            float offset = MaxDistance * i / ((float)ObjectList.Length - 1);
            StartCoroutine(MoveObject(ObjectList[i], InitialPositions[i], InitialPositions[i] + Vector3.forward * offset, false));
        }
    }


    
    public void MoveObjectsBackwards()
    {
        StopAllCoroutines();
        for (int i = 0; i < ObjectList.Length; i++)
        {
            StartCoroutine(MoveObject(ObjectList[i], ObjectList[i].localPosition, InitialPositions[i], true));
        }
    }


    IEnumerator MoveObject(Transform movingObject, Vector3 startPosition, Vector3 endPosition, bool reverse)
    {
        MovingPhase = true;
        float startTime = Time.time;
        while(Time.time - startTime <= LerpDuration)
        {
            float percentageComplete = (Time.time - startTime)/ LerpDuration;
            float curveValue = MovementCurve.Evaluate(percentageComplete);
        
            float targetXValue = 0f;

            if (reverse)
            {
                targetXValue = curveValue * (endPosition.z - startPosition.z);
            }
            else
            {
                targetXValue = curveValue * (endPosition.z);
            }

            Vector3 targetPosition = startPosition + Vector3.forward * targetXValue;
            movingObject.localPosition = targetPosition;
            yield return null;
        }

        if (reverse)
        {
            ExtendedObjects = false;
        }
        else
        {
            ExtendedObjects = true;
        }

        MovingPhase = false;
        yield return null;
    }

    void InitialiseArrays()
    {

        /*
        GameObject UXHandler = GameObject.Find("UXHandler");
        Transform Box = UXHandler.transform.GetChild(0);
        Transform ElectricBox = Box.GetChild(0);
        */


        ObjectList = new Transform[transform.childCount];
        InitialPositions = new Vector3[transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {

            ObjectList[i] = child;
            InitialPositions[i] = child.localPosition;
            i++;
        }
    }

    void Start()
    {
        InitialiseArrays();
    }


}
