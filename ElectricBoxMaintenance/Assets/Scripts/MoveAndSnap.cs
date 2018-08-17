using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using VertexUnityPlayer;

public class MoveAndSnap : MonoBehaviour, IInputHandler, ISourceStateHandler, IFocusable
{
    Vector3 initialPosition;
    Vector3 pointerPosition;
    Vector3 positionZ;
    GameObject targetObject;

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
        //Searching.SearchForObject();
        //GameObject firstObj = GameObject.Find("UXHandler");
        //GameObject nextObject = firstObj.transform.FindChild("Box").gameObject;

        //firstObj.transform.FindChild("Box").gameObject.SetActive(true);
        //nextObject.transform.FindChild("BoxModelV9").gameObject.SetActive(true);
        //Debug.Log("Found obj in Searching:" + firstObj);
        //Debug.Log("Found obj in Move script: ")

    }

    public void OnInputUp(InputEventData eventData)
    {
        clicked = false;
        inputSource = null;
        //snapped = true;
    }


    // Use this for initialization
    void Start()
    {
        
        //CreateSwitch();
        //SceneLink.Instance.CreateNode("Switch", new Vector3(0, 0, 0), Quaternion.identity, Vector3.one, "a00e3fa6-babb-498a-a559-e9ae05cf9c31", null,"Switch");
        positionZ.z = initialPosition.z;
    }
   //IEnumerator CreateSwitch()
   // {
   //     yield return new WaitForSeconds(2f);
   //     SceneLink.Instance.CreateNode("Swi", new Vector3(0, 0, 0), Quaternion.identity, Vector3.one, "a00e3fa6-babb-498a-a559-e9ae05cf9c31", null, "Switch");
   //     Debug.Log("Created: ");
   // }
    // Update is called once per frame
    void LateUpdate()
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
                //handMovementDirection.z = 0f;
                handMovementDirection = transform.TransformDirection(handMovementDirection);
                previousPosition = pointerPosition;
                gameObject.transform.position += handMovementDirection * 3f;
                
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
    {
        if (!clicked)
        {
            InputManager.Instance.PopModalInputHandler();
            clicked = false;
        }
        else
        {
            clicked = true;
        }
       
    }
    
    public void OnTriggerEnter(Collider col)
    {

        //GameObject targetObject = SceneLink.Instance.transform.Find("Breaker").gameObject;

        Debug.Log("OnTriggerEnter " + col.gameObject.name);
        if (col.gameObject.name.Contains("SnapSwitch"))
        //if(col.gameObject == targetObject)

        {
            targetObject = col.gameObject;

            //float targetHeight = targetObject.GetComponent<MeshRenderer>().bounds.size.y;
            //float targetLength = targetObject.GetComponent<MeshRenderer>().bounds.size.x;
            //float targetDepth = targetObject.GetComponent<MeshRenderer>().bounds.size.z;

            //float currentObjHeight = gameObject.GetComponent<MeshRenderer>().bounds.size.y;
            //float currentObjLength = gameObject.GetComponent<MeshRenderer>().bounds.size.x;
            //float currentObjDepth = gameObject.GetComponent<MeshRenderer>().bounds.size.z;

            if (gameObject.name == "SWITCH" )
            {
                
                clicked = false;
                inputSource = null;
                gameObject.transform.position =
                        //new Vector3(targetObject.transform.position.x, targetObject.transform.position.y + targetHeight/2 + currentObjHeight/2, targetObject.transform.position.z);
                new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, targetObject.transform.position.z);
                //targetObject.GetComponent<BoxCollider>().isTrigger = false;
                //targetObject.GetComponent<Rigidbody>().isKinematic = false;
                targetObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            }
        }
        else if (col.gameObject.name.Contains("SnapConnector"))
        {
            targetObject = col.gameObject;
            //float targetHeight = targetObject.GetComponent<MeshRenderer>().bounds.size.y;
            //float targetLength = targetObject.GetComponent<MeshRenderer>().bounds.size.x;
            //float targetDepth = targetObject.GetComponent<MeshRenderer>().bounds.size.z;

            //float currentObjHeight = gameObject.GetComponent<MeshRenderer>().bounds.size.y;
            //float currentObjLength = gameObject.GetComponent<MeshRenderer>().bounds.size.x;
            //float currentObjDepth = gameObject.GetComponent<MeshRenderer>().bounds.size.z;

            if (gameObject.name == "CONNECTOR")
            {

                clicked = false;
                inputSource = null;
                transform.position = targetObject.transform.position;
                //transform.rotation = targetObject.transform.rotation;
               
                 //targetObject.GetComponent<BoxCollider>().isTrigger = false;
                //targetObject.GetComponent<Rigidbody>().isKinematic = false;
                //targetObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                Debug.Log("SNAPPED!");
                //Destroy(gameObject.GetComponent<MoveAndSnap>());
                
            }
        }

    }
    public void OnCollisionExit(Collision col)
    {
       
        //targetObject.GetComponent<BoxCollider>().isTrigger = true;
        //targetObject.GetComponent<Rigidbody>().isKinematic = true;

    }
   
}
