using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VertexUnityPlayer;

[RequireComponent(typeof(SceneLink))]
public class SceneLinkEventManager : MonoBehaviour
{
    //public GameObject Camera;
    bool isHost = false;
    bool clientHostSet = false;

    // Use this for initialization
    void Start()
    {
        SceneLink.Instance.OnStateChange += Instance_OnStateChange;
        StartCoroutine(AttachManagers());

    }


    IEnumerator AttachManagers()
    {
        yield return new WaitForSeconds(3f);
        LoadVertxEventManager();
        LoadLiveInformationManager();
    }

    private void Instance_OnStateChange(SceneLinkStatus oldState, SceneLinkStatus newState)
    {
        if (newState == SceneLinkStatus.Connected)
        {
            //StartCoroutine(CheckIfHost());
            //Debug.Log("viewpoint count => " + SceneLink.Instance.ViewpointObject.transform.childCount);
            //if (SceneLink.Instance.transform.Find("Viewpoints").childCount > 1)
            //{
            //    // Disable the Raycast
            //    GameObject uxHandler = GameObject.Find("UXHandler");
            //    uxHandler.GetComponent<Player>().EnableRaycasting(false);
            //}

           // StartCoroutine(CheckIfLocationExists());
        }
    }

    // Method to create and return Vertex Node Link Game object 
    private GameObject CreateNode(string name, string id)
    {
        var vertxObject = SceneLink.Instance.transform.Find(name);
        GameObject vertxThing;

        if (vertxObject == null)
        {
            vertxThing = SceneLink.Instance.CreateNode(name,
                new Vector3(0f, 0f, 0f),
                Quaternion.identity,
                Vector3.one,
                id
           );
        }
        else
        {
            Debug.Log("Node: " + name + " \n already exists");
            vertxThing = vertxObject.gameObject;

        }
        return vertxThing;

    }


    public class SceneViewpointState
    {
        public string id { get; set; }
        public string[] clients { get; set; }
    }

    IEnumerator CheckIfHost()
    {
        yield return new WaitForSeconds(5f);

        //VertexDataTypes.CoreTypes.Models.ViewModels.ResourceViewModel
        //download json file from https://staging.vertx.cloud/session/*SCENE ID*
        //deserialise into SceneViewpointState object
        //check all clients, if length == 1 and SceneLink.Instance.ViewpointId is contained in the list, i am the host
        //Debug.Log("Fetching");
        ResultContainer<SceneViewpointState> result = new ResultContainer<SceneViewpointState>();
        yield return ServiceRequest.Get<SceneViewpointState>("/session/" + SceneLink.Instance.SceneId, result);
        //
        //Debug.Log("Response : " + result.Value.id);
        List<string> clientList = result.Value.clients.ToList();
        //Debug.Log(" count " + clientList.Count);
        if (clientList.Count == 1) //&& clientList.Contains(SceneLink.Instance.ViewpointId))
        {
            isHost = true;
            // Disable the Raycast
            GameObject uxHandler = GameObject.Find("UXHandler");
            uxHandler.GetComponent<Player>().EnableRaycasting(true);
            uxHandler.GetComponent<Player>().EnabledMeshRendering(true);
        }
        else
        {
            isHost = false;
            // Disable the Raycast
            GameObject uxHandler = GameObject.Find("UXHandler");
            uxHandler.GetComponent<Player>().EnableRaycasting(false);
            uxHandler.GetComponent<Player>().EnabledMeshRendering(false);
        }

        //Debug.Log("isHost : " + isHost + "  ID : " + SceneLink.Instance.ViewpointId);
    }

    IEnumerator CheckIfLocationExists()
    {
        Debug.Log("Started fetching location...");
        ResultContainer<VertexDataTypes.CoreTypes.Models.ViewModels.ResourceViewModel> locationResults = new ResultContainer<VertexDataTypes.CoreTypes.Models.ViewModels.ResourceViewModel>();
        yield return ServiceRequest.Get<VertexDataTypes.CoreTypes.Models.ViewModels.ResourceViewModel>("/core/v1.0/resource/" + SceneLink.Instance.SceneId, locationResults);
        Debug.Log("Location Result : " + locationResults.Value.ResourceKeys.Count());

        if(locationResults.Value.ResourceKeys.Contains(LocationManager.Instance.LocationProvider.GetIdentifier() + "-anchor.blob"))
        {
            Debug.Log("Location exists on vertx");
            Debug.Log("Location Syncing");

           LocationManager.Instance.BeginLocationSync();
        } else
        {
            Debug.Log("Location does not exist on vertx");
        }

    }

    void CheckViewpointCount()
    {
        int numChildren = SceneLink.Instance.ViewpointHost.transform.childCount;
        if (numChildren == 0)
            return;

        for (int i = 0; i < numChildren; i++)
        {
            Debug.Log("loaded: " + SceneLink.Instance.ViewpointHost.transform.GetChild(i).name);
        }
        Debug.Log("Mine: " + SceneLink.Instance.ViewpointId);

        if (numChildren > 0)
        {
            GameObject uxHandler = GameObject.Find("UXHandler");
            uxHandler.GetComponent<Player>().EnableRaycasting(false);

            isHost = true;
            clientHostSet = true;
        }
        else
        {
            GameObject uxHandler = GameObject.Find("UXHandler");
            uxHandler.GetComponent<Player>().EnableRaycasting(true);

            isHost = false;
            clientHostSet = true;
        }

    }

    public void CreateCollabVertxObjectHandler()
    {
        GameObject VertxObjectHandlerNode = CreateNode("CollabVertxObjectHandler", null);
        VertxObjectHandlerNode.AddComponent<CollabVertxObjectHandler>();
    }

    public void RemoveCollabVertxObjectHandler()
    {
        foreach (NodeLink a in SceneLink.Instance.GetComponentsInChildren<NodeLink>())
        {
            if (a.name != "VertxEventManager")
            {
                Debug.Log("Destroying object :" + a.name);
                Destroy(a.gameObject);
            }
        }
    }

    public void LoadVertxEventManager()
    {
        GameObject HandlerNode = CreateNode("VertxEventManager", null);
        HandlerNode.AddComponent<VertxEventHandler>();
        HandlerNode.AddComponent<AudioSource>();
    }

    public void LoadLiveInformationManager()
    {
        GameObject VERTXobjectDecompositionHandlerNode = CreateNode("VertxObjectDecompositionHandler", null);
        VERTXobjectDecompositionHandlerNode.AddComponent<ObjectDecompositionManager>();
    }

}
