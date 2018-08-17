using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexUnityPlayer;
using VertexDataTypes;

public class SwitchAndConnectorColliderRemoval : MonoBehaviour
{

    void NodeLink_Loaded()
    {
        RecurrsiveDownwards(gameObject);
        MakeColliderSmaller();
    }

    void MakeColliderSmaller()
    {
        GameObject ParentObject = gameObject.transform.parent.gameObject;
        if (ParentObject.name == "SceneLink")
        {
            var boxCollider = gameObject.GetComponent<BoxCollider>();

            if (boxCollider)
            {
                boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z*0.6f); 

                //Destroy(gameObject.GetComponent<BoxCollider>());
            }
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
                Debug.Log("SPARTAAAAAAAA"+childObject.name);
                RemoveBoxCollider(childObject);
            }
            else
            {
                RecurrsiveDownwards(childObject);
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

    //void AddMeshCollider(GameObject selectedObject)
    //{
    //    if (selectedObject.GetComponent<MeshCollider>() == null)
    //    {
    //        selectedObject.AddComponent<MeshCollider>();
    //        MeshCollider mesh = selectedObject.GetComponent<MeshCollider>();
    //        StartCoroutine(JustThat(mesh));
    //    }
    //}

    //IEnumerator JustThat(MeshCollider mesh)
    //{
    //    yield return new WaitForSeconds(1);
    //    mesh.skinWidth = 0.0001f;
    //}
}
