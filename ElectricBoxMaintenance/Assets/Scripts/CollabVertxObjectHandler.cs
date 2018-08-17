using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexUnityPlayer;
using HoloToolkit.Unity.InputModule;
using System.Threading;

public class CollabVertxObjectHandler : MonoBehaviour {

    public string[,] ComponentArray;
    bool doItOnce = true;

    float xPosition;

    GameObject VertxBoxComponent;
    List<GameObject> vertxGameObjects;
    List<string> allObjects;

    public static List<GameObject> ObjectComparisonList = new List<GameObject>();
    public static int GameObjectCounter;

    // Use this for initialization
    IEnumerator SpawnShit()
    {
        vertxGameObjects = new List<GameObject>();
        allObjects = new List<string>();
        ComponentArray = new string[,]
        {
            {"SWITCH_1", "27a9c182-675a-4f53-b344-d50a1c874839"},
            {"SWITCH_2", "27a9c182-675a-4f53-b344-d50a1c874839"},
            {"SWITCH_3", "27a9c182-675a-4f53-b344-d50a1c874839"},
            {"CONNECTOR_1", "bf2610b2-9f65-4017-beda-0f7dc1ec57a8"},
            {"CONNECTOR_2", "bf2610b2-9f65-4017-beda-0f7dc1ec57a8"},
            {"BOX", "aaad70db-a138-467e-94af-b73c5888bd38" }
        };

        //spawn vertx objects
        for (int i = 0; i < (ComponentArray.Length / 2); i++)
        {
            //POSITIONAL CHANGES

            if (ComponentArray[i, 0].Contains("CONNECTOR"))
            {
                yield return new WaitForSeconds(0.2f);
                GameObject connectorComponent = CreateNode(ComponentArray[i, 0], ComponentArray[i, 1], "ConnectorNodeLink");
                vertxGameObjects.Add(connectorComponent);
                connectorComponent.AddComponent<SwitchAndConnectorColliderRemoval>();
            }
            if (ComponentArray[i, 0] == "BOX")
            {
                yield return new WaitForSeconds(0.2f);
                VertxBoxComponent = CreateNode(ComponentArray[i, 0], ComponentArray[i, 1], "BoxNodeLink");
                GameObject box = GameObject.FindGameObjectWithTag("Box");
                VertxBoxComponent.transform.position = box.transform.localPosition;
                VertxBoxComponent.transform.rotation = box.transform.rotation;
                VertxBoxComponent.tag = "THEBOX";
                VertxBoxComponent.AddComponent<CreateWires>();

            }
            if (ComponentArray[i, 0].Contains("SWITCH"))
            {
                yield return new WaitForSeconds(0.2f);
                GameObject switchComponent = CreateNode(ComponentArray[i, 0], ComponentArray[i, 1], "SwitchNodeLink");
                vertxGameObjects.Add(switchComponent);
                switchComponent.AddComponent<SwitchAndConnectorColliderRemoval>();
            }
        }

        yield return new WaitForSeconds(2f);
        foreach (NodeLink a in SceneLink.Instance.GetComponentsInChildren<NodeLink>())
        {
            yield return new WaitForSeconds(0.1f);
            if (allObjects.Contains(a.name))
            {
                Destroy(a.gameObject);
            }
            else
            {
                allObjects.Add(a.name);
            }

        }

        yield break;
    }


    void Start ()
    {
        StartCoroutine(SpawnShit());
    }



    // Method to create and return Vertex Node Link Game object 
    private GameObject CreateNode(string name, string id, string nodelink)
    {


        GameObject box = GameObject.FindGameObjectWithTag("Box");
        var vertxThing = SceneLink.Instance.CreateNode(name,
            box.transform.position,
            box.transform.rotation,
            Vector3.one,
            id,
            null,
           nodelink
        );
        return vertxThing.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (VertxBoxComponent)
        {
            GameObject box = GameObject.FindGameObjectWithTag("Box");
            VertxBoxComponent.transform.position = box.transform.position;
            VertxBoxComponent.transform.rotation = box.transform.rotation;

            var xOffset = 0.4f;
            var offset = 0.2f;
            var xPos = VertxBoxComponent.transform.position.x;
            var zPos = VertxBoxComponent.transform.position.z;
            var yPos = VertxBoxComponent.transform.position.y;

            if (doItOnce)
            {
                Debug.Log("Box position: " + box.transform.localPosition);
                for (int i = 0; i < vertxGameObjects.Count; i++)
                {

                    //vertxGameObjects[i].transform.parent = VertxBoxComponent.transform;
                    
                    xOffset -= 0.1f;
                    vertxGameObjects[i].transform.rotation = VertxBoxComponent.transform.rotation;

                    vertxGameObjects[i].transform.position = new Vector3 (VertxBoxComponent.transform.position.x , VertxBoxComponent.transform.position.y , VertxBoxComponent.transform.position.z);
                    vertxGameObjects[i].transform.Translate(-xOffset,0.3f,0);



                    if (vertxGameObjects[i].name.Contains("CONNECTOR"))
                    {
                        vertxGameObjects[i].transform.position = new Vector3(VertxBoxComponent.transform.position.x, VertxBoxComponent.transform.position.y, VertxBoxComponent.transform.position.z);
                        vertxGameObjects[i].transform.Translate(-offset, 0.2f, 0);
                        offset -= 0.1f;
                    }
                    //Debug.Log("Switch position: " + vertxGameObjects[i].transform.position);
                }
                doItOnce = false;
            }
        }

    }
}
