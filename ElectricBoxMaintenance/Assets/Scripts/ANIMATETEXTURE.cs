using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANIMATETEXTURE : MonoBehaviour {

    private Material matt;
    float offset;
    public float speed = 0.025f;

	// Use this for initialization
	void Start () {
        matt = GetComponent<Renderer>().material;

        offset = 0.5f;
    }
	
	// Update is called once per frame
	void Update () {

        matt.SetTextureOffset("_MainTex", new Vector2(0, offset));
        offset += speed/10;
        if (offset >= 1f)
        {
            offset = 0f;
            
        }
    }
}
