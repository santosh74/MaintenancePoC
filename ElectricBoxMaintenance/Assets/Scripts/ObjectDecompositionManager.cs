using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexUnityPlayer;

public class ObjectDecompositionManager : MonoBehaviour {

    GameObject UXHandler;
    GameObject Box;
    //GameObject WholeBox;
    GameObject ElectricBox;

    List<GameObject> objectList = new List<GameObject>();

    public string[,] DecompositionComponents = new string[,]
    {
        {"BoxLayer","09d3673c-af66-4368-a473-497de950a25b"},
        {"WireLayer","d06fb2ad-49d6-4fab-872e-4ef40d4b6565"},
        {"FuseLayer","7f752527-4e44-41bc-b0d2-fa1ea0cd67a8"},
        {"SwitchLayer","35411d42-5559-4dfe-8420-fa3a64a50f26"}
    };

	// Use this for initialization
	void Start () {

        UXHandler = GameObject.Find("UXHandler");
        Box = UXHandler.transform.Find("Box").gameObject;
        
    }

    private void Update()
    {
        if (ElectricBox)
        {
            ElectricBox.transform.position = Box.transform.position;
            ElectricBox.transform.rotation = Box.transform.rotation;
        }

    }

    void LoadComponents()
    {
        for (int i = 0; i < DecompositionComponents.Length / 2; i++)
        {
            GameObject currentObject = CreateNode(DecompositionComponents[i, 0], DecompositionComponents[i, 1]);

            currentObject.AddComponent<OnFocusInformation>();
            currentObject.AddComponent<OnNodeLinkLoaded>();
        }

        ElectricBox.AddComponent<ObjectDecompositionVERTX>();
    }

    private GameObject CreateNode(string name, string id)
    {

        UXHandler = GameObject.Find("UXHandler");
        Box = UXHandler.transform.Find("Box").gameObject;


        var vertxObject = SceneLink.Instance.transform.Find(name);
        GameObject vertxThing;

        if (vertxObject == null)
        {
            Vector3 spawnPosition;

            if (Box == null)
            {
                spawnPosition = new Vector3(0f, 0f, 0f);
                Debug.Log("Box not found");
            }
            else
            {
                spawnPosition = Box.transform.position;
            }

            // Honestly Siege, I'm making this up at this point, if it works it's a miracle, if it doesn't it's Jamie's fault

            vertxThing = SceneLink.Instance.CreateNode(name,
                spawnPosition,
                Quaternion.Inverse(Box.transform.rotation),
                Vector3.one,
                id
           );
        }
        else
        {
            Debug.Log("Node: " + name + " \n already exists");
            vertxThing = vertxObject.gameObject;
        }

        if (name != "ElectricBox")
        {
            vertxThing.transform.SetParent(ElectricBox.transform);
        }

        if (!objectList.Contains(vertxThing))
        {
            objectList.Add(vertxThing);
        }

        return vertxThing;
    }

    public void VertxDecomposeStart()
    {
        Debug.Log("VertxDecomposeStart : ");
        ElectricBox = CreateNode("ElectricBox", null);
        LoadComponents();
    }

    public void VertxComposition()
    {
        ElectricBox.GetComponent<ObjectDecompositionVERTX>().ComposeVERTX();
    }

    public void RemoveBox()
    {
        Destroy(ElectricBox);
    }

}
