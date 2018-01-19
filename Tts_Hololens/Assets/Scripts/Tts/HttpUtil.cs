#if !NETFX_CORE 
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
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
using System.Reflection;
#endif

namespace TtsInterface
{
    public class HttpUtil
    {
#if !NETFX_CORE
        /*
         * 计算MD5+BASE64
         */
        public static String MD5Base64(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            String result = "";

            MD5 md5 = MD5.Create();
            byte[] dataMD5Bytes = md5.ComputeHash(data);
            result = Convert.ToBase64String(dataMD5Bytes);
            return result;
        }

        /*
         * 计算 HMAC-SHA1
         */
        public static String HMACSha1(String data, String key)
        {
            String result = "";
            byte[] dataBytes = Encoding.GetEncoding("GBK").GetBytes(data);
            byte[] keyBytes = Encoding.GetEncoding("GBK").GetBytes(key);

            using (HMACSHA1 hmac = new HMACSHA1(keyBytes))
            {
                using (MemoryStream dataStream = new MemoryStream(dataBytes))
                {
                    byte[] hashValue = hmac.ComputeHash(dataStream);
                    result = Convert.ToBase64String(hashValue);
                }
            }
            return result;
        }

        /*
         * 等同于javaScript中的 new Date().toUTCString();
         */
        public static String ToGMTString(DateTime date)
        {
            string dateString = date.ToString("r");
            return dateString;
        }

        /*
           * 发送POST请求
           */
        public static String sendAsrPost(byte[] audioData, String audioFormat, int sampleRate, String url, String ak_id, String ak_secret)
        {
            String result = "";
            Uri realUrl = new Uri(url);
            /*
             * http header 参数
             */
            String method = "POST";
            String accept = "application/json";
            String content_type = "audio/" + audioFormat + ";samplerate=" + sampleRate;
            int length = audioData.Length;

            DateTime time = DateTime.UtcNow;
            String date = ToGMTString(time);
            // 1.对body做MD5+BASE64加密
            String bodyMd5 = MD5Base64(audioData);
            String md52 = MD5Base64(Encoding.GetEncoding("GBK").GetBytes(bodyMd5));
            String stringToSign = method + "\n" + accept + "\n" + md52 + "\n" + content_type + "\n" + date;
            // 2.计算 HMAC-SHA1
            String signature = HMACSha1(stringToSign, ak_secret);
            // 3.得到 authorization header
            String authHeader = "Dataplus " + ak_id + ":" + signature;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(realUrl);
            httpWebRequest.Method = method;
            httpWebRequest.Accept = accept;
            httpWebRequest.ContentType = content_type;
            MethodInfo priDateMethod = httpWebRequest.Headers.GetType().GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
            priDateMethod.Invoke(httpWebRequest.Headers, new[] { "Date", date });
            httpWebRequest.Headers.Add("Authorization", authHeader);
            httpWebRequest.ContentLength = audioData.Length;

            httpWebRequest.GetRequestStream().Write(audioData, 0, audioData.Length);

            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                if (HttpStatusCode.OK == httpWebResponse.StatusCode)
                {
                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
            }
            return result;
        }

        public static String sendTtsPost(String textData, String audioType, String audioName, String url, String ak_id, String ak_secret)
        {
            String result = "";

            Uri realUrl = new Uri(url);
            /*
             * http header 参数
             */
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

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(realUrl);
            httpWebRequest.Method = method;
            httpWebRequest.Accept = accept;
            httpWebRequest.ContentType = content_type;
            MethodInfo priDateMethod = httpWebRequest.Headers.GetType().GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
            priDateMethod.Invoke(httpWebRequest.Headers, new[] { "Date", date });
            httpWebRequest.Headers.Add("Authorization", authHeader);
            httpWebRequest.ContentLength = Encoding.UTF8.GetBytes(textData).Length;

            httpWebRequest.GetRequestStream().Write(Encoding.UTF8.GetBytes(textData), 0, Encoding.UTF8.GetBytes(textData).Length);

            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                if (HttpStatusCode.OK == httpWebResponse.StatusCode)
                {
                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.Unicode))
                    {
                        string response = streamReader.ReadToEnd();
                        if (null != response && 0 < response.Length)
                        {
                            result = Application.streamingAssetsPath + "/" + audioName + "." + audioType;    //本地保存路径
                            using (FileStream fs = new FileStream(result, FileMode.Append))
                            {
                                var buff = Encoding.Unicode.GetBytes(response);
                                fs.Write(buff, 0, buff.Length);
                            }
                        }
                    }
                }
            }
            return result;
        }
#else

        /*
          * 计算MD5+BASE64
          */
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

        /*
         * 等同于javaScript中的 new Date().toUTCString();
         */
        public static String ToGMTString(DateTime date)
        {
            string dateString = date.ToString("r");
            return dateString;
        }

        /*
         * 发送POST请求
         */
        public static async Task<string> sendAsrPost(byte[] audioData, String audioFormat, int sampleRate, String url, String ak_id, String ak_secret)
        {
            String result = "";
            Uri realUrl = new Uri(url);
            /*
             * http header 参数
             */
            String method = "POST";
            String accept = "application/json";
            String content_type = "audio/" + audioFormat;// + ";samplerate=" + sampleRate;
            int length = audioData.Length;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //注册GBK编码
            Encoding encodingGbk = Encoding.GetEncoding("GBK");

            DateTime time = DateTime.UtcNow;
            String date = ToGMTString(time);
            // 1.对body做MD5+BASE64加密
            String bodyMd5 = MD5Base64(audioData);
            String md52 = MD5Base64(encodingGbk.GetBytes(bodyMd5));
            String stringToSign = method + "\n" + accept + "\n" + md52 + "\n" + content_type + "\n" + date;
            // 2.计算 HMAC-SHA1
            String signature = HMACSha1(stringToSign, ak_secret);
            // 3.得到 authorization header
            String authHeader = "Dataplus " + ak_id + ":" + signature;

            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("Method", method);
                    client.DefaultRequestHeaders.Add("Accept", accept);
                    client.DefaultRequestHeaders.Add("ContentType", content_type);
                    client.DefaultRequestHeaders.Add("ContentLength", audioData.Length.ToString());
                    client.DefaultRequestHeaders.Add("Authorization", authHeader);
                    client.DefaultRequestHeaders.Date = time;

                    using (Stream sr = new MemoryStream())
                    {
                        HttpContent httpContent = new StreamContent(sr);
                        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                        httpContent.Headers.ContentLength = audioData.Length;

                        sr.Write(audioData, 0, audioData.Length);
                        await httpContent.CopyToAsync(sr);
                        sr.Seek(0, SeekOrigin.Begin);
                        await sr.FlushAsync();

                        var httpResponseMessage = client.PostAsync(realUrl, httpContent).Result;
                        var contentType = httpResponseMessage.Content.Headers.ContentType.CharSet = "utf-8";
                        if (httpResponseMessage.EnsureSuccessStatusCode().StatusCode.ToString().ToLower() == "ok")
                        {
                            result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                        }
                    }
                    client.Dispose();
                }
            }
            return result;
        }

        /*
         * 发送POST请求
         */
        public static async Task<string> sendTtsPost(String textData, String audioType, String audioName, String url, String ak_id, String ak_secret)
        {
            String result = "";

            Uri realUrl = new Uri(url);
            /*
             * http header 参数
             */
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

            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("Method", method);
                    client.DefaultRequestHeaders.Add("Accept", accept);
                    client.DefaultRequestHeaders.Add("ContentType", content_type);
                    client.DefaultRequestHeaders.Add("ContentLength", Encoding.UTF8.GetBytes(textData).Length.ToString());
                    client.DefaultRequestHeaders.Add("Authorization", authHeader);
                    client.DefaultRequestHeaders.Date = time;

                    using (Stream sr = new MemoryStream())
                    {
                        HttpContent httpContent = new StreamContent(sr);
                        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                        httpContent.Headers.ContentLength = Encoding.UTF8.GetBytes(textData).Length;

                        sr.Write(Encoding.UTF8.GetBytes(textData), 0, Encoding.UTF8.GetBytes(textData).Length);
                        await httpContent.CopyToAsync(sr);
                        sr.Seek(0, SeekOrigin.Begin);
                        await sr.FlushAsync();

                        var httpResponseMessage = client.PostAsync(realUrl, httpContent).Result;
                        var contentType = httpResponseMessage.Content.Headers.ContentType.CharSet = "utf-16";
                        if (httpResponseMessage.EnsureSuccessStatusCode().StatusCode.ToString().ToLower() == "ok")
                        {
                            string responseBody = httpResponseMessage.Content.ReadAsStringAsync().Result;
                            if (null != responseBody && 0 < responseBody.Length)
                            {
                                result = ApplicationData.Current.LocalFolder.Path + "/" + audioName + "." + audioType;    //本地保存路径
                                using (FileStream fs = new FileStream(result, FileMode.Append))
                                {
                                    var buff = Encoding.Unicode.GetBytes(responseBody);
                                    fs.Write(buff, 0, buff.Length);
                                }
                            }
                        }
                    }
                    client.Dispose();
                }
            }
            return result;
        }
#endif
    }
}