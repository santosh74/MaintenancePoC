using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{

    Renderer[] childRenderers;
    Image[] childImages;
    public bool visible;

    // Use this for initialization
    void Start()
    {
        childRenderers = GetComponentsInChildren<Renderer>();
        childImages = GetComponentsInChildren<Image>();
        visible = false;

        foreach (Renderer i in childRenderers)
        {
            i.material.color = new Color(i.material.color.r, i.material.color.g, i.material.color.b, 0);
        }



        foreach (Image i in childImages)
        {
            i.color = new Color(i.material.color.r, i.material.color.g, i.material.color.b, 0);
        }

    }


    public void Fade()
    {
        StartCoroutine(FadeWithWait(1.0f));
        StartCoroutine(ImgFadeWithWait(1.0f));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutWithWait(0.2f));
        StartCoroutine(ImgFadeOutWithWait(0.2f));
    }


    IEnumerator FadeWithWait(float x)
    {
        yield return new WaitForSeconds(x);

        foreach (Renderer i in childRenderers)
        {
            StartCoroutine(FadingIn(i));
        }
    }

    IEnumerator FadeOutWithWait(float x)
    {
        yield return new WaitForSeconds(x);

        foreach (Renderer i in childRenderers)
        {
            StartCoroutine(FadingOut(i));
        }
    }

    IEnumerator FadingIn(Renderer i)
    {

        float alpha = i.material.color.a;
        float t = 0;
        while (t < 1f)
        {
            Color color = i.material.color;
            color.a = Mathf.Lerp(0, 1, t);
            t += 1f * Time.deltaTime;
            i.material.color = color;

            yield return null;
        }
    }

    IEnumerator FadingOut(Renderer i)
    {
        float alpha = i.material.color.a;
        float t = 0;
        while (t < 1f)
        {
            Color color = i.material.color;
            color.a = Mathf.Lerp(1, 0, t);
            t += 1f * Time.deltaTime;
            i.material.color = color;

            yield return null;
        }
    }





    IEnumerator ImgFadeWithWait(float x)
    {
        yield return new WaitForSeconds(x);

        foreach (Image i in childImages)
        {
            StartCoroutine(ImgFadingIn(i));
        }
    }

    IEnumerator ImgFadeOutWithWait(float x)
    {
        yield return new WaitForSeconds(x);

        foreach (Image i in childImages)
        {
            StartCoroutine(ImgFadingOut(i));
        }
    }

    IEnumerator ImgFadingIn(Image i)
    {

        float alpha = i.color.a;
        float t = 0;
        while (t < 1f)
        {
            Color color = i.color;
            color.a = Mathf.Lerp(0, 1, t);
            t += 1f * Time.deltaTime;
            i.color = color;

            yield return null;
        }
    }

    IEnumerator ImgFadingOut(Image i)
    {
        float alpha = i.color.a;
        float t = 0;
        while (t < 1f)
        {
            Color color = i.color;
            color.a = Mathf.Lerp(1, 0, t);
            t += 1f * Time.deltaTime;
            i.color = color;

            yield return null;
        }
    }

}