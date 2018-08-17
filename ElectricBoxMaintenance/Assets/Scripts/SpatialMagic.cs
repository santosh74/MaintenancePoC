using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity.InputModule;


public class SpatialMagic : MonoBehaviour, IInputClickHandler, ISourceStateHandler {

    public GameObject ElectricBox;

    bool MoveObject = true;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(MoveObject)
        {
            MoveObject = false;
        }
        else
        {
            MoveObject = true;
        }
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
        GameObject CameraParent = GameObject.Find("MixedRealityCameraParent");
        Transform Camera = CameraParent.transform.GetChild(0);

        Vector3 RaycastDirection = Camera.transform.forward;

        RaycastHit hitInfo;

        if (Physics.Raycast(Camera.position, RaycastDirection, out hitInfo, 2f) && hitInfo.transform.tag != "Ignore" && MoveObject)
        {
            ElectricBox.transform.position = hitInfo.point;
            ElectricBox.transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
        }

    }
}
