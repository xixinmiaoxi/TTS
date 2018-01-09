using ConsoleApp1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Testdemo : MonoBehaviour
{
    void Start()
    {
        String url = "http://nlsapi.aliyun.com/speak?";
        String ak_id = "LTAIr8I8A5HNZvdt";
        String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";
        String tts_text = "薄雾浓云愁永昼。瑞脑消金兽。佳节又重阳，玉枕纱厨，半夜凉初透。东篱把酒黄昏后。有暗香盈袖。莫道不消魂，帘卷西风，人比黄花瘦。";

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

        StartCoroutine(IEPost(tts_text, ttsRequest.getEncodeType(), fileName, ttsUrlSpeak, ak_id, ak_secret));
    }

    public static String ToGMTString(DateTime date)
    {
        string dateString = date.ToString("r");
        return dateString;
    }

    IEnumerator IEPost(String textData, String audioType, String audioName, String url, String ak_id, String ak_secret)
    {
        //HttpResponse response = HttpUtil.sendTtsPost(content, ttsRequest.getEncodeType(), fileName, ttsUrlSpeak, ak_id, ak_secret);

        String method = "POST";
        String content_type = "text/plain";
        String accept = "audio/" + audioType + ",application/json";
        int length = textData.Length;

        DateTime time = DateTime.UtcNow;
        string date = ToGMTString(time);

        // 1.对body做MD5+BASE64加密
        String bodyMd5 = MD5Base64(Encoding.UTF8.GetBytes(textData));
        String stringToSign = method + "\n" + accept + "\n" + bodyMd5 + "\n" + content_type + "\n" + date;
        // 2.计算 HMAC-SHA1
        String signature = HMACSha1(stringToSign, ak_secret);
        // 3.得到 authorization header
        String authHeader = "Dataplus " + ak_id + ":" + signature;


        // Dictionary<string, string> AuthDic = new Dictionary<string, string>();  // auth header
        // AuthDic.Add("Content-Type", content_type);
        // AuthDic.Add("Authorization", authHeader);
        // AuthDic.Add("Method", method);
        // //AuthDic["Date"] = date;
        // DateTime dateTime = DateTime.Now;
        // string times = dateTime.ToString("ddd, yyyy-mm-dd HH':'mm':'ss 'UTC'", DateTimeFormatInfo.InvariantInfo);
        // AuthDic["Date"] = times;
        // AuthDic.Add("ContentLength", Encoding.UTF8.GetBytes(textData).Length.ToString());
        //// url = hmac(url, path, time);

        //转换为字节
        byte[] post_data;
        post_data = System.Text.UTF8Encoding.UTF8.GetBytes(textData);

        // WWW www = new WWW(url, post_data, AuthDic);

        // yield return www;

        // if (www.error != null)
        // {
        //     Debug.LogError("error:" + www.error);
        // }
        // Debug.Log(www.text);



        //var request = new UnityEngine.Networking.UnityWebRequest(url, "POST");

        //request.uploadHandler = (UploadHandler)new UploadHandlerRaw(post_data);
        //request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        //request.SetRequestHeader("Content-Type", content_type);
        //request.SetRequestHeader("Authorization", authHeader);
        //request.SetRequestHeader("Method", method);
        //request.SetRequestHeader("Date", date);
        //request.SetRequestHeader("ContentLength", Encoding.UTF8.GetBytes(textData).Length.ToString());
        //MethodInfo priLengthMethod = request.GetType().GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
        //priLengthMethod.Invoke(request, new[] { "Date", Encoding.UTF8.GetBytes(textData).Length.ToString() });

        //yield return request.Send();

        //Debug.Log("Status Code: " + request.responseCode);
        //if (request.responseCode == 200)
        //{
        //    string text = request.downloadHandler.text;
        //    Debug.Log(text);
        //}


        HTTP.Request r = new HTTP.Request("POST", url, post_data);

        // Header部分  
        r.headers.Clear();
        r.headers.Add("Content-Type", content_type);
        r.headers.Add("Authorization", authHeader);
        r.headers.Add("Method", method);
        r.headers.Add("Date", date);
        r.headers.Add("ContentLength", Encoding.UTF8.GetBytes(textData).Length.ToString());
        r.headers.Add("Accept", accept);

        // 超时时间设定  
        r.timeout = 60;

        yield return r.Send();

        // 报错  
        if (r.exception != null)
        {
            Debug.Log("post request error: " + r.exception.ToString());

            // 超时  
            if (r.exception is System.TimeoutException)
            {
                Debug.Log("Request timed out.");
            }
            // 其他报错  
            else
            {
                Debug.Log("Exception occured in request.");
            }
        }
        else if (r.response.status != 200)
        {
            Debug.Log("post request code:" + r.response.status);
        }
        // 成功  
        else
        {
            Stream stream = new MemoryStream();
            byte[] bytes = r.response.Bytes;
            

            Debug.Log("post Success!!");
            Debug.Log("returned data:" + r.response.Text);
        }
        // 完成通信  
        Debug.Log("WWW Done. " + url);
    }

    //public string hmac(string url, string path, string time)
    //{
    //    StringBuilder stringToSign = new StringBuilder();
    //    stringToSign.Append("POST").Append("\n");
    //    stringToSign.Append(time).Append("\n");
    //    stringToSign.Append(path).Append("\n");

    //    var signature = string.Empty;
    //    using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(SECURE_KEY)))
    //    {
    //        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign.ToString()));
    //        signature = Convert.ToBase64String(hash);
    //    }

    //    StringBuilder s = new StringBuilder();
    //    s.Append(url);
    //    s.Append("&apiKey=");
    //    s.Append(HttpUtility.UrlEncode(API_KEY, Encoding.UTF8));
    //    s.Append("&signature=");
    //    s.Append(HttpUtility.UrlEncode(HttpUtility.UrlEncode(signature, Encoding.UTF8), Encoding.UTF8));

    //    return s.ToString();
    //}



    public static String MD5Base64(byte[] s)
    {
        if (s == null)
        {
            return null;
        }
        String encodeStr = "";

        MD5 md5 = MD5.Create();
        byte[] newBuffer = md5.ComputeHash(s);
        encodeStr = Convert.ToBase64String(newBuffer); //这个要注意，不要在newbuffer就转，你解密的时候会乱码（有时候）
        return encodeStr;
    }

    /*
     * 计算 HMAC-SHA1
     */
    public static String HMACSha1(String data, String key)
    {
        String result = "";
        byte[] dataByte = Encoding.GetEncoding("GBK").GetBytes(data);
        byte[] keyByte = Encoding.GetEncoding("GBK").GetBytes(key);


        using (HMACSHA1 hmac = new HMACSHA1(keyByte))
        {
            MemoryStream stream = new MemoryStream(dataByte);
            byte[] hashValue = hmac.ComputeHash(stream);

            result = Convert.ToBase64String(hashValue);
        }
        return result;
    }
}
