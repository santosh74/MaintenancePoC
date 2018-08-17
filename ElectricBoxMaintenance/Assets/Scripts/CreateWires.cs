using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexUnityPlayer;
using HoloToolkit.Unity.InputModule;

public class CreateWires : MonoBehaviour, IInputClickHandler, IFocusable
{
    GameObject SelectedObject;

    public static int CorrectWireCount = 0;
    public static int IncorrectWireCount = 0;

    public int SwitchesSnapped = 0;
    public int ConnectorsSnapped = 0;
    bool WireCollidersEnabled = false;

    public class Argument
    {
        public string name;
        public int index;


        public Argument(string _name, int _index)
        {
            name = _name;
            index = _index;
        }
    }

    public static string[,] ConnectionArray =
    {
            {"SnapSwitch1","SnapConnector1","5261185c-856e-4124-aa4b-7199244b9863","0"},
            {"SnapSwitch1","SnapSwitch3","e737f296-4aa9-43a5-88ed-99d7f3ed2403","0"},
            {"SnapSwitch2","SnapConnector1","1d9dcb06-b3b8-4e7d-9686-97cb5fcf793d","0"},
            {"SnapSwitch2","SnapConnector2","486df126-051a-4f6e-96dc-6d4d7402e0ca","0"},
            {"SnapSwitch3","SnapConnector2","ad9c2285-eb11-49e1-acd1-174e705d9591","0"},
            //Incorrect Combinations
            {"SnapSwitch1","SnapSwitch2","cb169ce9-a400-4ff4-bc4f-0b8a3fd230ff","0"},
            {"SnapSwitch1","SnapConnector2","1c2a45e6-fcd6-458b-a37a-6fafb784f1fc","0"},
            {"SnapSwitch2","SnapSwitch3","53f0721d-3ac8-4f3e-8029-26caa52239f6","0"},
            {"SnapSwitch3","SnapConnector1","b57aaba3-075c-49b0-8712-14eec28f69bd","0"},
            {"SnapConnector1","SnapConnector2","73973221-46b1-40a7-825a-d43666ff1787","0"}
    };

    public void OnFocusEnter()
    {
        InputManager.Instance.PushModalInputHandler(gameObject);
    }

    public void OnFocusExit()
    {
        InputManager.Instance.PopModalInputHandler();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        //SelectedObject = eventData.selectedObject.transform.parent.gameObject;
        SelectedObject = eventData.selectedObject.gameObject;
        Debug.Log(SelectedObject);


        AddToComparison();
        eventData.Use();
    }

    void RecurrsionParent(Transform PotentialParent)
    {
        if(PotentialParent.parent.name == "SceneLink")
        {
            SelectedObject = PotentialParent.gameObject;
        }
        else
        {
            RecurrsionParent(PotentialParent.parent.transform);
        }
    }

    void AddToComparison()
    {
        if (!CollabVertxObjectHandler.ObjectComparisonList.Contains(SelectedObject))
        {
            CollabVertxObjectHandler.ObjectComparisonList.Add(SelectedObject);
            if (CollabVertxObjectHandler.ObjectComparisonList.Count == 2)
            {
                //do things
                Comparison();
                //ValidateWiring();
                CollabVertxObjectHandler.ObjectComparisonList.Clear();
            }
        }
    }

    void Comparison()
    {
        NodeLink CurrentNodeLink = GetComponent<NodeLink>();

        bool isInCorrectWireIndex = false;

        var ComparisonArray = CollabVertxObjectHandler.ObjectComparisonList.ToArray();
        GameObject firstGameObject = ComparisonArray[0];
        GameObject secondGameObject = ComparisonArray[1];
        for (int i = 0; i < (ConnectionArray.Length/4); i++)
        {
            string name = ConnectionArray[i, 0].ToString() + ConnectionArray[i, 1].ToString();
            Argument args = new Argument(name, i);
            if (firstGameObject.name == ConnectionArray[i,0] && secondGameObject.name == ConnectionArray[i, 1] || secondGameObject.name == ConnectionArray[i, 0] && firstGameObject.name == ConnectionArray[i, 1])
            {
                if(i <= 4)
                {
                    isInCorrectWireIndex = true;
                }

                switch(ConnectionArray[i,3])
                {
                    case "0":
                        CurrentNodeLink.Fire("LoadWire", args);
                        if(isInCorrectWireIndex)
                        {
                            CorrectWireCount++;
                        }
                        else
                        {
                            IncorrectWireCount++;
                        }
                        break;
                    case "1":
                        CurrentNodeLink.Fire("DeleteWire", args);
                        if (isInCorrectWireIndex)
                        {
                            CorrectWireCount--;
                        }
                        else
                        {
                            IncorrectWireCount--;
                        }
                        break;
                }
                break;
            }
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
                SceneLink.Instance.transform.position,
                SceneLink.Instance.transform.rotation,
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

    void LoadWire(Argument args)
    {
        //unpack
        string name = args.name;
        int index = args.index;
        var temp = SceneLink.Instance.transform.Find(name);
        if (temp == null)
        {
            GameObject wire = CreateNode(name, ConnectionArray[index, 2]);
            ConnectionArray[index, 3] = "1";
            wire.AddComponent<RemoveCollider>();
        }

    }

    void DeleteWire(Argument args)
    {
        //unpack class
        string name = args.name;
        int index = args.index;

        GameObject temp = SceneLink.Instance.transform.Find(name).gameObject;
        if (temp != null)
        {
            Destroy(temp);
            ConnectionArray[index, 3] = "0";
        }

    }

    public void ValidateWiring()
    {
        Debug.Log("VALIDATING WIRING");
        int counter = 0;

        for (int i = 0; i < (ConnectionArray.Length / 4); i++)
        {
            if (ConnectionArray[i, 3] == "1")
            {
                counter++;
            }
            else
            {
                counter--;
            }
            
        }

    }

    public void DecrementSwitchCount()
    {
        SwitchesSnapped--;
    }
    public void DecrementConnectorCount()
    {
        ConnectorsSnapped--;
    }
    public void IncrementSwitchCount()
    {
        SwitchesSnapped++;
    }
    public void IncrementConnectorCount()
    {
        ConnectorsSnapped++;
    }

    void Update()
    {
        if (!WireCollidersEnabled)
        {
            if (SwitchesSnapped == 3 && ConnectorsSnapped == 2)
            {
                EnableWireColliders(gameObject);
            }
        }
    }

    void EnableWireColliders(GameObject toSearch)
    {
        foreach (UnityEngine.Transform child in toSearch.transform)
        {
            GameObject childObject = child.gameObject;

            if (toSearch.name.Contains("SnapSwitch") || toSearch.name.Contains("SnapConnector"))
            {
                if (childObject.name == "Primitive")
                {
                    var boxCollider = childObject.GetComponent<BoxCollider>();
                    if (boxCollider)
                    {
                        boxCollider.enabled = true;

                    }
                    continue;
                }
            }
            else
            {
                EnableWireColliders(childObject);
            }
        }
    }

    public void CheckIfValidCircuit()
    {
        if (CorrectWireCount == 5 && IncorrectWireCount == 0)
        {
            Debug.Log("VALID WIRE CONFIGURATION");

            GameObject wireAnimated = CreateNode("AnimatedWires", "7f20c0bb-82e1-480d-87db-81d60888c03d");

            wireAnimated.AddComponent<ElectricityAnim>();
        }
        else
        {
            //highlight badwires red
            //StopCoroutine(ShowBadWires());
            StopAllCoroutines();
            StartCoroutine(ShowBadWires());
        }
    }

    IEnumerator ShowBadWires()
    {
        yield return null;

        for (int i = 5; i < ConnectionArray.Length/4; i++)
        {
            if (ConnectionArray[i, 3] == "1")
            {
                GameObject badWire;
                string name = ConnectionArray[i, 0] + ConnectionArray[i, 1];
                string nameReverse = ConnectionArray[i, 1] + ConnectionArray[i, 0];
                if (SceneLink.Instance.transform.Find(name))
                {
                    badWire = SceneLink.Instance.transform.Find(name).gameObject;
                    RecurrsionSearch(badWire);
                }
                else if (SceneLink.Instance.transform.Find(nameReverse))
                {
                    badWire = SceneLink.Instance.transform.Find(nameReverse).gameObject;
                    RecurrsionSearch(badWire);
                }
                else
                {
                    Debug.Log("bad wire not found");
                }
            }
        }
    }

    public void ResetWireConnections()
    {
        for (int i = 0; i < ConnectionArray.Length/4; i++)
        {
            ConnectionArray[i, 3] = "0"; 
        }
    }

    void RecurrsionSearch(GameObject gameObject)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childObject = gameObject.transform.GetChild(i).gameObject;
            if (childObject.name == "Primitive")
            {
                Renderer renderer = childObject.GetComponent<Renderer>();
                Material mat = renderer.material;
                float emision = Mathf.PingPong(Time.time, 1.0f);
                Color baseColour = Color.red;
                Color finalColour = baseColour * Mathf.LinearToGammaSpace(emision);
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", finalColour);
            }
            else
            {
                RecurrsionSearch(childObject);
            }
        }
    }

}
