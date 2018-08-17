using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VertexUnityPlayer;

[RequireComponent(typeof(AudioSource))]
public class ComponentWindow : VertexSingleton<ComponentWindow>
{

    public FloatingButton RecordButton;
    public FloatingButton PlayButton;

    public GameObject RecordIcon;
    public GameObject RecordStopIcon;
    public GameObject PlayIcon;
    public GameObject PlayStopIcon;

    bool isRecording = false;
    bool isUploading = false;
    bool isDownloading = false;
    AudioSource AudioSrc;

    GameObject TitleSection;
    GameObject StatusSection;
    GameObject InformationSection;

    const int AUDIO_SAMPLE_RATE = 44100;

    void Start()
    {



        // Get the close button and listen for close events
        RecordButton.Clicked += RecordButton_Clicked;
        PlayButton.Clicked += PlayButton_Clicked;
        AudioSrc = GetComponent<AudioSource>();
        StartCoroutine(GetRecording());
        // set default component texts

        //SetPanelText("Tap on components", "to display more information", "");

        TitleSection = transform.Find("TitleText").gameObject;
        StatusSection = transform.Find("StatusText").gameObject;
        InformationSection = transform.Find("InformationText").gameObject;

        //ResetColourPanel();

        

        Color baseColour;
        Renderer render = TitleSection.GetComponent<Renderer>();
        Material mat = render.material;
        baseColour = Color.white;
        mat.color = baseColour;
    }

    private void PlayButton_Clicked(GameObject button)
    {
        Debug.Log("PlayButton_Clicked!");

     

        GetAndPlayRecording();
    }

    private void RecordButton_Clicked(GameObject button)
    {
        Debug.Log("Record button clicked!");

        if (!isRecording)
        {
            RecordMessage();
            
        }
        else
        {
            StartCoroutine(StopRecording());
        }
    }

    private void CloseButton_Activated(GameObject source)
    {
        Hide();
    }

    // hide panel windows
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // show panel window
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // Methods for Recording functionality
    public void GetAndPlayRecording()
    {
        StartCoroutine(StopRecording());
        StartCoroutine(GetRecording());
        if (AudioSrc && !AudioSrc.isPlaying)
        {
            StartCoroutine(PlayAudio());
            //AudioSrc.Play();
        }
        else if (AudioSrc && AudioSrc.isPlaying)
        {
            AudioSrc.Stop();
            Debug.Log("Stop playing ");
        }
        else
        {
            Debug.Log("Audio source is NULL");
        }
    }

    IEnumerator PlayAudio()
    {
        //PlayIcon.SetActive(false);
        //PlayStopIcon.SetActive(true);

        while (isDownloading)
        {
            yield return new WaitForSeconds(0.5f);
        }
        AudioSrc.Play();
        Debug.Log("Start playing");
    }

    public void RecordMessage()
    {
        StopAllCoroutines();
        StartCoroutine(StartRecording());
    }

    // Coroutine to start recording audio
    IEnumerator StartRecording()
    {
        Debug.Log("RECORD PRESSED");
        RecordIcon.SetActive(false);
        RecordStopIcon.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        isRecording = true;
        isUploading = true;
        AudioSrc.clip = Microphone.Start(Microphone.devices[0], false, 15, AUDIO_SAMPLE_RATE);
        
        while (Microphone.IsRecording(Microphone.devices[0]))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Microphone.End(Microphone.devices[0]);
        isRecording = false;
        // Start Upload coroutine once recording is finished.
        yield return UploadAudioOnServer();
    }

    // Coroutine to upload recorded audio on server
    IEnumerator UploadAudioOnServer()
    {
        float[] samples = new float[AudioSrc.clip.samples * AudioSrc.clip.channels];
        AudioSrc.clip.GetData(samples, 0);

        var byteArray = new byte[samples.Length * 4];
        Buffer.BlockCopy(samples, 0, byteArray, 0, byteArray.Length);

        // Post recorded audio clips on server using UnityWebRequest
        using (UnityWebRequest WebRequest = UnityWebRequest.Post("https://staging.vertx.cloud/core/v1.0/resource/088c0839-d2d1-4808-87d4-a33ca223876e/audioClip.wav", ""))
        {
            WebRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            WebRequest.uploadHandler = new UploadHandlerRaw(byteArray);
            WebRequest.uploadHandler.contentType = "application/octet-stream";
            WebRequest.downloadHandler = new DownloadHandlerBuffer();
            WebRequest.AddVertexAuth();
            yield return WebRequest.SendWebRequest();
            Debug.Log("Audio file Uploaded");
        }
        yield return null;
        isUploading = false;
    }

    IEnumerator StopRecording()
    {
        RecordStopIcon.SetActive(false);
        RecordIcon.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        Debug.Log("RECORD STOPPED");

        isRecording = false;
        if (Microphone.devices.Length > 0)
        {
            Microphone.End(Microphone.devices[0]);
            yield return null;
        }
        else
        {
            Debug.Log("No Microphone device found. Please connect microphone and try again!!");
        }
    }

    IEnumerator GetRecording()
    {
        isDownloading = true;
        using (UnityWebRequest www = UnityWebRequest.Get("https://staging.vertx.cloud/core/v1.0/resource/088c0839-d2d1-4808-87d4-a33ca223876e/audioClip.wav"))
        {
            www.SetRequestHeader("Content-Type", "application/octet-stream");
            www.AddVertexAuth();
            while (isUploading)
            {
                yield return new WaitForSeconds(0.5f);
            }
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;
                float[] downloadArray = new float[results.Length / 4];
                Buffer.BlockCopy(results, 0, downloadArray, 0, results.Length);

                AudioClip clip = AudioClip.Create("voice recording", downloadArray.Length, 1, AUDIO_SAMPLE_RATE, false);
                clip.SetData(downloadArray, 0);
                AudioSrc.clip = clip;

                Debug.Log("Audio file downloaded");
                //yield return new WaitForSeconds(7f);
            }
        }
        yield return new WaitForSeconds(7f);
        isDownloading = false;
    }

    public void SetPanelText(string name, string status, string desc)
    {
        if(TitleSection)
        {

        TitleSection.GetComponent<TextMesh>().text = name;
        }
        if(StatusSection)
            StatusSection.GetComponent<TextMesh>().text = status;
        if (InformationSection)
            InformationSection.GetComponent<TextMesh>().text = desc;
    }

    public void SetColourPanel(GameObject selectedComponent)
    {

        List<Renderer> RenderList = new List<Renderer>();

        Color baseColour;
        if(TitleSection) RenderList.Add(TitleSection.GetComponent<Renderer>());
        if(StatusSection) RenderList.Add(StatusSection.GetComponent<Renderer>());


        foreach (Renderer render in RenderList)
        {
            Material mat = render.material;

            if (selectedComponent.tag == "Working")
            {
                baseColour = Color.green;
            }
            else if (selectedComponent.tag == "Faulty")
            {
                baseColour = Color.red;
            }
            else
            {
                baseColour = new Color(1,1,1,0);
            }

            mat.color = baseColour;
            

        }

    }

}
