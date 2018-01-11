#if NETFX_CORE
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
#else 
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using TtsInterface;
using UnityEngine;
using UnityEngine.UI;
#endif

#if NETFX_CORE
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
        string TextStr;
        TextStr = IEPost(tts_text, ttsRequest.getEncodeType(), fileName, ttsUrlSpeak, ak_id, ak_secret).Result;
        GameObject text = GameObject.Find("Canvas/Text");
        text.GetComponent<Text>().text = TextStr;
    }

    private async Task<String> IEPost(String textData, String audioType, String audioName, String url, String ak_id, String ak_secret)
    {
        String method = "POST";
        String content_type = "text/plain";
        String accept = "audio/" + audioType;// + ",application/json";
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

        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.Method = method;
        httpWebRequest.Accept = accept;
        httpWebRequest.ContentType = content_type;
        httpWebRequest.Headers["Date"] = date;
        httpWebRequest.Headers["Authorization"] = authHeader;
        httpWebRequest.Headers["ContentLength"] = Encoding.UTF8.GetBytes(textData).Length.ToString();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //注册GBK编码
        Encoding encodingUnicode = Encoding.Unicode;

        //var stream = await httpWebRequest.GetRequestStreamAsync();
        //await stream.WriteAsync(Encoding.UTF8.GetBytes(textData), 0, Encoding.UTF8.GetBytes(textData).Length);
        //await stream.FlushAsync();

        //var httpWeb = await httpWebRequest.GetResponseAsync();
        //HttpWebResponse httpWebResponse = (HttpWebResponse)httpWeb;


        await httpWebRequest.GetRequestStreamAsync().Result.WriteAsync(Encoding.UTF8.GetBytes(textData), 0, Encoding.UTF8.GetBytes(textData).Length);

        HttpWebResponse httpWebResponse = (HttpWebResponse)(httpWebRequest.GetResponseAsync().Result);

        if (HttpStatusCode.OK == httpWebResponse.StatusCode)
        {
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.Unicode);

            var responseContent = streamReader.ReadToEndAsync().Result;

            //string savePath = ApplicationData.Current.LocalFolder.Path + "/" + audioName + "." + audioType;  //本地保存路径

            return responseContent;

            //byte[] bytes = encodingUnicode.GetBytes(responseContent);

            //using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
            //{
            //    var buff = Encoding.Unicode.GetBytes(responseContent);
            //    fs.Write(buff, 0, buff.Length);
            //}
        }
        return null;
    }


    public static String ToGMTString(DateTime date)
    {
        string dateString = date.ToString("r");
        return dateString;
    }

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
        //使用CodePagesEncodingProvider去注册扩展编码。
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //注册GBK编码
        Encoding encodingGbk = Encoding.GetEncoding("GBK");

        byte[] dataByte = encodingGbk.GetBytes(data);
        byte[] keyByte = encodingGbk.GetBytes(key);


        byte[] DataBt = encodingGbk.GetBytes(data);
        byte[] KeyBt = encodingGbk.GetBytes(key);
        data = Encoding.UTF8.GetString(DataBt);
        key = Encoding.UTF8.GetString(KeyBt);

        string SHA1Name = MacAlgorithmNames.HmacSha1;
        IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);
        IBuffer buffKeyMaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
        MacAlgorithmProvider objMacProv = MacAlgorithmProvider.OpenAlgorithm(SHA1Name);
        CryptographicKey hmacKey = objMacProv.CreateKey(buffKeyMaterial);
        IBuffer buffHMAC = CryptographicEngine.Sign(hmacKey, buffUtf8Msg);
        byte[] hashValue = Buffer2Bytes(buffHMAC);
        result = Convert.ToBase64String(hashValue);
        return result;
    }

    public static IBuffer Bytes2Buffer(byte[] bytes)
    {
        using (var dataWriter = new DataWriter())
        {
            dataWriter.WriteBytes(bytes);
            return dataWriter.DetachBuffer();
        }
    }
    public static byte[] Buffer2Bytes(IBuffer buffer)
    {
        using (var dataReader = DataReader.FromBuffer(buffer))
        {
            var bytes = new byte[buffer.Length];
            dataReader.ReadBytes(bytes);
            return bytes;
        }
    }
}
#else
public class Testdemo : MonoBehaviour
{


}
#endif