using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
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
        public static HttpResponse sendAsrPost(byte[] audioData, String audioFormat, int sampleRate, String url, String ak_id, String ak_secret)
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
                httpWebRequest.Date = time;
                httpWebRequest.Headers.Add("Authorization", authHeader);
                httpWebRequest.ContentLength = audioData.Length;

                httpWebRequest.GetRequestStream().Write(audioData, 0, audioData.Length);

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (HttpStatusCode.OK == httpWebResponse.StatusCode)
                {
                    response.setStatus(200);
                    StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
                    string responseContent = streamReader.ReadToEnd();
                    streamReader.Close();
                    httpWebResponse.Close();
                    response.setResult(responseContent);
                    Console.WriteLine("response: " + responseContent );
                    response.setMassage("OK");

                    string savePath = @"D:\1.Txt";    //本地保存路径

                    FileStream fs = new FileStream(savePath, FileMode.Append);
                    var buff = Encoding.Unicode.GetBytes(responseContent);
                    fs.Write(buff, 0, buff.Length);

                    fs.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        /*
         * 发送POST请求
         */
        public static HttpResponse sendTtsPost(String textData, String audioType, String audioName, String url, String ak_id, String ak_secret)
        {
            String result = "";
            HttpResponse response = new HttpResponse();

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
            Console.WriteLine("date: " + date);

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

            //bool bo = GMTStrParse(DateTime.Now.ToString("r"), out time);
            //MethodInfo priMethod = httpWebRequest.Date.GetType().GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
            //priMethod.Invoke(httpWebRequest.Date, new[] { "Kind", "DateTimeKind.Utc" });
            //httpWebRequest.Date = DateTime.ParseExact(DateTime.Now.ToString("r").Replace("GMT", ""), "ddd, dd MMM yyyy HH':'mm':'ss zzz", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            httpWebRequest.Date = time;
            //httpWebRequest.Date.Kind = DateTimeKind.Utc;

            // httpWebRequest.Date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            httpWebRequest.Headers.Add("Authorization", authHeader);
            httpWebRequest.ContentLength = Encoding.UTF8.GetBytes(textData).Length;

            httpWebRequest.GetRequestStream().Write(Encoding.UTF8.GetBytes(textData), 0, Encoding.UTF8.GetBytes(textData).Length);

            Console.WriteLine("Accept: " + httpWebRequest.Accept + "   " + " bodyMd5: " + bodyMd5 + "   " + " ContentType: " + httpWebRequest.ContentType + "  " + " Date: " + httpWebRequest.Date + "   " + " Authorization: " + authHeader + " ContentLength: " + httpWebRequest.ContentLength);

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (HttpStatusCode.OK == httpWebResponse.StatusCode)
            {
                try
                {
                    response.setStatus(200);
                    StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.Unicode);
                    string responseContent = streamReader.ReadToEnd();
                    streamReader.Close();
                    httpWebResponse.Close();
                    response.setResult(responseContent);
                    response.setMassage("OK");
                    Console.WriteLine("post response status code: [" + response.getStatus() + "], generate tts audio file :" + audioName + "." + audioType);

                    string savePath = @"D:\" + audioName + "." + audioType;    //本地保存路径

                    FileStream fs = new FileStream(savePath, FileMode.Append);
                    var buff = Encoding.Unicode.GetBytes(responseContent);
                    fs.Write(buff, 0, buff.Length);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("报错  ==》 " + ex.Message);
                }
            }
            return response;
        }

        public static bool GMTStrParse(string gmtStr, out DateTime gmtTime)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            bool s = DateTime.TryParseExact(gmtStr, "r", enUS, DateTimeStyles.None, out gmtTime);
            return s;
        }
    }
}
