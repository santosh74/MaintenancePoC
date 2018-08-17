using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveCollider : MonoBehaviour
{

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
                RemoveBoxCollider(childObject);
            }
            else
            {
                RecurrsionSearch(childObject);
            }
        }
    }

    void RemoveBoxCollider(GameObject selectedObject)
    {
        if (selectedObject.GetComponent<BoxCollider>())
        {
            Destroy(selectedObject.GetComponent<BoxCollider>());
        }
    }

    void AddMeshCollider(GameObject selectedObject)
    {
        if(selectedObject.GetComponent<MeshCollider>())
        {
            Destroy(selectedObject.GetComponent<MeshCollider>());
        }
    }

}
