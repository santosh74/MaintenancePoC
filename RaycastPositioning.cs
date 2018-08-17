using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class RaycastPositioning : MonoBehaviour, IInputHandler
{
    public GameObject Quad;
    public float lerpTime = 0.01f; 
    RaycastHit hit;
    Vector3 normalAtHitPosition;
    Vector3 pointOfHit;
    //int layer = 1 << 31;
    int layer = 1 << 8;
    bool state = false;    
    
    public void OnInputDown(InputEventData eventData)
    {
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20f, layer))
        {
            //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("TransparentFX"))
            //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Spatial Mapping"))
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PhysicalBox"))
            {
                Quad.SetActive(true);
                state = true;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20f, layer) && state)
                {
                    Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * hit.distance, Color.red);
                    normalAtHitPosition = hit.normal;
                    pointOfHit = hit.point;
                    //Debug.Log("Normal: " + normalAtHitPosition.ToString());

                    Quad.transform.position = pointOfHit;
                    Quad.transform.rotation = Quaternion.LookRotation(normalAtHitPosition, Vector3.up);
                    Quad.transform.localEulerAngles = new Vector3(0, Quad.transform.localEulerAngles.y, 0);
                }
            }
        }
        else
        {
            Debug.Log("Could not find a surface upon tap");
        }
    }

    public void OnInputUp(InputEventData eventData)
    {
        state = false;
       Quad.GetComponent<Movement>().enabled = true;
        //gameObject.GetComponent<RaycastPositioning>().enabled = false;
        
    }

    // Use this for initialization
    void Start ()
    {
        InputManager.Instance.AddGlobalListener(gameObject);        
    }
	
	// Update is called once per frame
	void Update ()
    {       
        //if(state && Quad.GetComponent<OnFocusQuad>().focusState == true)
        //    Raycast();       
    }

    public void PlacementFinished()
    {
        InputManager.Instance.RemoveGlobalListener(gameObject);
    }

    //void Raycast()
    //{
    //    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20f, layer))
    //    {
    //        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * hit.distance, Color.red);
    //        normalAtHitPosition = hit.normal;
    //        pointOfHit = hit.point;
    //        //Debug.Log("Normal: " + normalAtHitPosition.ToString());            

    //        Quad.transform.position = Vector3.Lerp(Quad.transform.position,pointOfHit,lerpTime);
    //        Quad.transform.rotation = Quaternion.LookRotation(normalAtHitPosition, Vector3.up);
    //        Quad.transform.localEulerAngles = new Vector3(0, Quad.transform.localEulerAngles.y, 0);
    //    }
    //}   
}
