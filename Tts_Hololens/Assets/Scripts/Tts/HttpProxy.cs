#if !NETFX_CORE
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
using Windows.Storage;
using Windows.Storage.Streams;
#endif

namespace TtsInterface
{
    public class HttpProxy
    {
#if !NETFX_CORE
        //计算MD5+BASE64
        public static String MD5Base64(String data)
        {
            if (null == data)
            {
                return null;
            }

            String result = "";
            //string tts接口此处编码必须为utf-8
            byte[] dataUtf8Bytes = Encoding.UTF8.GetBytes(data);
            MD5 md5 = MD5.Create();
            byte[] dataMD5Bytes = md5.ComputeHash(dataUtf8Bytes);
            result = Convert.ToBase64String(dataMD5Bytes);
            return result;
        }

        /*
         * 计算 HMAC-SHA1
         */
        public static String HMACSha1(String data, String key)
        {
            String result = "";
            //string tts接口此处编码必须为GBK
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

        //发送请求
        public static String sendRequest(String url, String body, String ak_id, String ak_secret)
        {
            String result = "";

            Uri realUrl = new Uri(url);

            //http header 参数
            String method = "POST";
            String accept = "application/json";
            String content_type = "application/json";

            DateTime time = DateTime.UtcNow;
            string date = ToGMTString(time);

            // 1.对body做MD5+BASE64加密
            String bodyMd5 = MD5Base64(body);
            String stringToSign = method + "\n" + accept + "\n" + bodyMd5 + "\n" + content_type + "\n" + date;
            // 2.计算 HMAC-SHA1
            String signature = HMACSha1(stringToSign, ak_secret);
            // 3.得到 authorization header
            String authHeader = "Dataplus " + ak_id + ":" + signature;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(realUrl);
            httpWebRequest.Method = method;
            httpWebRequest.Accept = accept;
            httpWebRequest.ContentType = content_type;
            MethodInfo priDateMethod = httpWebRequest.Headers.GetType().GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
            priDateMethod.Invoke(httpWebRequest.Headers, new[] { "Date", date });
            httpWebRequest.Headers.Add("Authorization", authHeader);
            byte[] bt = Encoding.UTF8.GetBytes(body);
            httpWebRequest.ContentLength = bt.Length;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            httpWebRequest.ServicePoint.UseNagleAlgorithm = true;

            httpWebRequest.GetRequestStream().Write(bt, 0, bt.Length);

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

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }

#else
        //计算MD5+BASE64
        public static String MD5Base64(String s)
        {
            if (s == null)
            {
                return null;
            }

            String encodeStr = "";

            //string 编码必须为utf-8
            byte[] utfBytes = Encoding.UTF8.GetBytes(s);

            MD5 md5 = MD5.Create();
            byte[] newBuffer = md5.ComputeHash(utfBytes);
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

        //发送请求
        public static async Task<String> sendRequest(String url, String body, String ak_id, String ak_secret)
        {
            String result = "";

            Uri realUrl = new Uri(url);

            //http header 参数
            String method = "POST";
            String accept = "application/json";
            String content_type = "application/json";

            DateTime time = DateTime.UtcNow;
            string date = ToGMTString(time);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //注册GBK编码
            Encoding encodingGbk = Encoding.UTF8;

            // 1.对body做MD5+BASE64加密
            String bodyMd5 = MD5Base64(body);
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
                    client.DefaultRequestHeaders.Add("ContentLength", encodingGbk.GetBytes(body).Length.ToString());
                    client.DefaultRequestHeaders.Add("Authorization", authHeader);
                    client.DefaultRequestHeaders.Date = time;

                    using (Stream sr = new MemoryStream())
                    {
                        HttpContent httpContent = new StreamContent(sr);
                        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                        httpContent.Headers.ContentLength = encodingGbk.GetBytes(body).Length;

                        sr.Write(encodingGbk.GetBytes(body), 0, encodingGbk.GetBytes(body).Length);
                        await httpContent.CopyToAsync(sr);
                        sr.Seek(0, SeekOrigin.Begin);
                        await sr.FlushAsync();

                        var httpResponseMessage =  client.PostAsync(realUrl, httpContent).Result;
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
#endif
    }
}