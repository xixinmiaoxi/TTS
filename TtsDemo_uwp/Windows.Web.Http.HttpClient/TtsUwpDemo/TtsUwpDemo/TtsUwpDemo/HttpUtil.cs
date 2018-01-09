using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
//using System.Net.Http;
using System.Net.Sockets;
//using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace TtsUwpTest
{
    public class HttpUtil
    {
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


            using (HMACSHA1 hmac = new HMACSHA1(keyByte))
            {
                using (MemoryStream stream = new MemoryStream(dataByte))
                {
                    byte[] hashValue = hmac.ComputeHash(stream);

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
        public static async void sendAsrPost(byte[] audioData, String audioFormat, int sampleRate, String url, String ak_id, String ak_secret)
        {
            String result = "";
            HttpResponse response = new HttpResponse();
            try
            {
                Uri realUrl = new Uri(url);
                /*
                 * http header 参数
                 */
                String method = "POST";
                String accept = "application/json";
                String content_type = "audio/" + audioFormat + ";samplerate=" + sampleRate;
                int length = audioData.Length;

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //注册GBK编码
                Encoding encodingGbk = Encoding.GetEncoding("GBK");

                Encoding encodingUtf8 = Encoding.UTF8;

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
                String authHeaders = ak_id + ":" + signature;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Method", method);
                    client.DefaultRequestHeaders.Add("Accept", accept);
                    client.DefaultRequestHeaders.Add("ContentType", content_type);
                    client.DefaultRequestHeaders.Add("ContentLength", audioData.Length.ToString());
                    client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Dataplus", authHeaders);
                    client.DefaultRequestHeaders.Date = time;

                    byte[] bytes = audioData;
                    InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
                    DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
                    datawriter.WriteBytes(bytes);
                    await datawriter.StoreAsync();
                    var httpContent = new HttpStreamContent(memoryStream);

                    httpContent.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue(content_type);
                    httpContent.Headers.ContentLength = (ulong)audioData.Length;


                    var httpResponseMessage = await client.PostAsync(realUrl, httpContent);

                    if (httpResponseMessage.EnsureSuccessStatusCode().StatusCode.ToString().ToLower() == "ok")
                    {
                        var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                        uint a = 1;
                    }

                }
            }
            catch (Exception ex)
            {

            }
            //return response;
        }


        /*
         * 发送POST请求
         */
        public static async void sendTtsPost(String textData, String audioType, String audioName, String url, String ak_id, String ak_secret)
        {
            String result = "";
            HttpResponse response = new HttpResponse();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //注册GBK编码
            Encoding encodingGbk = Encoding.GetEncoding("GBK");

            Encoding encodingUtf8 = Encoding.UTF8;


            Encoding encodingUtf16 = Encoding.Unicode;
            try
            {
                Uri realUrl = new Uri(url);
                /*
                 * http header 参数
                 */
                String method = "POST";
                String content_type = "text/plain";
                // String accept = "audio/" + audioType + ",application/json";
                string accept = "audio/wav";

                DateTime time = DateTime.UtcNow;
                string date = ToGMTString(time);

                // 1.对body做MD5+BASE64加密
                String bodyMd5 = MD5Base64(encodingUtf8.GetBytes(textData));
                String stringToSign = method + "\n" + accept + "\n" + bodyMd5 + "\n" + content_type + "\n" + date;
                // 2.计算 HMAC-SHA1
                String signature = HMACSha1(stringToSign, ak_secret);
                // 3.得到 authorization header
                String authHeader = "Dataplus " + ak_id + ":" + signature;
                String authHeaders = ak_id + ":" + signature;

                var baseFilter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
                baseFilter.AllowAutoRedirect = true;
                baseFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.NoCache;
                using (var client = new HttpClient(baseFilter))
                {
                    client.DefaultRequestHeaders.Add("Method", method);
                    client.DefaultRequestHeaders.Add("Accept", accept);
                    client.DefaultRequestHeaders.Add("ContentType", content_type);
                    client.DefaultRequestHeaders.Add("ContentLength", encodingUtf8.GetBytes(textData).Length.ToString());
                    client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Dataplus", authHeaders);
                    client.DefaultRequestHeaders.Date = time;


                    byte[] bytes = encodingUtf8.GetBytes(textData);
                    InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
                    DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
                    datawriter.WriteBytes(bytes);
                    await datawriter.StoreAsync();
                    var httpContent = new HttpStreamContent(memoryStream);
                    httpContent.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue(content_type);
                    httpContent.Headers.ContentLength = (ulong)encodingUtf8.GetBytes(textData).Length;

                    var httpResponseMessage = await client.PostAsync(realUrl, httpContent);
                    var contentType = httpResponseMessage.Content.Headers.ContentType.CharSet = "utf-16";
                    if (httpResponseMessage.EnsureSuccessStatusCode().StatusCode.ToString().ToLower() == "ok")
                    {
                        StorageFolder folder = await KnownFolders.VideosLibrary.CreateFolderAsync("Greeting", CreationCollisionOption.ReplaceExisting);//创建文件夹
                        StorageFile x = await folder.CreateFileAsync("语音文件.wav", CreationCollisionOption.ReplaceExisting);//创建文件
                        StorageFile storageFile = await folder.GetFileAsync("语音文件.wav");
                        string inputStream = await httpResponseMessage.Content.ReadAsStringAsync();
                        byte[] inputBytes = Encoding.Unicode.GetBytes(inputStream);
                        await x.OpenStreamForWriteAsync().Result.WriteAsync(inputBytes, 0, inputBytes.Length);



                        LogTest.LogWrite("responseBody:  "); ;
                    }

                    client.Dispose();
                }
            }
            catch (Exception e)
            {
                LogTest.LogWrite("错误： " + e.Message);
            }
        }
    }
}
