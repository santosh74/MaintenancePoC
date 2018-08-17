// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;
using VertexUnityPlayer;
using System.Collections.Generic;
using System.Collections;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Component that allows dragging an object with your hand on HoloLens.
    /// Dragging is done by calculating the angular delta and z-delta between the current and previous hand positions,
    /// and then repositioning the object based on that.
    /// </summary>
    public class MoveAndSnap2 : MonoBehaviour, IFocusable, IInputHandler, ISourceStateHandler
    {
        /// <summary>
        /// Event triggered when dragging starts.
        /// </summary>
        public event Action StartedDragging;
        GameObject targetObject;
        bool hasCollided = false;
        public static bool isNotColiding = false;

        
        


        /// <summary>
        /// Event triggered when dragging stops.
        /// </summary>
        public event Action StoppedDragging;

        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        public Transform HostTransform;

        [Tooltip("Scale by which hand movement in z is multiplied to move the dragged object.")]
        public float DistanceScale = 2f;

        public enum RotationModeEnum
        {
            Default,
            LockObjectRotation,
            OrientTowardUser,
            OrientTowardUserAndKeepUpright
        }

        public RotationModeEnum RotationMode = RotationModeEnum.Default;

        [Tooltip("Controls the speed at which the object will interpolate toward the desired position")]
        [Range(0.01f, 1.0f)]
        public float PositionLerpSpeed = 0.2f;

        [Tooltip("Controls the speed at which the object will interpolate toward the desired rotation")]
        [Range(0.01f, 1.0f)]
        public float RotationLerpSpeed = 0.2f;

        public bool IsDraggingEnabled = true;

        private bool isDragging;
        private bool isGazed;
        private Vector3 objRefForward;
        private Vector3 objRefUp;
        private float objRefDistance;
        private Quaternion gazeAngularOffset;
        private float handRefDistance;
        private Vector3 objRefGrabPoint;

        private Vector3 draggingPosition;
        private Quaternion draggingRotation;

        private IInputSource currentInputSource;
        private uint currentInputSourceId;
        private Rigidbody hostRigidbody;
        private bool hostRigidbodyWasKinematic;
        Transform sceneLink;
        List<GameObject> switches = new List<GameObject>();
        public List<string> guidList;
        string grabbedGuid;
        bool isAvailable = true;

        
        IEnumerator StartAddingGuids()
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < sceneLink.childCount; i++)
            {
                //Debug.Log("i Is " + i + " Childcount is " + sceneLink.childCount);
                if (sceneLink.GetChild(i).GetComponent<NodeLink>() && !guidList.Contains(sceneLink.GetChild(i).GetComponent<NodeLink>().Guid.ToString()))
                {
                    guidList.Add(sceneLink.GetChild(i).GetComponent<NodeLink>().Guid.ToString());
                    
                }
            }
        }
      
        private void Start()
        {
            guidList = new List<string>();
            sceneLink = SceneLink.Instance.transform;
            if (HostTransform == null)
            {
                HostTransform = transform;
            }
            StartCoroutine(StartAddingGuids());
            //for (int i = 0; i <sceneLink.transform.childCount; i++)
            //{
            //    Debug.Log("i Is " + i + " Childcount is " + sceneLink.transform.childCount);
            //    if (sceneLink.transform.GetChild(i).GetComponent<NodeLink>())
            //    {
            //        guidList.Add(sceneLink.transform.GetChild(i).GetComponent<NodeLink>().Guid.ToString());
            //        Debug.Log("GUID: "+guidList[i]);
            //    }
            //}
            //foreach (NodeLink x in SceneLink.Instance.transform)
            //{
            //    guidList.Add(x.Guid);
            //}



            hostRigidbody = HostTransform.GetComponent<Rigidbody>();
        }

        private void OnDestroy()
        {
            if (isDragging)
            {
                StopDragging();
            }

            if (isGazed)
            {
                OnFocusExit();
            }
        }

        private void Update()
        {
            
            if (IsDraggingEnabled && isDragging)
            {
                UpdateDragging();
               
            }
        }

        /// <summary>
        /// Starts dragging the object.
        /// </summary>
        public void StartDragging(Vector3 initialDraggingPosition)
        {
           
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (isDragging)
            {
                return;
            }

            // TODO: robertes: Fix push/pop and single-handler model so that multiple HandDraggable components
            //       can be active at once.

            // Add self as a modal input handler, to get all inputs during the manipulation
            InputManager.Instance.PushModalInputHandler(gameObject);

            isDragging = true;
            if (hostRigidbody != null)
            {
                hostRigidbodyWasKinematic = hostRigidbody.isKinematic;
                hostRigidbody.isKinematic = true;
            }

            Transform cameraTransform = CameraCache.Main.transform;

            Vector3 inputPosition = Vector3.zero;
#if UNITY_2017_2_OR_NEWER
            InteractionSourceInfo sourceKind;
            currentInputSource.TryGetSourceKind(currentInputSourceId, out sourceKind);
            switch (sourceKind)
            {
                case InteractionSourceInfo.Hand:
                    currentInputSource.TryGetGripPosition(currentInputSourceId, out inputPosition);
                    break;
                case InteractionSourceInfo.Controller:
                    currentInputSource.TryGetPointerPosition(currentInputSourceId, out inputPosition);
                    break;
            }
#else
            currentInputSource.TryGetPointerPosition(currentInputSourceId, out inputPosition);
#endif

            Vector3 pivotPosition = GetHandPivotPosition(cameraTransform);
            handRefDistance = Vector3.Magnitude(inputPosition - pivotPosition);
            objRefDistance = Vector3.Magnitude(initialDraggingPosition - pivotPosition);

            Vector3 objForward = HostTransform.forward;
            Vector3 objUp = HostTransform.up;
            // Store where the object was grabbed from
            objRefGrabPoint = cameraTransform.transform.InverseTransformDirection(HostTransform.position - initialDraggingPosition);

            Vector3 objDirection = Vector3.Normalize(initialDraggingPosition - pivotPosition);
            Vector3 handDirection = Vector3.Normalize(inputPosition - pivotPosition);

            objForward = cameraTransform.InverseTransformDirection(objForward);       // in camera space
            objUp = cameraTransform.InverseTransformDirection(objUp);                 // in camera space
            objDirection = cameraTransform.InverseTransformDirection(objDirection);   // in camera space
            handDirection = cameraTransform.InverseTransformDirection(handDirection); // in camera space

            objRefForward = objForward;
            objRefUp = objUp;

            // Store the initial offset between the hand and the object, so that we can consider it when dragging
            gazeAngularOffset = Quaternion.FromToRotation(handDirection, objDirection);
            draggingPosition = initialDraggingPosition;

            StartedDragging.RaiseEvent();
        }

        /// <summary>
        /// Gets the pivot position for the hand, which is approximated to the base of the neck.
        /// </summary>
        /// <returns>Pivot position for the hand.</returns>
        private Vector3 GetHandPivotPosition(Transform cameraTransform)
        {
            return cameraTransform.position + new Vector3(0, -0.2f, 0) - cameraTransform.forward * 0.2f; // a bit lower and behind
        }

        /// <summary>
        /// Enables or disables dragging.
        /// </summary>
        /// <param name="isEnabled">Indicates whether dragging should be enabled or disabled.</param>
        public void SetDragging(bool isEnabled)
        {
            if (IsDraggingEnabled == isEnabled)
            {
                return;
            }

            IsDraggingEnabled = isEnabled;

            if (isDragging)
            {
                StopDragging();
            }
        }

        /// <summary>
        /// Update the position of the object being dragged.
        /// </summary>
        private void UpdateDragging()
        {
            Transform cameraTransform = CameraCache.Main.transform;

            Vector3 inputPosition = Vector3.zero;
#if UNITY_2017_2_OR_NEWER
            InteractionSourceInfo sourceKind;
            currentInputSource.TryGetSourceKind(currentInputSourceId, out sourceKind);
            switch (sourceKind)
            {
                case InteractionSourceInfo.Hand:
                    currentInputSource.TryGetGripPosition(currentInputSourceId, out inputPosition);
                    break;
                case InteractionSourceInfo.Controller:
                    currentInputSource.TryGetPointerPosition(currentInputSourceId, out inputPosition);
                    break;
            }
#else
            currentInputSource.TryGetPointerPosition(currentInputSourceId, out inputPosition);
#endif

            Vector3 pivotPosition = GetHandPivotPosition(cameraTransform);

            Vector3 newHandDirection = Vector3.Normalize(inputPosition - pivotPosition);

            newHandDirection = cameraTransform.InverseTransformDirection(newHandDirection); // in camera space
            Vector3 targetDirection = Vector3.Normalize(gazeAngularOffset * newHandDirection);
            targetDirection = cameraTransform.TransformDirection(targetDirection); // back to world space

            float currentHandDistance = Vector3.Magnitude(inputPosition - pivotPosition);

            float distanceRatio = currentHandDistance / handRefDistance;
            float distanceOffset = distanceRatio > 0 ? (distanceRatio - 1f) * DistanceScale : 0;
            float targetDistance = objRefDistance + distanceOffset;

            draggingPosition = pivotPosition + (targetDirection * targetDistance);

            if (RotationMode == RotationModeEnum.OrientTowardUser || RotationMode == RotationModeEnum.OrientTowardUserAndKeepUpright)
            {
                draggingRotation = Quaternion.LookRotation(HostTransform.position - pivotPosition);
            }
            else if (RotationMode == RotationModeEnum.LockObjectRotation)
            {
                draggingRotation = HostTransform.rotation;
            }
            else // RotationModeEnum.Default
            {
                Vector3 objForward = cameraTransform.TransformDirection(objRefForward); // in world space
                Vector3 objUp = cameraTransform.TransformDirection(objRefUp);           // in world space
                draggingRotation = Quaternion.LookRotation(objForward, objUp);
            }

            Vector3 newPosition = Vector3.Lerp(HostTransform.position, draggingPosition + cameraTransform.TransformDirection(objRefGrabPoint), PositionLerpSpeed);
            // Apply Final Position
            //if (hostRigidbody == null)
            //{
                HostTransform.position = newPosition;
            //}
            //else
            //{
            //    hostRigidbody.MovePosition(newPosition);
            //}

            // Apply Final Rotation
            Quaternion newRotation = Quaternion.Lerp(HostTransform.rotation, draggingRotation, RotationLerpSpeed);
            //if (hostRigidbody == null)
            //{
                HostTransform.rotation = newRotation;
            //}
            //else
            //{
            //    hostRigidbody.MoveRotation(newRotation);
            //}

            if (RotationMode == RotationModeEnum.OrientTowardUserAndKeepUpright)
            {
                Quaternion upRotation = Quaternion.FromToRotation(HostTransform.up, Vector3.up);
                HostTransform.rotation = upRotation * HostTransform.rotation;
            }
        }

        /// <summary>
        /// Stops dragging the object.
        /// </summary>
        public void StopDragging()
        {
            if (!isDragging)
            {
                return;
            }
            if (!guidList.Contains(grabbedGuid))
            {
                //guidList.Add(grabbedGuid);
                gameObject.GetComponent<NodeLink>().Fire("UnlockItem", grabbedGuid);
            }

            

            // Remove self as a modal input handler
            InputManager.Instance.PopModalInputHandler();

            

            isDragging = false;
            //isAvailable = true;
            currentInputSource = null;
            currentInputSourceId = 0;
            if (hostRigidbody != null)
            {
                hostRigidbody.isKinematic = hostRigidbodyWasKinematic;
            }
            StoppedDragging.RaiseEvent();
        }

        IEnumerator DeactivateColliderFor(int x, Collider col)
        {
            col.enabled = false;

            yield return new WaitForSeconds(x);

            col.enabled = true;
        }
        
        public void OnFocusEnter()
        {
            grabbedGuid = gameObject.GetComponent<NodeLink>().Guid.ToString();
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (isGazed)
            {
                return;
            }

            isGazed = true;
        }

        public void OnFocusExit()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (!isGazed)
            {
                return;
            }

            isGazed = false;
        }

        public void OnInputUp(InputEventData eventData)
        {
           
            if (currentInputSource != null &&
                eventData.SourceId == currentInputSourceId)
            {
                eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.

                StopDragging();
            }
            for (int i = 0; i < guidList.Count; i++)
            {
                //Debug.Log("In List AFTER InputUP: " + guidList[i]);
            }


        }

        public void OnInputDown(InputEventData eventData)
        {
            
            for (int i = 0; i < guidList.Count; i++)
            {
                if (guidList[i] == grabbedGuid.ToString())
                {
                    isAvailable = true;
                    break;
                }
                else
                {
                    isAvailable = false;
                }

            }

            if (isDragging)
            {
                
                // We're already handling drag input, so we can't start a new drag operation.
                return;
            }
           
            
           

#if UNITY_2017_2_OR_NEWER
            InteractionSourceInfo sourceKind;
            eventData.InputSource.TryGetSourceKind(eventData.SourceId, out sourceKind);
            if (sourceKind != InteractionSourceInfo.Hand)
            {
                if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.GripPosition))
                {
                    // The input source must provide grip positional data for this script to be usable
                    return;
                }
            }
#else
            if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.PointerPosition))
            {
                // The input source must provide positional data for this script to be usable
                return;
            }
#endif // UNITY_2017_2_OR_NEWER

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.

            currentInputSource = eventData.InputSource;
            currentInputSourceId = eventData.SourceId;

            FocusDetails? details = FocusManager.Instance.TryGetFocusDetails(eventData);

            Vector3 initialDraggingPosition = (details == null)
                ? HostTransform.position
                : details.Value.Point;
            
            if (isAvailable)
            {
                for (int i = 0; i < guidList.Count; i++)
                {
                    //Debug.Log("In List before Fire: " + guidList[i]);
                }
                StartDragging(initialDraggingPosition);
                gameObject.GetComponent<NodeLink>().Fire("LockItem", grabbedGuid);
                //Debug.Log("Message sent " + grabbedGuid);
               
            }
            else
            {
                return;
            }


        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            // Nothing to do
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (currentInputSource != null && eventData.SourceId == currentInputSourceId)
            {
                StopDragging();
            }
            
        }

        public void OnTriggerEnter(Collider col)
        {
                if (col.gameObject.name.Contains("SnapSwitch"))
                {
                    GameObject hitObject = col.gameObject;
                
                    if ((gameObject.name.Contains("SWITCH")) && !(col.gameObject.tag == "SnapOccupied") && !(gameObject.tag == "Snapped"))
                    {
                        
                        
                    
                        StopDragging();
                        GameObject box = GameObject.FindGameObjectWithTag("THEBOX");

                        // isNotColiding = false;
                        transform.position = hitObject.transform.position;
                        transform.rotation = box.transform.rotation;

                        //to make sure that it doesnt pick it up again straight after
                        StartCoroutine(DeactivateColliderFor(1,gameObject.GetComponent<Collider>()));

                        Debug.Log(col.GetComponentInParent<CreateWires>().SwitchesSnapped);

                        //col.gameObject.tag = "SnapOccupied";
                        gameObject.tag = "Snapped";
                    col.GetComponentInParent<CreateWires>().IncrementSwitchCount();
                    }
                }
                else if (col.gameObject.name.Contains("SnapConnector"))
                {
                    GameObject hitObject = col.gameObject;

                    if ((gameObject.name.Contains("CONNECTOR")) && !(col.gameObject.tag == "SnapOccupied")&& !(gameObject.tag == "Snapped"))
                    {
                       

                        

                        StopDragging();
                        GameObject box = GameObject.FindGameObjectWithTag("THEBOX");

                        //isNotColiding = false;
                        transform.position = hitObject.transform.position;
                        transform.rotation = box.transform.rotation;
                        // hitObject.GetComponent<BoxCollider>().isTrigger = false;

                        Debug.Log(col.GetComponentInParent<CreateWires>().ConnectorsSnapped);

                        //col.gameObject.tag = "SnapOccupied";
                        gameObject.tag = "Snapped";
                    col.GetComponentInParent<CreateWires>().IncrementConnectorCount();
                }
                }
        }

        void OnTriggerStay(Collider col)
        {
            if (col.gameObject.name.Contains("SnapSwitch") || col.gameObject.name.Contains("SnapConnector"))
            {
                col.gameObject.tag = "SnapOccupied";
            }
        }

        public void OnTriggerExit(Collider col)
        {
            if (col.gameObject.name.Contains("SnapSwitch"))
            {
                GameObject hitObject = col.gameObject;
                if (col.gameObject.tag == "SnapOccupied")
                {
                    col.gameObject.tag = "Snappable";

                    if (gameObject.name.Contains("SWITCH") && (gameObject.tag == "Snapped"))
                    {

                        col.GetComponentInParent<CreateWires>().DecrementSwitchCount();

                        Debug.Log(col.GetComponentInParent<CreateWires>().SwitchesSnapped);
                        gameObject.tag = "Free";



                    }

                }
            }
            else if (col.gameObject.name.Contains("SnapConnector"))
            {
                GameObject hitObject = col.gameObject;

                if(col.gameObject.tag == "SnapOccupied")
                {

                    col.gameObject.tag = "Snappable";

                    if (gameObject.name.Contains("CONNECTOR") && (gameObject.tag == "Snapped"))
                    {

                        col.GetComponentInParent<CreateWires>().DecrementConnectorCount();
                        Debug.Log(col.GetComponentInParent<CreateWires>().ConnectorsSnapped);
                        gameObject.tag = "Free";
                    }
                }

               
            }
        }

        
        //public void OnCollisionExit(Collision col)
        //{
        //    col.gameObject.AddComponent<Rigidbody>();
        //    col.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //    col.gameObject.GetComponent<Rigidbody>().useGravity = false;
        //}
        void LockItem(string guid)
        {
            if (guidList.Contains(guid))
            {
                guidList.Remove(guid);
                //Debug.Log("Received " + guid);
                for (int i = 0; i < guidList.Count; i++)
                {
                    //Debug.Log("In List AFTER Fire: " + guidList[i]);
                }
            }
        }
        void UnlockItem(string guid)
        {
            if (!guidList.Contains(guid))
            {
                guidList.Add(guid);
            }
        }

    }
    
}
