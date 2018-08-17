using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexUnityPlayer;

[RequireComponent(typeof(SceneLink))]
public class FuseBoxS : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        GameObject HandlerNode = CreateNode("VertxEventManager", null);
        HandlerNode.AddComponent<VertxEventHandler>();

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

    // Update is called once per frame
    void Update()
    {

    }
}
