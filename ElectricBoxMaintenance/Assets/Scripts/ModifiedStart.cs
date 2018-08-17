using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexUnityPlayer;
using VertexDataTypes;

public class ModifiedStart : MonoBehaviour
{
    List<Component> colliders = new List<Component>();
   // bool hasLoaded = false;
    
    void NodeLink_Loaded()
    {
        
        RecurrsionSearch(gameObject);
      
    }

    void RecurrsionSearch(GameObject toSearch)
    {
        foreach (UnityEngine.Transform child in toSearch.transform)
        {
            GameObject childObject = child.gameObject;

            if (toSearch.name.Contains("Box") || toSearch.name.Contains("SupportShelf"))
            {
                if (childObject.name == "Primitive")
                {
                    Destroy(childObject.GetComponent<BoxCollider>());
                }
            }
            else if (toSearch.name.Contains("SnapSwitch") || toSearch.name.Contains("SnapConnector"))
            {
                if (childObject.name == "Primitive")
                {
                    var boxCollider = childObject.GetComponent<BoxCollider>();
                    if (boxCollider)
                    {
                        var boxColliderSize = boxCollider.size;
                        SphereCollider smallCollider = toSearch.AddComponent<SphereCollider>();
                        smallCollider.radius = smallCollider.radius * 0.01f;
                        smallCollider.isTrigger = true;
                        smallCollider.gameObject.AddComponent<Rigidbody>();
                        smallCollider.GetComponent<Rigidbody>().isKinematic = true;

                        //makes the wire connecting collider shorter, allowing us to grab the component at top and bottom to re-position, but the centre still creates wires.
                        boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y*0.8f, boxCollider.size.x);
                        boxCollider.enabled = false;
                        //Destroy(boxCollider);

                    }
                    // gameObject.AddComponent<IsColiding>();
                    toSearch.layer = UnityEngine.LayerMask.NameToLayer("SnapPoints");
                    continue;
                }
            }
            else
            {
                RecurrsionSearch(childObject);
            }
        }
    }
}