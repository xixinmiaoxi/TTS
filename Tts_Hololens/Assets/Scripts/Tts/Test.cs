#if !NETFX_CORE 
using TtsInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Security;
#else
using System;
using System.IO;
using System.Net;
using System.Text;
using Windows.Storage;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TtsInterface;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Globalization;
using System.Security.Cryptography;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Threading.Tasks;
#endif


public class Test : MonoBehaviour
{
    GameObject Q6;
    GameObject Q7;

    RecordingWav reWav;

    AudioSource source;

    AudioSource VedioSource;

    string Result;

    Image TestImage;

    UITest UITest;

    Animation animations;
    // Use this for initialization
    void Start()
    {
        VedioSource = GameObject.Find("Sofa/Scenario/TV/Canvas_TV/Image_TV").GetComponent<AudioSource>();
        animations = GameObject.Find("Sofa/Scenario/woman").GetComponent<Animation>();
        animations.Stop();
        Q6 = GameObject.Find("Canvas/Q6");
        Q7 = GameObject.Find("Canvas/Q7");

        Q6.GetComponent<Button>().onClick.AddListener(OnClickQ6);
        Q7.GetComponent<Button>().onClick.AddListener(OnClickQ7);

        source = GameObject.Find("Sofa/Scenario/fb").GetComponent<AudioSource>();
        source.loop = false;
        source.playOnAwake = false;

        TestImage = GameObject.FindWithTag("Test").GetComponent<Image>();
        UITest = GameObject.Find("Canvas_Info").GetComponent<UITest>();
        UITest.Init();
    }

    private void Update()
    {
        // TestImage.transform.Rotate(Vector3.forward,0.05f);
    }

#if !NETFX_CORE
    private void OnClickQ6()
    {
        VedioSource.volume = 0.05f;
        GameObject.Find("Sofa/Scenario/fb").AddComponent<RecordingWav>();
        reWav = GameObject.Find("Sofa/Scenario/fb").GetComponent<RecordingWav>();
        reWav.StartRecording();
    }

    private void OnClickQ7()
    {
        string path = Application.persistentDataPath + "/" + reWav.StopRecording() + ".wav";
        Destroy(GameObject.Find("Sofa/Scenario/fb").GetComponent<RecordingWav>());
        StartCoroutine(OnClickQ7WaitForThreadIEnumerator(path));
    }
    private IEnumerator OnClickQ7WaitForThreadIEnumerator(string path)
    {
        Result = null;
        Thread td = new Thread(new ParameterizedThreadStart(OnClickQ7ThreadFun));
        td.Start(path);
        while (true)
        {
            if (Result != null)
            {
                break;
            }
            yield return 0;
        }
        UITest.SetData(Result);
        string audioText = Result.Remove(0, Result.IndexOf(")") + 1);
        StartCoroutine(AnswersToAudio(audioText));
        yield return 0;
    }

    private void OnClickQ7ThreadFun(object path)
    {
        string reWavPath = (string)path;
        string str = VedioToText(reWavPath);
        Debug.Log("str: " + str);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Debug.Log("strDialogText: " + strDialogText);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        Result = result;
    }


    /// <summary>
    /// 语音转文字
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private String VedioToText(string path)
    {
        String ak_id = "LTAIr8I8A5HNZvdt";
        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";
        String urls = "http://nlsapi.aliyun.com/recognize?";
        //使用对应的ASR模型 详情见文档部分2
        String model = "chat";
        urls = urls + "model=" + model;
        byte[] audioBytes;
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            audioBytes = new byte[fs.Length];
            fs.Read(audioBytes, 0, (int)fs.Length);
        }
        String responseRecognize = HttpUtil.sendAsrPost(audioBytes, "pcm", 16000, urls, ak_id, ak_secret);
        return responseRecognize;
    }

    private string DialogText(string text)
    {
        String ak_id = "LTAIr8I8A5HNZvdt";
        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";

        String urlStr = "https://nlsapi.aliyun.com/manage/qas?action=single:prepub:qa";
        String bdStr = "{\"projectId" + "\"" + ":" + 4649 + "," + "\"question" + "\"" + ":" + "\"" + text + "\"}";

        String responseStr = HttpProxy.sendRequest(urlStr, bdStr, ak_id, ak_secret);
        return responseStr;
    }

    private IEnumerator AnswersToAudio(string content)
    {

        String url = "http://nlsapi.aliyun.com/speak?";
        String ak_id = "LTAIr8I8A5HNZvdt";
        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";

        //设置TTS的参数,详细参数说明详见文档部分2.1 参数配置
        TtsRequest ttsRequest = new TtsRequest();
        ttsRequest.setEncodeType("wav");
        ttsRequest.setVoiceName("xiaoyun");
        ttsRequest.setVolume(50);
        ttsRequest.setSampleRate(16000);
        ttsRequest.setSpeechRate(0);
        ttsRequest.setPitchRate(0);
        ttsRequest.setTtsNus(1);
        ttsRequest.setBackgroundMusicId(0);
        ttsRequest.setBackgroundMusicOffset(0);
        ttsRequest.setBackgroundMusicVolume(0);

        String ttsUrlSpeak = url + "encode_type=" + ttsRequest.getEncodeType()
                 + "&voice_name=" + ttsRequest.getVoiceName()
                 + "&volume=" + ttsRequest.getVolume()
                 + "&sample_rate=" + ttsRequest.getSampleRate()
                 + "&speech_rate=" + ttsRequest.getSpeechRate()
                 + "&pitch_rate=" + ttsRequest.getPitchRate()
                 + "&tts_nus=" + ttsRequest.getTtsNus()
                + "&background_music_id=" + ttsRequest.getBackgroundMusicId()
                + "&background_music_offset=" + ttsRequest.getBackgroundMusicOffset()
                + "&background_music_volume=" + ttsRequest.getBackgroundMusicVolume();


        string fileName = System.Guid.NewGuid().ToString("N");
        string response = HttpUtil.sendTtsPost(content, ttsRequest.getEncodeType(), fileName, ttsUrlSpeak, ak_id, ak_secret);
        if (null != response && 0 < response.Length)
        {
            AudioClip audioClip;
            string fileUrl = "file://" + response;
            Debug.Log(fileUrl);
            WWW www = new WWW(fileUrl);
            yield return www;
            if (www.isDone && www.error == null)
            {
                audioClip = www.GetAudioClip();
                source.clip = audioClip;
                source.Play();
                animations.Play();
                animations["sp_source"].speed = 0.5f;
                yield return new WaitForSeconds(audioClip.length);
                animations.Stop();
                animations["sp_source"].time = 0;
                VedioSource.volume = 0.5f;
                if (false == source.isPlaying)
                {
                    File.Delete(response);
                    Debug.Log("删除成功！");
                }
            }
        }
        yield return 0;
    }

#else
    private void OnClickQ6()
    {
        VedioSource.volume = 0.05f;
        GameObject.Find("Sofa/Scenario/fb").AddComponent<RecordingWav>();
        reWav = GameObject.Find("Sofa/Scenario/fb").GetComponent<RecordingWav>();
        reWav.StartRecording();
    }

    private void OnClickQ7()
    {
        //StartCoroutine(OnClickQ7IEnumerator());

        string path = ApplicationData.Current.LocalFolder.Path + "/" + reWav.StopRecording() + ".wav";
        Destroy(GameObject.Find("Sofa/Scenario/fb").GetComponent<RecordingWav>());
        StartCoroutine(OnClickQ7WaitForThreadIEnumerator(path));
    }
    private IEnumerator OnClickQ7WaitForThreadIEnumerator(string path)
    {
        string vedioToText = null;
        Task t = new Task(() =>
        {
            vedioToText = OnClickQ7ThreadFun(path);
        });
        t.Start();
        yield return 0;
        while (true)
        {
            if (vedioToText != null)
            {
                break;
            }
            yield return 0;
        }
        UITest.SetData(vedioToText);
        string audioText = vedioToText.Remove(0, vedioToText.IndexOf(")") + 1);
        StartCoroutine(AnswersToAudio(audioText));
        yield return 0;
    }

    private string OnClickQ7ThreadFun(object path)
    {
        string reWavPath = (string)path;
        string str = VedioToText(reWavPath);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        return result;
    }

    private IEnumerator OnClickQ7IEnumerator()
    {
        string path = ApplicationData.Current.LocalFolder.Path + "/" + reWav.StopRecording() + ".wav";
        Destroy(GameObject.Find("Sofa/Scenario/fb").GetComponent<RecordingWav>());
        string str = VedioToText(path);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));
        VedioSource.volume = 0.5f;
        yield return 0;
    }

    private static void WriteLog(string log, int name)
    {

        string savePath = ApplicationData.Current.LocalFolder.Path + "/" + name + ".txt";  //本地保存路径

        using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
        {
            var buff = Encoding.Unicode.GetBytes(log);
            fs.Write(buff, 0, buff.Length);
        }
    }

    /// <summary>
    /// 语音转文字
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private String VedioToText(string path)
    {
        String ak_id = "LTAIr8I8A5HNZvdt";
        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";
        String urls = "http://nlsapi.aliyun.com/recognize?";
        //使用对应的ASR模型 详情见文档部分2
        String model = "chat";
        urls = urls + "model=" + model;
        byte[] audioBytes;
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            audioBytes = new byte[fs.Length];
            fs.Read(audioBytes, 0, (int)fs.Length);
        }
        String responseRecognize = HttpUtil.sendAsrPost(audioBytes, "pcm", 16000, urls, ak_id, ak_secret).Result;
        return responseRecognize;
    }

    private string DialogText(string text)
    {
        String ak_id = "LTAIr8I8A5HNZvdt";
        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";

        String urlStr = "https://nlsapi.aliyun.com/manage/qas?action=single:prepub:qa";
        String bdStr = "{\"projectId" + "\"" + ":" + 4649 + "," + "\"question" + "\"" + ":" + "\"" + text + "\"}";
        String responseStr = HttpProxy.sendRequest(urlStr, bdStr, ak_id, ak_secret).Result;
        return responseStr;
    }

    private IEnumerator AnswersToAudio(string content)
    {

        String url = "http://nlsapi.aliyun.com/speak?";
        String ak_id = "LTAIr8I8A5HNZvdt";
        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";

        //设置TTS的参数,详细参数说明详见文档部分2.1 参数配置
        TtsRequest ttsRequest = new TtsRequest();
        ttsRequest.setEncodeType("wav");
        ttsRequest.setVoiceName("xiaoyun");
        ttsRequest.setVolume(50);
        ttsRequest.setSampleRate(16000);
        ttsRequest.setSpeechRate(0);
        ttsRequest.setPitchRate(0);
        ttsRequest.setTtsNus(1);
        ttsRequest.setBackgroundMusicId(0);
        ttsRequest.setBackgroundMusicOffset(0);
        ttsRequest.setBackgroundMusicVolume(0);

        String ttsUrlSpeak = url + "encode_type=" + ttsRequest.getEncodeType()
                 + "&voice_name=" + ttsRequest.getVoiceName()
                 + "&volume=" + ttsRequest.getVolume()
                 + "&sample_rate=" + ttsRequest.getSampleRate()
                 + "&speech_rate=" + ttsRequest.getSpeechRate()
                 + "&pitch_rate=" + ttsRequest.getPitchRate()
                 + "&tts_nus=" + ttsRequest.getTtsNus()
                + "&background_music_id=" + ttsRequest.getBackgroundMusicId()
                + "&background_music_offset=" + ttsRequest.getBackgroundMusicOffset()
                + "&background_music_volume=" + ttsRequest.getBackgroundMusicVolume();


        string fileName = System.Guid.NewGuid().ToString("N");
        string response = HttpUtil.sendTtsPost(content, ttsRequest.getEncodeType(), fileName, ttsUrlSpeak, ak_id, ak_secret).Result;
        if (null != response && 0 < response.Length)
        {
            AudioClip audioClip;
            string fileUrl = "file://" + response;
            WWW www = new WWW(fileUrl);
            yield return www;
            if (www.isDone && www.error == null)
            {
                audioClip = www.GetAudioClip();
                source.clip = audioClip;
                source.Play();
                animations.Play();
                animations["sp_source"].speed = 0.5f;
                yield return new WaitForSeconds(audioClip.length);
                animations.Stop();
                animations["sp_source"].time = 0;
                VedioSource.volume = 0.5f;
                if (false == source.isPlaying)
                {
                    File.Delete(response);
                    Debug.Log("删除成功！");
                }
            }
        }
        yield return 0;
    }
#endif
}

[System.Serializable]
public class ModelTest
{
    public string result;
    public string status;
    public string request_id;
}

[System.Serializable]
public class Model
{
    public string requestId;
    public int resultCode;
    public Datas data;
}

[System.Serializable]
public class Datas
{
    public string id;
    public bool success;
    public Answers[] answers;
}

[System.Serializable]
public class Answers
{
    public string question;
    public string answer;
    public string score;
    public string domain;
    public string optional;
}
