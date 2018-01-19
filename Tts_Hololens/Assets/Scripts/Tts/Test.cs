#if !NETFX_CORE 
using TtsInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
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
    GameObject Q1;
    GameObject Q2;
    GameObject Q3;
    GameObject Q4;
    GameObject Q5;
    GameObject Q6;
    GameObject Q7;

    RecordingWav reWav;

    AudioSource source;
    bool flag;

    // Use this for initialization
    void Start()
    {
        Q1 = GameObject.Find("Canvas/Q1");
        Q2 = GameObject.Find("Canvas/Q2");
        Q3 = GameObject.Find("Canvas/Q3");
        Q4 = GameObject.Find("Canvas/Q4");
        Q5 = GameObject.Find("Canvas/Q5");
        Q6 = GameObject.Find("Canvas/Q6");
        Q7 = GameObject.Find("Canvas/Q7");

        reWav = GameObject.Find("Sofa/Scenario/fa").GetComponent<RecordingWav>();

        Q1.GetComponent<Button>().onClick.AddListener(OnClickQ1);
        Q2.GetComponent<Button>().onClick.AddListener(OnClickQ2);
        Q3.GetComponent<Button>().onClick.AddListener(OnClickQ3);
        Q4.GetComponent<Button>().onClick.AddListener(OnClickQ4);
        Q5.GetComponent<Button>().onClick.AddListener(OnClickQ5);
        Q6.GetComponent<Button>().onClick.AddListener(OnClickQ6);
        Q7.GetComponent<Button>().onClick.AddListener(OnClickQ7);

        source = GameObject.Find("Sofa/Scenario/fa").GetComponent<AudioSource>();
        source.loop = false;
        source.playOnAwake = false;
    }

#if !NETFX_CORE

    private void OnClickQ1()
    {
        if (true == flag)
        {
            return;
        }
        flag = true;
        string path = Application.streamingAssetsPath + "/1.wav";
        string str = VedioToText(path);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));
    }

    private void OnClickQ2()
    {
        if (true == flag)
        {
            return;
        }
        flag = true;
        string path = Application.streamingAssetsPath + "/2.wav";
        string str = VedioToText(path);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));

    }

    private void OnClickQ3()
    {
        if (true == flag)
        {
            return;
        }
        flag = true;
        string path = Application.streamingAssetsPath + "/3.wav";
        string str = VedioToText(path);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));

    }

    private void OnClickQ4()
    {
        if (true == flag)
        {
            return;
        }
        flag = true;
        string path = Application.streamingAssetsPath + "/4.wav";
        string str = VedioToText(path);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));
    }


    private void OnClickQ5()
    {
        if (true == flag)
        {
            return;
        }
        flag = true;
        string path = Application.streamingAssetsPath + "/5.wav";
        string str = VedioToText(path);
        Debug.Log(str);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Debug.Log(strDialogText);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));

    }

    private void OnClickQ6()
    {
        reWav.StartRecording();
    }
    private void OnClickQ7()
    {
        string path = Application.persistentDataPath + "/" + reWav.StopRecording() + ".wav";
        if (true == flag)
        {
            return;
        }
        flag = true;
        string str = VedioToText(path);
        Debug.Log(str);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Debug.Log(strDialogText);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));
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
                if (false == source.isPlaying)
                {
                    File.Delete(response);
                    Debug.Log("删除成功！");
                }
            }
            flag = false;
        }
        yield return 0;
    }
#else
    private void OnClickQ1()
    {
        if (true == flag)
        {
            return;
        }
        flag = true;
        WriteLog("Click", 1);
        string path = Application.streamingAssetsPath + "/1.wav";
        WriteLog("VedioToText", 2);
        string str = VedioToText(path);
        WriteLog("json", 3);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        WriteLog("DialogText", 4);
        string strDialogText = DialogText(obj.result);
        WriteLog("FromJson", 5);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        WriteLog("StartCoroutine", 6);
        StartCoroutine(AnswersToAudio(result));
        WriteLog("End", 7);
    }

    private void OnClickQ2()
    {
        if (true == flag)
        {
            return;
        }
        flag = true;
        string path = Application.streamingAssetsPath + "/2.wav";
        string str = VedioToText(path);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));

    }

    private void OnClickQ3()
    {
        if (true == flag)
        {
            return;
        }
        flag = true;
        string path = Application.streamingAssetsPath + "/3.wav";
        string str = VedioToText(path);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));

    }

    private void OnClickQ4()
    {
        if (true == flag)
        {
            return;
        }
        flag = true;
        string path = Application.streamingAssetsPath + "/4.wav";
        string str = VedioToText(path);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));
    }


    private void OnClickQ5()
    {
        if (true == flag)
        {
            return;
        }
        flag = true;
        string path = Application.streamingAssetsPath + "/5.wav";
        string str = VedioToText(path);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));

    }

    private void OnClickQ6()
    {
        reWav.StartRecording();
    }
    private void OnClickQ7()
    {
        string path = ApplicationData.Current.LocalFolder.Path + "/" + reWav.StopRecording() + ".wav";
        if (true == flag)
        {
            return;
        }
        flag = true;
        string str = VedioToText(path);
        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
        string strDialogText = DialogText(obj.result);
        Model model = JsonUtility.FromJson<Model>(strDialogText);
        string result = model.data.answers[0].answer;
        StartCoroutine(AnswersToAudio(result));
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
        WriteLog(responseRecognize, 8);
        return responseRecognize;
    }

    private string DialogText(string text)
    {
        String ak_id = "LTAIr8I8A5HNZvdt";
        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";

        String urlStr = "https://nlsapi.aliyun.com/manage/qas?action=single:prepub:qa";
        String bdStr = "{\"projectId" + "\"" + ":" + 4649 + "," + "\"question" + "\"" + ":" + "\"" + text + "\"}";
        WriteLog(text, 9);
        String responseStr = HttpProxy.sendRequest(urlStr, bdStr, ak_id, ak_secret).Result;
        WriteLog(responseStr, 10);
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
        WriteLog(content, 11);
        string response = HttpUtil.sendTtsPost(content, ttsRequest.getEncodeType(), fileName, ttsUrlSpeak, ak_id, ak_secret).Result;
        WriteLog(response, 12);
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
                //if (false == source.isPlaying)
                //{
                //    File.Delete(response);
                //    Debug.Log("删除成功！");
                //}
            }
            flag = false;
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
