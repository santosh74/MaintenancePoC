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
    public GameObject Camera;
    public GameObject MainBox;
    public GameObject BoundingBox;
    public GameObject SpatialUnderstanding;
    public GameObject sceneLink;
    public GameObject SpatialMapping;
    bool boxStatus = true;
    bool inDecomp = false;
    // panels for live information
    GameObject ComponentWindowPanel;




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

        if (homeButton.isActiveAndEnabled)
        {
            homeButton.setActiveStatus(false);
        }

        SceneLink.Instance.OnStateChange += SceneLink_OnStateChange;

        
    }

    // On scene connect, Handler is set up
    private void SceneLink_OnStateChange(SceneLinkStatus oldState, SceneLinkStatus newState)
    {
        Debug.Log("SceneLink_OnStateChange - VERTX connected : " + newState);
        if (newState == SceneLinkStatus.Connected)
        {
            Debug.Log("SceneLink_OnStateChange - VERTX connected : ");
            StartCoroutine(ResetVertxEventHandler());
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
        if(SceneLink.Instance.transform.GetComponentInChildren<CreateWires>())
        {
            SceneLink.Instance.transform.GetComponentInChildren<CreateWires>().ResetWireConnections();
            Debug.Log("Values reset");
        }

        button.SetActive(false);
        if(inDecomp)
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
        }

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

    // Menu container button click event handler
    private void OnButtonClicked(GameObject button)
    {

        if (button.name == "LiveInformation")
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
        else if (button.name == "InteractiveGuide")
        {
            //WholeBox.SetActive(false);
            LoadKeyAnimation();
            mainMenuContainer.SetActiveStatus(false);
            windowManager.SetActive(false);
            Reset.setActiveStatus(false);
            homeButton.setActiveStatus(true);
            
        }
        else if(button.name == "Collab")
        {


            StartCoroutine(EnableIoTListeners(false));
            //WholeBox.SetActive(false);
            mainMenuContainer.SetActiveStatus(false);
            windowManager.SetActive(false);
            Reset.setActiveStatus(false);
            homeButton.setActiveStatus(true);
            StartCoroutine(StartCollaberation());


            
        }
    }

    // Coroutine to start loading assets from vertx

    IEnumerator StartCollaberation()
    {
        yield return new WaitForSeconds(0.5f);

        

        // Disable IoT component attached to the SceneLink
        SceneLink.Instance.GetComponent<SceneLinkEventManager>().CreateCollabVertxObjectHandler();
        sceneLink.GetComponent<SwitchAndConnectorNode>().enabled = true;
        MainBox.GetComponent<BoxCollider>().enabled = false;

        ValidateButton.setActiveStatus(true);
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
        EnableIoTListeners(false);
        foreach (NodeLink a in SceneLink.Instance.GetComponentsInChildren<NodeLink>())
        {
            if(a.name != "VertxEventManager")
            {
                Debug.Log("Destroying object :" + a.name);
                Destroy(a.gameObject);
               
            }

        }
        yield return null;
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
