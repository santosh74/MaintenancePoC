using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveButtons : MonoBehaviour
{

    Vector3 zeroPos = new Vector3(0.2f, 0.65f, 0.2f);
    Vector3 liveInfoPos = new Vector3(-0.15f, 0.65f, 0.123f);
    Vector3 interactivePos = new Vector3(0, 0.65f, 0.123f);
    Vector3 RemotePos = new Vector3(0.4f, 0.65f, 0.123f);
    Vector3 collabPos = new Vector3(0.55f, 0.65f, 0.123f);
    FloatingButton[] buttons;

    // Use this for initialization
    void Start()
    {
        buttons = gameObject.GetComponentsInChildren<FloatingButton>();
    }

    private void OnEnable()
    {
        if (buttons == null)
        {

            buttons = gameObject.GetComponentsInChildren<FloatingButton>();
        }

        MoveButton();
    }
    

    IEnumerator FadingIn(FloatingButton i, Vector3 target)
    {
        float t = 0;
        float x = 0;
        float time = 0.5f;
        float timeScale = 1 / time;

        while (t < time)
        {
            t += 1f * Time.deltaTime;

            x = Mathf.Clamp(Mathf.SmoothStep(0f, 1f, t * timeScale), 0f, 1f);

            Vector3 tempPos = i.transform.localPosition;
            tempPos = Vector3.Lerp(zeroPos, target, x);
            i.transform.localPosition = tempPos;

            Vector3 tempScale = i.transform.localScale;
            tempScale = Vector3.Lerp(Vector3.zero, Vector3.one, x);
            //Debug.Log(tempScale);
            i.transform.localScale = tempScale;

            yield return null;
        }
    }

    public void MoveButton()
    {


        foreach (FloatingButton i in buttons)
        {
            if (i.name == "LiveInformation")
            {
                StartCoroutine(FadingIn(i, liveInfoPos));
            }
            else if (i.name == "InteractiveGuide")
            {

                StartCoroutine(FadingIn(i, interactivePos));
            }
            else if (i.name == "RemoteAssistance")
            {
                StartCoroutine(FadingIn(i, RemotePos));
            }
            else if (i.name == "Collab")
            {
                StartCoroutine(FadingIn(i, collabPos));
            }

        }
    }
}