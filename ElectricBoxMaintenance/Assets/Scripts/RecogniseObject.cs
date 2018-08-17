using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class RecogniseObject : MonoBehaviour, IInputClickHandler {

    [SerializeField]
    public GameObject _object;
    public TextMesh SpatialText;

    bool inScan = false;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(inScan)
        {
            SpatialUnderstanding.Instance.RequestFinishScan();
            SpatialUnderstanding.Instance.ScanStateChanged += ScanStateChanged;
            inScan = false;
        }
        else
        {
            SpatialUnderstanding.Instance.RequestBeginScanning();
            SpatialUnderstanding.Instance.ScanStateChanged -= ScanStateChanged;
            inScan = true;
        }

        eventData.Use();
    }

    public void ScanStateChanged()
    {
        switch (SpatialUnderstanding.Instance.ScanState)
        {
            case SpatialUnderstanding.ScanStates.Scanning:
                SpatialText.text = "Scan Start";
                ///LogSurfaceState();
                break;
            case SpatialUnderstanding.ScanStates.Done:
                SpatialText.text = "Scan Finish";
                ///LogSurfaceState();
                InstantiateObjectOnTable();
                break;
            default:
                break;
        }
    }

    /*
    private void LogSurfaceState()
    {
        IntPtr statPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
        if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statPtr) != 0)
        {
            var stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
            string debugInfo = String.Format("TotalSurfaceArea: {0:0.##}\n" +
                                      "WallSurfaceArea: {1:0.##}\n" +
                                      "HorizSurfaceArea: {2:0.##}",
                                      stats.TotalSurfaceArea,
                                      stats.WallSurfaceArea,
                                      stats.HorizSurfaceArea);
            Debug.Log(debugInfo);
        }
    }
    */

    void AddShapeDefinition(string shapeName,
                        List<SpatialUnderstandingDllShapes.ShapeComponent> shapeComponent,
                        List<SpatialUnderstandingDllShapes.ShapeConstraint> shapeConstraint)
    {
        IntPtr shapeComponentsPtr = (shapeComponent == null) ? IntPtr.Zero : SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(shapeComponent.ToArray());
        IntPtr shapeConstraintsPtr = (shapeConstraint == null) ? IntPtr.Zero : SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(shapeConstraint.ToArray());
        if (SpatialUnderstandingDllShapes.AddShape(shapeName,
            (shapeComponent == null) ? 0 : shapeComponent.Count,
            shapeComponentsPtr,
            (shapeConstraint == null) ? 0 : shapeConstraint.Count,
            shapeConstraintsPtr) == 0)
        {
            SpatialText.text = "Failed to create object";
        }
    }

    private void CreateElectricBoxShape()
    {
        var shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_IsRectangle(0.1f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Between(0.4f, 0.6f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Between(0.3f, 0.5f)
                }),

            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_IsRectangle(0.1f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Between(0.15f, 0.2f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Between(0.3f, 0.5f)
                }),

            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_IsRectangle(0.1f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Between(0.4f, 0.6f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Between(0.15f, 0.2f)
                }),

        };

        var shapeConstraints = new List<SpatialUnderstandingDllShapes.ShapeConstraint>()
        {
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_RectanglesSameLength(0,2),
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_RectanglesPerpendicular(0,1),
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_RectanglesPerpendicular(0,2)
        };

        AddShapeDefinition("ElectricBox", shapeComponents, shapeConstraints);
        SpatialUnderstandingDllShapes.ActivateShapeAnalysis();
    }

    private void InstantiateObjectOnTable()
    {
        const int MaxResultCount = 512;
        var shapeResults = new SpatialUnderstandingDllShapes.ShapeResult[MaxResultCount];

        var resultsShapePtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(shapeResults);
        var locationCount = SpatialUnderstandingDllShapes.QueryShape_FindPositionsOnShape("ElectricBox", 0.1f, shapeResults.Length, resultsShapePtr);

        if (locationCount > 0)
        {
            Instantiate(_object,
                        shapeResults[0].position,
                        Quaternion.LookRotation(shapeResults[0].position.normalized, Vector3.up));
            // For some reason the halfDims of the shape result are always 0,0,0 so we can't scale 
            // to the size of the surface. This may be a bug in the HoloToolkit?

            SpatialText.text = "Placed Hologram";
        }
        else
        {
            SpatialText.text = "Hologram not placed";
        }
    }


    // Use this for initialization
    void Start () {
        InputManager.Instance.PushModalInputHandler(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
