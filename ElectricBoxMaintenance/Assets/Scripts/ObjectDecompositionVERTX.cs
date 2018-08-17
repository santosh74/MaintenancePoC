
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexUnityPlayer;

public class ObjectDecompositionVERTX : MonoBehaviour {

    static public GameObject boxObject;

    public Transform[] ObjectList;
    public Vector3[] InitialPositions;

    [Header("Timing variables")]
    public float LerpDuration = 1.0f;
    [Header("Spatial variables")]
    public float MaxDistance = 0.3f;
    public AnimationCurve MovementCurve = new AnimationCurve();

    public static bool ExtendedObjects = true;
    public static bool MovingPhase = true;

    public void MoveObjectsForwards()
    {
        StopAllCoroutines();
        for (int i = 0; i < ObjectList.Length; i++)
        {
            float offset = MaxDistance * i / ((float)ObjectList.Length - 1);
            StartCoroutine(MoveObject(ObjectList[i], InitialPositions[i], InitialPositions[i] + Vector3.forward * offset, false));
        }
    }

    public IEnumerator DecomposeVERTX()
    {
        yield return new WaitForSeconds(1);
        MoveObjectsForwards();
    }

    public void ComposeVERTX()
    {
        MoveObjectsBackwards();
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
        //add movement curve frames
        MovementCurve.AddKey(new Keyframe(0, 0, 0, 0));
        MovementCurve.AddKey(new Keyframe(1, 1, 0, 0));
        StartCoroutine(DecomposeVERTX());
    }

}
