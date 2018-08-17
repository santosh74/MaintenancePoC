using System.Collections;
using UnityEngine;
using VertexUnityPlayer;

public class Player : VertexSingleton<Player>
{
    public GameObject windowManager;
    public MainMenuContainer mainMenuContainer;
    public FloatingButton homeButton;
    public FloatingButton StartButton;
    public FloatingButton Reset;
    public FloatingButton ValidateButton;
    public FloatingButton DisableClippingButton;
    public GameObject Camera;
    public GameObject MainBox;
    public GameObject BoundingBox;
    public GameObject SpatialUnderstanding;
    public GameObject sceneLink;
    public GameObject SpatialMapping;
    public GameObject RemoteAssistance;
    
    bool boxStatus = true;
    bool inDecomp = false;
    // panels for live information
    GameObject ComponentWindowPanel;
    NodeLink CurrentNodeLink;




    // Use this for initialization
    void Start()
    {
        ComponentWindowPanel = windowManager.transform.Find("ComponentWindow").gameObject;


        // create holographic buttons to get started with
        mainMenuContainer.ButtonClicked += OnButtonClicked;
        homeButton.Clicked += HomeButton_Clicked;
        StartButton.Clicked += Start_Clicked;
        Reset.Clicked += Reset_Clicked;
        ValidateButton.Clicked += Validate_Clicked;
        DisableClippingButton.Clicked += DisableClippingButton_Clicked;


        if (homeButton.isActiveAndEnabled)
        {
            homeButton.setActiveStatus(false);
        }
        if (RemoteAssistance)
        {
            RemoteAssistance.SetActive(false);
        }

        SceneLink.Instance.OnStateChange += SceneLink_OnStateChange;

       
    }
    void RemoveBoxCollider(GameObject selectedObject)
    {
        if (selectedObject.GetComponent<BoxCollider>())
        {
            Destroy(selectedObject.GetComponent<BoxCollider>());
        }
    }

    public void RecurrsiveDownwards(GameObject gameObject)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childObject = gameObject.transform.GetChild(i).gameObject;
            Debug.Log(childObject.name);
            if (childObject.name == "Primitive")
            {
                Debug.Log("SPARTAAAAAAAA" + childObject.name);
                RemoveBoxCollider(childObject);
            }
            else
            {
                RecurrsiveDownwards(childObject);
            }
        }


    }


    public void DisableSnapping()
    {
        foreach (NodeLink a in SceneLink.Instance.GetComponentsInChildren<NodeLink>())
        {
            if (a.name.Contains("SWITCH") || a.name.Contains("CONNECTOR"))
            {
                a.GetComponent<BoxCollider>().enabled = false;
                RecurrsiveDownwards(a.gameObject);
            }

        }
    }

    public void DisableClippingButton_Clicked(GameObject button)
    {
        SceneLink.Instance.GetComponentInChildren<NodeLink>().Fire("DisableClipping", button.name);
    }

    // On scene connect, Handler is set up
    private void SceneLink_OnStateChange(SceneLinkStatus oldState, SceneLinkStatus newState)
    {
        Debug.Log("SceneLink_OnStateChange - VERTX connected : " + newState);
        if (newState == SceneLinkStatus.Connected)
        {
            Debug.Log("SceneLink_OnStateChange - VERTX connected : ");
            StartCoroutine(ResetVertxEventHandler());
            // StartCoroutine(GetMessageHandler());
            StartCoroutine(SaveScene());
        }

    }

    IEnumerator GetMessageHandler()
    {
        yield return new WaitForSeconds(10f);

        GameObject messageObj = SceneLink.Instance.transform.Find("VertxMessageHandler").gameObject;
        if (messageObj)
        {
            CurrentNodeLink = messageObj.GetComponent<NodeLink>();
        }
        //CurrentNodeLink = SceneLink.Instance.transform.Find("VertxMessageHandler").GetComponent<NodeLink>();
        if (CurrentNodeLink)
        {
            Debug.Log("Messanger found");
        }
        else
        {
            Debug.Log("Messanger node not found");
        }
    }

    public void Validate_Clicked(GameObject button)
    {
        sceneLink.GetComponentInChildren<CreateWires>().CheckIfValidCircuit();
    }

    // Start button click handler
    public void Start_Clicked(GameObject button)
    {
        StartButton.setActiveStatus(false);
        Reset.setActiveStatus(true);
        SpatialUnderstanding.SetActive(false);
        mainMenuContainer.SetActiveStatus(true);
        MainBox.GetComponent<Movement>().enabled = false;
        BoundingBox.SetActive(false);
        //WholeBox.SetActive(false);

        Camera.GetComponent<RaycastPositioningV1>().enabled = false;

        //LocationManager.Instance.BeginLocationSync();
        //CurrentNodeLink = SceneLink.Instance.transform.Find("VertxMessageHandler").GetComponent<NodeLink>();
    }
    // Reset button click handler
    public void Reset_Clicked(GameObject button)
    {
        Camera.GetComponent<RaycastPositioningV1>().enabled = true;

        MainBox.SetActive(false);
        BoundingBox.SetActive(true);
        Reset.setActiveStatus(false);
        StartButton.setActiveStatus(true);
        mainMenuContainer.SetActiveStatus(false);
        SpatialUnderstanding.SetActive(true);

        SetVertxEventHandlerState(false);
    }

    // HomeButton click event handler
    private void HomeButton_Clicked(GameObject button)
    {
        SceneLink.Instance.GetComponentInChildren<NodeLink>().Fire("ButtonEventHandler", button.name);
    }

    IEnumerator GoToHome()
    {
        //WholeBox.GetComponent<ObjectDecomposition>().MoveObjectsBackwards();
        SceneLink.Instance.GetComponentInChildren<ObjectDecompositionManager>().VertxComposition();
        yield return new WaitForSeconds(1.5f);

        if (boxStatus)
        {
            boxStatus = false;
            //MainBoxDoor.SetActive(true);
            //MainBoxPanel.SetActive(true);
        }
        windowManager.SetActive(false);
        mainMenuContainer.SetActiveStatus(true);
        // Hide home button
        //WholeBox.SetActive(false);
        Reset.setActiveStatus(true);

        SetVertxEventHandlerState(false);
        inDecomp = false;
        //sceneLink.GetComponent<SwitchAndConnectorNode>().enabled = true;
        SceneLink.Instance.GetComponentInChildren<ObjectDecompositionManager>().RemoveBox();
    }

   
    public void GoingHome()
    {
        if (SceneLink.Instance.transform.GetComponentInChildren<CreateWires>())
        {
            SceneLink.Instance.transform.GetComponentInChildren<CreateWires>().ResetWireConnections();
            Debug.Log("Values reset");
        }
        RemoteAssistance.SetActive(false);
        homeButton.setActiveStatus(false);
        if (inDecomp)
        {
            StartCoroutine(GoToHome());
            windowManager.GetComponent<FadeIn>().FadeOut();
        }
        else
        {
            if (boxStatus)
            {
                boxStatus = false;
                //MainBoxDoor.SetActive(true);
                //MainBoxPanel.SetActive(true);
            }
            windowManager.SetActive(false);
            mainMenuContainer.SetActiveStatus(true);
            // Hide home button

            //WholeBox.SetActive(false);
            Reset.setActiveStatus(true);

            SetVertxEventHandlerState(false);
            // REmove CollabVertxObjectHAndler 
            //SceneLink.Instance.GetComponent<SceneLinkEventManager>().RemoveCollabVertxObjectHandler();
            //sceneLink.GetComponent<SwitchAndConnectorNode>().enabled = false;
            //MainBox.GetComponent<Movement>().enabled = false;
            MainBox.GetComponent<BoxCollider>().enabled = true;


            ValidateButton.setActiveStatus(false);
            DisableClippingButton.setActiveStatus(false);
            StartCoroutine(ResetVertxEventHandler());
        }

    }

    public void LiveInfo()
    {

        //WholeBox.SetActive(true);
        mainMenuContainer.SetActiveStatus(false);
        windowManager.SetActive(true);
        homeButton.setActiveStatus(true);
        Reset.setActiveStatus(false);
        BoundingBox.SetActive(false);

        boxStatus = true;
        //MainBoxDoor.SetActive(false);
        //MainBoxPanel.SetActive(false);

        //WholeBox.GetComponent<ObjectDecomposition>().MoveObjectsForwards();
        inDecomp = true;
        windowManager.GetComponent<FadeIn>().Fade();
        //
        SceneLink.Instance.GetComponentInChildren<ObjectDecompositionManager>().VertxDecomposeStart();
        //
        // Set the default values to component panel

        ComponentWindow.Instance.SetPanelText("Tap on components", "to display more information", "");
        ComponentWindow.Instance.SetColourPanel(ComponentWindowPanel);
    }

    public void InteractiveGuide()
    {

        //WholeBox.SetActive(false);
        //LoadKeyAnimation();
        mainMenuContainer.SetActiveStatus(false);
        windowManager.SetActive(false);
        Reset.setActiveStatus(false);
        homeButton.setActiveStatus(true);
    }

    public void Collaboration()
    {
        StartCoroutine(EnableIoTListeners(false));
        //WholeBox.SetActive(false);
        mainMenuContainer.SetActiveStatus(false);
        windowManager.SetActive(false);
        Reset.setActiveStatus(false);
        homeButton.setActiveStatus(true);
        StartCoroutine(StartCollaberation());
    }

    public void Remote()
    {
        RemoteAssistance.SetActive(true);
        mainMenuContainer.SetActiveStatus(false);
        windowManager.SetActive(false);
        homeButton.setActiveStatus(true);
        Reset.setActiveStatus(false);
    }

    // Menu container button click event handler
    private void OnButtonClicked(GameObject button)
    {

        if(button.name == "InteractiveGuide")
        {
            LoadKeyAnimation();
            StartCoroutine(FireMessage(button));
        }
        else
        {
            SceneLink.Instance.GetComponentInChildren<NodeLink>().Fire("ButtonEventHandler", button.name);
        }
        
    }

    IEnumerator FireMessage(GameObject button)
    {
        yield return new WaitForSeconds(1f);
        SceneLink.Instance.GetComponentInChildren<NodeLink>().Fire("ButtonEventHandler", button.name);
    }

    //private void ButtonEventHandler(string button)
    //{
    //    if (button == "LiveInformation")
    //    {
    //        //WholeBox.SetActive(true);
    //        mainMenuContainer.SetActiveStatus(false);
    //        windowManager.SetActive(true);
    //        homeButton.setActiveStatus(true);
    //        Reset.setActiveStatus(false);
    //        BoundingBox.SetActive(false);

    //        boxStatus = true;
    //        //MainBoxDoor.SetActive(false);
    //        //MainBoxPanel.SetActive(false);

    //        //WholeBox.GetComponent<ObjectDecomposition>().MoveObjectsForwards();
    //        inDecomp = true;
    //        windowManager.GetComponent<FadeIn>().Fade();
    //        //
    //        SceneLink.Instance.GetComponentInChildren<ObjectDecompositionManager>().VertxDecomposeStart();
    //        //
    //        // Set the default values to component panel

    //        ComponentWindow.Instance.SetPanelText("Tap on components", "to display more information", "");
    //        ComponentWindow.Instance.SetColourPanel(ComponentWindowPanel);
    //    }
    //    else if (button == "InteractiveGuide")
    //    {
    //        //WholeBox.SetActive(false);
    //        LoadKeyAnimation();
    //        mainMenuContainer.SetActiveStatus(false);
    //        windowManager.SetActive(false);
    //        Reset.setActiveStatus(false);
    //        homeButton.setActiveStatus(true);

    //    }
    //    else if (button == "Collab")
    //    {


    //        StartCoroutine(EnableIoTListeners(false));
    //        //WholeBox.SetActive(false);
    //        mainMenuContainer.SetActiveStatus(false);
    //        windowManager.SetActive(false);
    //        Reset.setActiveStatus(false);
    //        homeButton.setActiveStatus(true);
    //        StartCoroutine(StartCollaberation());
    //    }
    //    else if (button == "RemoteAssistance")
    //    {
    //        RemoteAssistance.SetActive(true);
    //        mainMenuContainer.SetActiveStatus(false);
    //        windowManager.SetActive(false);
    //        homeButton.setActiveStatus(true);
    //        Reset.setActiveStatus(false);
    //    }
    //}

    // Coroutine to start loading assets from vertx

    IEnumerator StartCollaberation()
    {
        yield return new WaitForSeconds(0.5f);

        

        // Disable IoT component attached to the SceneLink
        SceneLink.Instance.GetComponent<SceneLinkEventManager>().CreateCollabVertxObjectHandler();
        sceneLink.GetComponent<SwitchAndConnectorNode>().enabled = true;
        MainBox.GetComponent<BoxCollider>().enabled = false;

        ValidateButton.setActiveStatus(true);
        DisableClippingButton.setActiveStatus(true);
    }

    // Coroutine to load first key animation
     void LoadKeyAnimation()
    {
        Debug.Log("Load Key Animation Co routine ");

        //EnableIoTListeners(true);
        if (SceneLink.Instance.GetComponent<SceneLinkEventManager>() != null)
        {
            SceneLink.Instance.GetComponent<SceneLinkEventManager>().enabled = true;
        }
      
        if (SceneLink.Instance.GetComponentInChildren<VertxEventHandler>() != null)
        {

           // SceneLink.Instance.GetComponentInChildren<VertxEventHandler>().gameObject.SetActive(true);
            SceneLink.Instance.GetComponentInChildren<VertxEventHandler>().enabled = true;
            SceneLink.Instance.GetComponentInChildren<VertxEventHandler>().setIoTEnabled(true);
        }
        if (SceneLink.Instance.GetComponentInChildren<VertxEventHandler>() != null)
        {
            SceneLink.Instance.GetComponentInChildren<VertxEventHandler>().InitKeyAnimation();
        }
    }

    // Coroutine to reset the vertx event manager
    IEnumerator ResetVertxEventHandler()
    {
        yield return new WaitForSeconds(0.5f);
        EnableIoTListeners(false);
        foreach (NodeLink a in SceneLink.Instance.GetComponentsInChildren<NodeLink>())
        {
            if(!(a.name == "VertxEventManager" || a.name == "VertxObjectDecompositionHandler"))
            {
                Debug.Log("Destroying object :" + a.name);
                Destroy(a.gameObject);
            }

        }
    }

    public IEnumerator SaveScene()
    {
        UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(VertexAuthentication.Instance.ServiceUrl + "/session/scene/save/" + SceneLink.Instance.SceneId);
        webRequest.AddVertexAuth();
        yield return webRequest.SendWebRequest();
    }


    // Enable / Disable VertxEventHandler
    private void SetVertxEventHandlerState(bool state)
    {
        StartCoroutine(EnableIoTListeners(state));
        if (!state)
        {
            foreach (AnimEventHandler a in SceneLink.Instance.GetComponentsInChildren<AnimEventHandler>())
            {
                Destroy(a.gameObject);
            }
            // Reset Collaboration objects
            foreach (Transform x in SceneLink.Instance.transform)
            {
                if (x.name.Contains("SWITCH") || x.name.Contains("CONNECTOR") || x.name.Contains("BOX") || x.name.Contains("SnapSwitch1Snap") || x.name.Contains("SnapSwitch2Snap") || x.name.Contains("SnapSwitch3Snap") || x.name.Contains("AnimatedWires"))
                {
                    Destroy(x.gameObject);
                }

            }
        }
        StartCoroutine(SaveScene());

    }

    IEnumerator EnableIoTListeners(bool isEnabled)
    {
        if (SceneLink.Instance.GetComponent<SceneLinkEventManager>() != null)
        {
            SceneLink.Instance.GetComponent<SceneLinkEventManager>().enabled = isEnabled;
        }
        yield return new WaitForSeconds(1.0f);
        if (SceneLink.Instance.GetComponentInChildren<VertxEventHandler>() != null)
        {
            //SceneLink.Instance.GetComponentInChildren<VertxEventHandler>().gameObject.SetActive(isEnabled);
            //VertxEventHandler.IoTEnabled
            SceneLink.Instance.GetComponentInChildren<VertxEventHandler>().setIoTEnabled(isEnabled);
            SceneLink.Instance.GetComponentInChildren<VertxEventHandler>().enabled = isEnabled;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }


    // Function to enable / disable the raycasting
    public void EnableRaycasting(bool isEnabled)
    {
        Camera.GetComponent<RaycastPositioningV1>().enabled = isEnabled;
    }

    public void EnabledMeshRendering(bool isEnabled)
    {
        SpatialUnderstanding.SetActive(isEnabled);
        SpatialMapping.SetActive(isEnabled);
    }

    // Location manager stuff
    public void OnPositionLocated()
    {
        //StartButton.setActiveStatus(false);
        //Reset.setActiveStatus(true);
        //SpatialUnderstanding.SetActive(false);
        //mainMenuContainer.SetActiveStatus(true);
        //MainBox.GetComponent<Movement>().enabled = false;
        //BoundingBox.SetActive(false);
        //WholeBox.SetActive(false);

        //Camera.GetComponent<RaycastPositioningV1>().enabled = false;
    }

    public void OnLocationFailed()
    {
        Debug.Log("OnLocationFailed!!");
    }

    public void OnLocationChanged()
    {
        Debug.Log("OnLocationChanged!!" + LocationManager.Instance.CurrentState);

       // LocationManager.Instance.CurrentState
    }


    


}
