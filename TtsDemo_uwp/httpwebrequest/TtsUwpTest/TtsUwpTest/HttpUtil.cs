using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;

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

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(realUrl);
                httpWebRequest.Method = method;
                httpWebRequest.Accept = accept;
                httpWebRequest.ContentType = content_type;
                httpWebRequest.Headers["Date"] = date;
                httpWebRequest.Headers["Authorization"] = authHeader;
                httpWebRequest.Headers["ContentLength"] = audioData.Length.ToString();
                
                var stream = await httpWebRequest.GetRequestStreamAsync();
                await stream.WriteAsync(audioData, 0, audioData.Length);
                await stream.FlushAsync();

                var httpWeb = await httpWebRequest.GetResponseAsync();
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWeb;
                if (HttpStatusCode.OK == httpWebResponse.StatusCode)
                {
                    response.setStatus(200);
                    StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
                    string responseContent = streamReader.ReadToEnd();
                    response.setResult(responseContent);
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

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(realUrl);
            httpWebRequest.Method = method;
            httpWebRequest.Accept = accept;
            httpWebRequest.ContentType = content_type;
            httpWebRequest.Headers["Date"] = date;
            httpWebRequest.Headers["Authorization"] = authHeader;
            httpWebRequest.Headers["ContentLength"] = Encoding.UTF8.GetBytes(textData).Length.ToString();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //注册GBK编码
            Encoding encodingUnicode = Encoding.Unicode;

            var stream = await httpWebRequest.GetRequestStreamAsync();
            await stream.WriteAsync(Encoding.UTF8.GetBytes(textData), 0, Encoding.UTF8.GetBytes(textData).Length);
            await stream.FlushAsync();

            var httpWeb = await httpWebRequest.GetResponseAsync();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWeb;
            var aa = httpWebResponse.ContentType;

            if (HttpStatusCode.OK == httpWebResponse.StatusCode)
            {
                try
                {
                    response.setStatus(200);
                    StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.Unicode);

                    var responseContent = await streamReader.ReadToEndAsync();

                    string savePath = ApplicationData.Current.LocalFolder.Path + "/" + audioName + "." + audioType;  //本地保存路径
                    response.setResult(responseContent);
                    byte[] bytes = encodingUnicode.GetBytes(responseContent);

                    using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
                    {
                        var buff = Encoding.Unicode.GetBytes(responseContent);
                        fs.Write(buff, 0, buff.Length);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        { // Always accept
            return true; //总是接受
        }
    }
}
