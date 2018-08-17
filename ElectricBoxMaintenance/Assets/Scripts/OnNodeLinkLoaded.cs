using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnNodeLinkLoaded : MonoBehaviour {

    void NodeLink_Loaded()
    {
        RecurrsionSearch(gameObject);
    }

    void RecurrsionSearch(GameObject gameObject)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childObject = gameObject.transform.GetChild(i).gameObject;
            if (childObject.name == "Primitive")
            {
                MeshChange(childObject);
            }
            else
            {
                RecurrsionSearch(childObject);
            }
        }
    }

    void MeshChange(GameObject selectedObject)
    {
        if (selectedObject.GetComponent<BoxCollider>())
        {
            Destroy(selectedObject.GetComponent<BoxCollider>());
        }

        MeshCollider meshCollider = selectedObject.GetComponent<MeshCollider>();

        if (!meshCollider && selectedObject.transform.parent.name == "Switch3" || selectedObject.transform.parent.name == "Switch2" || selectedObject.transform.parent.name == "Switch1" || selectedObject.transform.parent.name == "Box001")
        {
            meshCollider = selectedObject.AddComponent<MeshCollider>();
            meshCollider.skinWidth = 0.0001f;
            if(selectedObject.transform.parent.name == "Box001")
            {
                selectedObject.transform.parent.tag = "Faulty";
            }
            else
            {
                selectedObject.transform.parent.tag = "Working";
            }
        }
    }

}
