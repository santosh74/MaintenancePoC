using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricityAnim : MonoBehaviour
{
    List<Transform> primitives ;

    Material material;

    float offset1 = 0.1f;
    public float speed = 0.01f;

    // Use this for initialization
    void Start()
    {
        primitives = new List<Transform>();
        StartCoroutine(WaitThenCall(gameObject.transform));
    }

    IEnumerator WaitThenCall(Transform go)
    {
        yield return new WaitForSeconds(3);

        RecursiveSearch(go);

    }

    //Finds primitive vertx objects that contain materials
    void RecursiveSearch(Transform go)
    {

        for (int i = 0; i < go.transform.childCount; i++)
        {
            Transform child = go.transform.GetChild(i);

            Debug.Log(child.name);

            if (child.name == "Primitive")
            {
                Material mat = child.GetComponent<Renderer>().material;
                primitives.Add(child);
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.white);
            }
            else
            {
                RecursiveSearch(child);
            }
        }
    }

    // Increments offset and updates material location in all wires
    void Update()
    {
        Debug.Log("primitives found: " + primitives.Count + " with offset: " + offset1);

        offset1 -= speed;
        if (offset1 <= 0f)
        {
            offset1 = 1f;
        }

        foreach (Transform wire in primitives)
        {
            material = wire.GetComponent<Renderer>().material;
            material.SetTextureOffset("_MainTex", new Vector2(0, offset1));
        }
        
    }

}
