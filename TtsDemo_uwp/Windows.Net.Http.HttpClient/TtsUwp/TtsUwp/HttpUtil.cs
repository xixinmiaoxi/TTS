using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
//using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;

namespace TtsUwp
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
            try
            {
                Uri realUrl = new Uri(url);
                /*
                 * http header 参数
                 */
                String method = "POST";
                String accept = "application/json";
                String content_type = "audio/" + audioFormat;// + ";samplerate=" + sampleRate;
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

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //注册GBK编码
                Encoding encodingUtf8 = Encoding.UTF8;

                using (var handler = new HttpClientHandler())
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                    using (var client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("Method", method);
                        client.DefaultRequestHeaders.Add("Accept", accept);
                        client.DefaultRequestHeaders.Add("ContentType", content_type);
                        client.DefaultRequestHeaders.Add("ContentLength", audioData.Length.ToString());
                        client.DefaultRequestHeaders.Add("Authorization", authHeader);
                        client.DefaultRequestHeaders.Date = time;
                        client.DefaultRequestHeaders.ExpectContinue = true;
                        //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                        client.Timeout = new TimeSpan(1, 0, 0);

                        //using (Stream sr = new MemoryStream())
                        {
                            Stream file = KnownFolders.VideosLibrary.OpenStreamForReadAsync("1.wav").Result;

                            HttpContent httpContent = new StreamContent(file);
                            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                            httpContent.Headers.ContentLength = audioData.Length;

                            //sr.Write(audioData, 0, audioData.Length);
                            //await httpContent.CopyToAsync(sr);
                            //sr.Seek(0, SeekOrigin.Begin);
                            //await sr.FlushAsync();

                            //using (HttpContent httpContent = new ByteArrayContent(encodingUtf8.GetBytes(textData)))
                            {
                                var httpResponseMessage = await client.PostAsync(realUrl, httpContent);

                                if (httpResponseMessage.EnsureSuccessStatusCode().StatusCode.ToString().ToLower() == "ok")
                                {
                                    byte[] responseBody = await httpResponseMessage.Content.ReadAsByteArrayAsync();

                                    //using (Stream responseBodys = new MemoryStream())
                                    //{
                                    //    await httpResponseMessage.Content.CopyToAsync(responseBodys);
                                    //    responseBodys.Seek(0, SeekOrigin.Begin);
                                    //    await responseBodys.FlushAsync();
                                    //}


                                    // StreamReader reader = new StreamReader(responseBody);
                                    //string responseResult = reader.ReadToEnd();

                                    //string savePath = ApplicationData.Current.LocalFolder.Path + "/" + audioName + "." + audioType;
                                    //using (FileStream fs = new FileStream(savePath, FileMode.Append))
                                    //{
                                    //    var buff = encodingUtf16.GetBytes(responseResult);
                                    //    fs.Write(buff, 0, buff.Length);
                                    //}
                                }
                            }
                        }

                        client.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                int a = 0;
            }
            //return response;
        }


        /*
         * 发送POST请求
         */
        public static async void sendTtsPost(String textData, String audioType, String audioName, String url, String ak_id, String ak_secret)
        {
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
                String accept = "audio/" + audioType;// + ",application/json";

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


                // Create a New HttpClient object.
                using (var handler = new HttpClientHandler())
                {
                    //handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                    using (var client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("Method", method);
                        client.DefaultRequestHeaders.Add("Accept", accept);
                        client.DefaultRequestHeaders.Add("ContentType", content_type);
                        client.DefaultRequestHeaders.Add("ContentLength", encodingUtf8.GetBytes(textData).Length.ToString());
                        client.DefaultRequestHeaders.Add("Authorization", authHeader);
                        client.DefaultRequestHeaders.Date = time;

                        using (Stream sr = new MemoryStream())
                        {
                            HttpContent httpContent = new StreamContent(sr);
                            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                            httpContent.Headers.ContentLength = encodingUtf8.GetBytes(textData).Length;

                            sr.Write(encodingUtf8.GetBytes(textData), 0, encodingUtf8.GetBytes(textData).Length);
                            await httpContent.CopyToAsync(sr);
                            sr.Seek(0, SeekOrigin.Begin);
                            await sr.FlushAsync();

                            var httpResponseMessage = await client.PostAsync(realUrl, httpContent);

                            if (httpResponseMessage.EnsureSuccessStatusCode().StatusCode.ToString().ToLower() == "ok")
                            {
                                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                                
                            }
                        }

                        client.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                int a = 0;
            }
        }
    }
}
