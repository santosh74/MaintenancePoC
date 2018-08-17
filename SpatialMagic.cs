using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity.InputModule;


public class ScriptTest : MonoBehaviour, IInputHandler, ISourceStateHandler {

    public GameObject ElectricBox;

    bool MoveObject;

    public void OnInputDown(InputEventData eventData)
    {
        MoveObject = true;
        eventData.Use();
    }

    public void OnInputUp(InputEventData eventData)
    {
        MoveObject = false;
        eventData.Use();
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        eventData.Use();
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        MoveObject = false;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (MoveObject)
        {
            GameObject CameraParent = GameObject.Find("MixedRealityCameraParent");
            Transform Camera = CameraParent.transform.GetChild(0);

            Vector3 RaycastDirection = Camera.transform.forward;

            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.position, RaycastDirection, out hitInfo, 2f, 31))
            {
                ElectricBox.transform.position = hitInfo.point;
            }
        }
    }
}
