using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VertexUnityPlayer;


public class VertxEventHandler : MonoBehaviour
{
    string newValue;

    bool correctStep = true;

    int incorrectStep;


    GameObject currentGameObject;
    public string[,] AnimationArray;

    GameObject PreviousAnimationNode;
    int currentStep;

    public bool IoTEnabled { get; set; }

    private Dictionary<string, string> VoiceOverDictionary = new Dictionary<string, string>();



    // Use this for initialization
    void Start()
    {
        currentStep = 0;
        AnimationArray = new string[,]
        {
                {"KEY_ANIMATION","353f92d5-3f34-4bde-859e-f6bda4c51d0d", "1"}, 
                //{"DOOR_ANIMATION","a7e0309a-2196-4827-8ea9-5239c7dce5ab", "1"}, 
                {"SWITCH_ONE","dba4bf63-f221-4afb-be70-2d4d3a120f0b", "0" },
                {"SWITCH_TWO","6515834e-e299-4169-a2cf-48bbba3fde9c", "1"},
                {"SWITCH_THREE","a2f4d458-ee08-479b-963a-a1e92c029ea2", "0"},
                {"FUSE_ANIMATION","61399449-de19-4183-ab35-48eb4b3e6c98", "0" },
                {"FUSE_ANIMATION_CLOSE","33a4426a-cb3d-4618-9f5e-cd0fb4314c2e", "1" },
                {"SWITCH_FOUR","407a5f53-5f75-4689-8370-fba5d317c212", "1"},
                {"SWITCH_FIVE","4935b63e-3f0d-4614-bd5d-eb4560bad25e", "0"},
                {"SWITCH_SIX","7a62effa-ef7b-47fb-a21e-9a5bab0b10b0", "1"},
                {"DOOR_FINISH","a60d4de1-5ab4-4b61-9865-e6bfbf426c63", "0"},
                {"KEY_ANIMATION_FINISH","41dbd084-6dd4-4f24-af2d-0ce3c2dccfc0","0"}
        };

        VoiceOverDictionary.Add("DOOR_ANIMATION", "03");
        VoiceOverDictionary.Add("SWITCH_ONE", "04");
        VoiceOverDictionary.Add("SWITCH_TWO", "05");
        VoiceOverDictionary.Add("SWITCH_THREE", "06");
        VoiceOverDictionary.Add("FUSE_ANIMATION", "07");
        VoiceOverDictionary.Add("FUSE_ANIMATION_CLOSE", "08");
        VoiceOverDictionary.Add("SWITCH_FOUR", "09");
        VoiceOverDictionary.Add("SWITCH_FIVE", "10");
        VoiceOverDictionary.Add("SWITCH_SIX", "11");
        //VoiceOverDictionary.Add("DOOR_FINISH", "12");
        VoiceOverDictionary.Add("KEY_ANIMATION_FINISH", "12");

    }



    // Update is called once per frame
    void Update()
    {
        if (currentGameObject)
        {
            GameObject box = GameObject.FindGameObjectWithTag("Box");
            currentGameObject.transform.position = box.transform.position;
            currentGameObject.transform.rotation = box.transform.rotation;
        }
    }

    public void setIoTEnabled(bool enabled)
    {
        IoTEnabled = enabled;
    }

    public void OnUpdate(object message)
    {
        // Deserialize the message received by IOT
        Message _message = JsonConvert.DeserializeObject<Message>(message.ToString());

        if (!IoTEnabled)
        {
            return;
        }

        string componentName = _message.name;
        string componentState = _message.state.ToString();

        if (componentName == "DOOR_ANIMATION" && componentState == "1")
        {
            return;
        }


        if (PreviousAnimationNode)
        {
            DestroyImmediate(PreviousAnimationNode);
        }

        if (currentStep > 4)
        {
            switch (componentName)
            {
                case "SWITCH_THREE":
                    componentName = "SWITCH_FOUR";
                    break;
                case "SWITCH_TWO":
                    componentName = "SWITCH_FIVE";
                    break;
                case "SWITCH_ONE":
                    componentName = "SWITCH_SIX";
                    break;
                case "DOOR_ANIMATION":
                    componentName = "DOOR_FINISH";
                    break;
                case "KEY_ANIMATION":
                    componentName = "KEY_ANIMATION_FINISH";
                    break;
                case "FUSE_ANIMATION":
                    componentName = "FUSE_ANIMATION_CLOSE";
                    break;
            }
        }


        // Deserialize the message received by IOT

        //Debug.Log("Component name => " + componentName);
        //Debug.Log("Component state => " + componentState);

        //SAME SWITCHES USED

        if (!isComplete())
        {
            ExecuteAnimation(componentName, componentState);
        }
        else
        {
            PreviousAnimationNode = CreateNode("MAINTANENCE_MODEL", "b7617179-cf1f-45b1-90be-e6d05369de7a");
        } 
    }

    //maintenence completed when all animations have been played
    bool isComplete()
    {
        bool maintenenceComplete;
        if (currentStep == AnimationArray.Length / 3 - 1)
        {
            maintenenceComplete = true;
        }
        else
        {
            maintenenceComplete = false;
        }
        return maintenenceComplete;
    }

    void ExecuteAnimation(string _ComponentName, string _ComponentStatus)
    {
        if (_ComponentName == AnimationArray[currentStep, 0] && _ComponentStatus == AnimationArray[currentStep, 2])
        {
            CreateNode(AnimationArray[currentStep + 1, 0], AnimationArray[currentStep + 1, 1]);
            currentStep++;
            PlayVoiceOver(_ComponentName);
        }
        else
        {
            CreateNode(AnimationArray[currentStep, 0], AnimationArray[currentStep, 1]);
        }
    }

    IEnumerator PlayInitialVoiceOver()
    {
        yield return new WaitForSeconds(1f);
        AudioSource src = SceneLink.Instance.GetComponentInChildren<VertxEventHandler>().GetComponent<AudioSource>();
        Debug.Log("Playing Audio");
        AudioClip clip1 = Resources.Load<AudioClip>("01");
        src.clip = clip1;
        src.Play();

        yield return new WaitForSeconds(4f);
        src = SceneLink.Instance.GetComponentInChildren<VertxEventHandler>().GetComponent<AudioSource>();
        Debug.Log("Playing Audio");
        clip1 = Resources.Load<AudioClip>("02");
        src.clip = clip1;
        src.Play();
    }

    public void InitKeyAnimation()
    {

        StartCoroutine(PlayInitialVoiceOver());
        Debug.Log("Init key animation :");
        currentGameObject = CreateNode("KEY_ANIMATION", "353f92d5-3f34-4bde-859e-f6bda4c51d0d");
        currentGameObject.AddComponent<AnimEventHandler>();
        PreviousAnimationNode = currentGameObject;
        currentStep = 0;
    }

    private void PlayVoiceOver(string componentName)
    {
        Debug.Log("Playing Audio => " + componentName);
        string voiceOverName = null;
        if (VoiceOverDictionary.TryGetValue(componentName, out voiceOverName))
        {
            AudioSource src = SceneLink.Instance.GetComponentInChildren<VertxEventHandler>().GetComponent<AudioSource>();
            Debug.Log("Playing Audio");
            AudioClip clip1 = Resources.Load<AudioClip>(voiceOverName);
            src.clip = clip1;
            src.Play();
        }
    }

    // Method to create and return Vertex Node Link Game object 
    private GameObject CreateNode(string name, string id)
    {
        GameObject box = GameObject.FindGameObjectWithTag("Box");
        var vertxObject = SceneLink.Instance.transform.Find(name);
        GameObject vertxThing;
        if (vertxObject == null)
        {
            vertxThing = SceneLink.Instance.CreateNode(name,
                box.transform.position,
                box.transform.rotation,
                Vector3.one,
                id
           );
        }
        else
        {
            vertxThing = vertxObject.gameObject;
            Debug.Log("node already exists");

        }
        currentGameObject = vertxThing;
        currentGameObject.AddComponent<AnimEventHandler>();
        PreviousAnimationNode = SceneLink.Instance.transform.Find(name).gameObject;
        return vertxThing;

    }


}