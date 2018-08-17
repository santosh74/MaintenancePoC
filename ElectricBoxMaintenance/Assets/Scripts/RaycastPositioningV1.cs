using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class RaycastPositioningV1 : MonoBehaviour, IInputHandler
{
    public GameObject Quad;
    //public GameObject BoxModel;
    public float lerpTime = 0.01f; 
    RaycastHit hit;
    Vector3 normalAtHitPosition;
    Vector3 pointOfHit;
    int layer = 1 << 31;
    int layer2 = 1 << 8;
    bool raycast = false;
    bool move = true;
    Vector3 initialCameraPosition;
    Vector3 offset = new Vector3(0,0,0.2f);
    private float zOffset = 0.23f;
    public GameObject spatialMesh;
    public GameObject SystemScanMessage;

    
    public void OnInputDown(InputEventData eventData)
    {
        if (raycast)
        {
            raycast = false;
            move = true;
            Quad.GetComponent<Movement>().enabled = true;
            gameObject.GetComponent<RaycastPositioningV1>().enabled = false;
            spatialMesh.SetActive(false);

        }
        else if (move)
        {
            raycast = true;
            move = false;
            Quad.GetComponent<Movement>().enabled = false;
        }

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20f, layer) || Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20f, layer2))
        {
            //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("TransparentFX"))
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Spatial Mapping") || hit.transform.gameObject.layer == LayerMask.NameToLayer("PhysicalBox"))
            //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PhysicalBox"))
            {
                Quad.SetActive(true);
                SystemScanMessage.SetActive(false);

                //BoxModel.SetActive(false);
                
                if ((Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20f, layer) || Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20f, layer2)) && raycast)
                {
                    Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * hit.distance, Color.red);
                    normalAtHitPosition = hit.normal;
                    pointOfHit = new Vector3(hit.point.x - 0.2f, hit.point.y - 0.3f, hit.point.z);
                    //Debug.Log("Normal: " + normalAtHitPosition.ToString());

                    Quad.transform.position = pointOfHit + normalAtHitPosition* zOffset;
                    Quad.transform.rotation = Quaternion.LookRotation(-normalAtHitPosition, Vector3.up);
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
        //if (raycast)
        //{
        //    raycast = false;
        //    move = true;
        //    Quad.GetComponent<Movement>().enabled = true;

        //} else if (move)
        //{
        //    raycast = true;
        //    move = false;
        //    Quad.GetComponent<Movement>().enabled = false;
        //}
       //Quad.GetComponent<Movement>().enabled = true;
       //gameObject.GetComponent<RaycastPositioningV1>().enabled = false;
        
    }

    // Use this for initialization
    void Start ()
    {
        if (!gameObject.GetComponent<RaycastPositioningV1>().enabled)
        {
            gameObject.GetComponent<RaycastPositioningV1>().enabled = true;
        }
        initialCameraPosition = Camera.main.transform.forward;
        InputManager.Instance.AddGlobalListener(gameObject);        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (raycast)
        {
            //Raycast();
        }     
    }

    public void PlacementFinished()
    {
        InputManager.Instance.RemoveGlobalListener(gameObject);
    }

    void Raycast()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20f, layer) || Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20f, layer2))
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * hit.distance, Color.red);
            normalAtHitPosition = hit.normal;
            pointOfHit = new Vector3(hit.point.x-0.2f,hit.point.y-0.3f,hit.point.z);
            //Debug.Log("Normal: " + normalAtHitPosition.ToString());

            Quad.transform.localPosition = pointOfHit + normalAtHitPosition * zOffset;
            Quad.transform.rotation = Quaternion.LookRotation(-normalAtHitPosition, Vector3.up);
            Quad.transform.localEulerAngles = new Vector3(0, Quad.transform.localEulerAngles.y, 0);
        }
    }
}
