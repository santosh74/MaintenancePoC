using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexUnityPlayer;

public class UpdateText : MonoBehaviour {

    public SceneLink SceneLink;
    private TextMesh _text;

	// Use this for initialization
	void Start () {
        _text = GetComponent<TextMesh>();

        SceneLink.OnStateChange += SceneLink_OnStateChange;
	}

    private void SceneLink_OnStateChange(SceneLinkStatus oldState, SceneLinkStatus newState)
    {
        _text.text = newState.ToString("F");
    }
}
