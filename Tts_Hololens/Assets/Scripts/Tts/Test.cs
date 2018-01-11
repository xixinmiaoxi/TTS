//using TtsInterface;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using UnityEngine;
//using UnityEngine.UI;


//public class Test : MonoBehaviour
//{
//    string path;
//    GameObject Q1;
//    GameObject Q2;
//    GameObject Q3;
//    GameObject Q4;
//    GameObject Q5;
//    AudioClip audioClip1;
//    AudioClip audioClip2;
//    AudioClip audioClip3;
//    AudioClip audioClip4;
//    AudioClip audioClip5;

//    AudioSource source;
//    bool flag;


//    // Use this for initialization
//    void Start()
//    {
//        Q1 = GameObject.Find("Canvas/Q1");
//        Q2 = GameObject.Find("Canvas/Q2");
//        Q3 = GameObject.Find("Canvas/Q3");
//        Q4 = GameObject.Find("Canvas/Q4");
//        Q5 = GameObject.Find("Canvas/Q5");

//        Q1.GetComponent<Button>().onClick.AddListener(OnClickQ1);
//        Q2.GetComponent<Button>().onClick.AddListener(OnClickQ2);
//        Q3.GetComponent<Button>().onClick.AddListener(OnClickQ3);
//        Q4.GetComponent<Button>().onClick.AddListener(OnClickQ4);
//        Q5.GetComponent<Button>().onClick.AddListener(OnClickQ5);

//        audioClip1 = (AudioClip)Resources.Load("1");
//        audioClip2 = (AudioClip)Resources.Load("2");
//        audioClip3 = (AudioClip)Resources.Load("3");
//        audioClip4 = (AudioClip)Resources.Load("4");
//        audioClip5 = (AudioClip)Resources.Load("5");

//        source = GameObject.Find("Sofa/Scenario/fa").GetComponent<AudioSource>();
//        source.loop = false;
//        source.playOnAwake = false;
//    }

//    private void OnClickQ1()
//    {
//        if (true == flag)
//        {
//            return;
//        }
//        flag = true;
//        string path = @"D:/1.wav";
//        string str = VedioToText(path);
//        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
//        string strDialogText = DialogText(obj.result);
//        Model model = JsonUtility.FromJson<Model>(strDialogText);
//        string result = model.data.answers[0].answer;
//        StartCoroutine(AnswersToAudio(result));

//    }

//    private void OnClickQ2()
//    {
//        if (true == flag)
//        {
//            return;
//        }
//        flag = true;
//        string path = @"D:/2.wav";
//        string str = VedioToText(path);
//        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
//        string strDialogText = DialogText(obj.result);
//        Model model = JsonUtility.FromJson<Model>(strDialogText);
//        string result = model.data.answers[0].answer;
//        StartCoroutine(AnswersToAudio(result));

//    }

//    private void OnClickQ3()
//    {
//        if (true == flag)
//        {
//            return;
//        }
//        flag = true;
//        string path = @"D:/3.wav";
//        string str = VedioToText(path);
//        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
//        string strDialogText = DialogText(obj.result);
//        Model model = JsonUtility.FromJson<Model>(strDialogText);
//        string result = model.data.answers[0].answer;
//        StartCoroutine(AnswersToAudio(result));

//    }

//    private void OnClickQ4()
//    {
//        if (true == flag)
//        {
//            return;
//        }
//        flag = true;
//        string path = @"D:/4.wav";
//        string str = VedioToText(path);
//        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
//        string strDialogText = DialogText(obj.result);
//        Model model = JsonUtility.FromJson<Model>(strDialogText);
//        string result = model.data.answers[0].answer;
//        StartCoroutine(AnswersToAudio(result));
//    }


//    private void OnClickQ5()
//    {
//        if (true == flag)
//        {
//            return;
//        }
//        flag = true;
//        string path = @"D:/5.wav";
//        string str = VedioToText(path);
//        Debug.Log(str);
//        ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
//        string strDialogText = DialogText(obj.result);
//        Debug.Log(strDialogText);
//        Model model = JsonUtility.FromJson<Model>(strDialogText);
//        string result = model.data.answers[0].answer;
//        StartCoroutine(AnswersToAudio(result));

//    }

//    /// <summary>
//    /// 语音转文字
//    /// </summary>
//    /// <param name="path"></param>
//    /// <returns></returns>
//    private String VedioToText(string path)
//    {
//        String ak_id = "LTAIr8I8A5HNZvdt";
//        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";
//        String urls = "http://nlsapi.aliyun.com/recognize?";
//        //使用对应的ASR模型 详情见文档部分2
//        String model = "chat";
//        urls = urls + "model=" + model;
//        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
//        byte[] audioBytes = new byte[fs.Length];
//        fs.Read(audioBytes, 0, (int)fs.Length);

//        HttpResponse responseRecognize = HttpUtil.sendAsrPost(audioBytes, "pcm", 16000, urls, ak_id, ak_secret);

//        fs.Close();
//        return responseRecognize.getResult();
//    }

//    private string DialogText(string text)
//    {
//        String ak_id = "LTAIr8I8A5HNZvdt";
//        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";

//        String urlStr = "https://nlsapi.aliyun.com/manage/qas?action=single:prepub:qa";
//        String bdStr = "{\"projectId" + "\"" + ":" + 4649 + "," + "\"question" + "\"" + ":" + "\"" + text + "\"}";

//        String responseStr = HttpProxy.sendRequest(urlStr, bdStr, ak_id, ak_secret);

//        return responseStr;
//    }

//    private IEnumerator AnswersToAudio(string content)
//    {
//        String url = "http://nlsapi.aliyun.com/speak?";
//        String ak_id = "LTAIr8I8A5HNZvdt";
//        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";

//        //设置TTS的参数,详细参数说明详见文档部分2.1 参数配置
//        TtsRequest ttsRequest = new TtsRequest();
//        ttsRequest.setEncodeType("wav");
//        ttsRequest.setVoiceName("xiaoyun");
//        ttsRequest.setVolume(50);
//        ttsRequest.setSampleRate(16000);
//        ttsRequest.setSpeechRate(0);
//        ttsRequest.setPitchRate(0);
//        ttsRequest.setTtsNus(1);
//        ttsRequest.setBackgroundMusicId(0);
//        ttsRequest.setBackgroundMusicOffset(0);
//        ttsRequest.setBackgroundMusicVolume(0);

//        String ttsUrlSpeak = url + "encode_type=" + ttsRequest.getEncodeType()
//                 + "&voice_name=" + ttsRequest.getVoiceName()
//                 + "&volume=" + ttsRequest.getVolume()
//                 + "&sample_rate=" + ttsRequest.getSampleRate()
//                 + "&speech_rate=" + ttsRequest.getSpeechRate()
//                 + "&pitch_rate=" + ttsRequest.getPitchRate()
//                 + "&tts_nus=" + ttsRequest.getTtsNus()
//                + "&background_music_id=" + ttsRequest.getBackgroundMusicId()
//                + "&background_music_offset=" + ttsRequest.getBackgroundMusicOffset()
//                + "&background_music_volume=" + ttsRequest.getBackgroundMusicVolume();


//        string fileName = System.Guid.NewGuid().ToString("N");
//        HttpResponse response = HttpUtil.sendTtsPost(content, ttsRequest.getEncodeType(), fileName, ttsUrlSpeak, ak_id, ak_secret);

//        AudioClip audioClip;
//        string fileUrl = "file://" + response.getResult();
//        Debug.Log(fileUrl);
//        WWW www = new WWW(fileUrl);
//        yield return www;
//        if (www.isDone && www.error == null)
//        {
//            audioClip = www.GetAudioClip();
//            source.clip = audioClip;
//            source.Play();
//            if (false == source.isPlaying)
//            {
//                File.Delete(response.getResult());
//                Debug.Log("删除成功！");
//            }
//        }
//        flag = false;
//        yield return 0;
//    }

//    [System.Serializable]
//    public class ModelTest
//    {
//        public string result;
//        public string status;
//        public string request_id;
//    }

//    [System.Serializable]
//    public class Model
//    {
//        public string requestId;
//        public int resultCode;
//        public Datas data;
//    }

//    [System.Serializable]
//    public class Datas
//    {
//        public string id;
//        public bool success;
//        public Answers[] answers;
//    }

//    [System.Serializable]
//    public class Answers
//    {
//        public string question;
//        public string answer;
//        public string score;
//        public string domain;
//        public string optional;
//    }
//}


////class Program : MonoBehaviour
////{
////    void Start()
////    {
////        #region  tts  文字--》语音
////        String url = "http://nlsapi.aliyun.com/speak?";
////        //String tts_text = "薄雾浓云愁永昼。瑞脑消金兽。佳节又重阳，玉枕纱厨，半夜凉初透。东篱把酒黄昏后。有暗香盈袖。莫道不消魂，帘卷西风，人比黄花瘦。";
////        String tts_text = "32号谁啊，我怎么没见过？";
////        //请使用https://ak-console.aliyun.com/ 页面获取的Access 信息
////        //请提前开通智能语音服务(https://data.aliyun.com/product/nls)
////        String ak_id = "LTAIr8I8A5HNZvdt";
////        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";
////        //String ak_id = "LTAILNHYi9oS9tmi";
////        //String ak_secret = "hZH1gI8FYIeOPc7rFxtQl8geg7aaGW";

////        //设置TTS的参数,详细参数说明详见文档部分2.1 参数配置
////        Program ttsDemo = new Program();
////        TtsRequest ttsRequest = new TtsRequest();
////        ttsRequest.setEncodeType("wav");
////        ttsRequest.setVoiceName("xiaoyun");
////        ttsRequest.setVolume(50);
////        ttsRequest.setSampleRate(16000);
////        ttsRequest.setSpeechRate(0);
////        ttsRequest.setPitchRate(0);
////        ttsRequest.setTtsNus(1);
////        ttsRequest.setBackgroundMusicId(0);
////        ttsRequest.setBackgroundMusicOffset(0);
////        ttsRequest.setBackgroundMusicVolume(0);

////        String ttsUrlSpeak = url + "encode_type=" + ttsRequest.getEncodeType()
////                 + "&voice_name=" + ttsRequest.getVoiceName()
////                 + "&volume=" + ttsRequest.getVolume()
////                 + "&sample_rate=" + ttsRequest.getSampleRate()
////                 + "&speech_rate=" + ttsRequest.getSpeechRate()
////                 + "&pitch_rate=" + ttsRequest.getPitchRate()
////                 + "&tts_nus=" + ttsRequest.getTtsNus()
////                + "&background_music_id=" + ttsRequest.getBackgroundMusicId()
////                + "&background_music_offset=" + ttsRequest.getBackgroundMusicOffset()
////                + "&background_music_volume=" + ttsRequest.getBackgroundMusicVolume();


////        string fileName = System.Guid.NewGuid().ToString("N");
////        //HttpResponse response = HttpUtil.sendTtsPost(tts_text, ttsRequest.getEncodeType(), fileName, ttsUrlSpeak, ak_id, ak_secret);
////        #endregion

////        #region tts 语音--》文字
////        //String urls = "http://nlsapi.aliyun.com/recognize?";
////        ////使用对应的ASR模型 详情见文档部分2
////        //String model = "chat";
////        //urls = urls + "model=" + model;
////        //FileStream fs = new FileStream(@"D:\1.wav", FileMode.Open, FileAccess.Read);
////        //byte[] audioBytes = new byte[fs.Length];
////        //fs.Read(audioBytes, 0, (int)fs.Length);

////        //HttpResponse responseRecognize = HttpUtil.sendAsrPost(audioBytes, "pcm", 16000, urls, ak_id, ak_secret);

////        //fs.Close();
////        #endregion


////        ////智能对话
////        //String urlStr = "https://nlsapi.aliyun.com/manage/qas?action=single:prepub:qa";
////        ////String urlStr = "https://nlsapi.aliyun.com/manage/qas?action=projects:list";
////        //String urlStrTest = "https://nlsapi.aliyun.com/manage/qas";

////        //String bdStr = "{\"projectId" + "\"" + ":" + 4649 + "," + "\"question" + "\"" + ":" + "\"" + "请问勒布朗詹姆斯最近的技术统计怎么样？" + "\"}";

////        ////String bdStr = "{\"offset" + "\"" + ":" + 0+ "," + "\"pageSize" + "\"" + ":" + 1 +"}";

////        //String bdStrTest = "{\"version" + "\"" + ":" + "\"" + "2.0" + "\"" + "," + "\"app_key" + "\"" + ":" + "\"" + "nui-d9lLBVpmR0dl" + "\"" + "," + "\"question" + "\"" + ":" + "\"" + "请问勒布朗詹姆斯最近的技术统计怎么样？" + "\"}";

////        //String responseStrTest = HttpProxy.sendRequest(urlStrTest, bdStrTest, ak_id, ak_secret);
////        //Debug.Log(responseStrTest);

////        //String responseStr = HttpProxy.sendRequest(urlStr, bdStr, ak_id, ak_secret);
////        // Debug.Log(responseStr);

////        //语音采集
////        //SoundRecord recorder = new SoundRecord();
////        ////开始录音
////        //string wavfile = null;
////        //wavfile = "test.wav";
////        //recorder.SetFileName(wavfile);
////        //recorder.RecStart();
////        ////结束
////        //recorder.RecStop();
////        //recorder = null;

////    }

////}
