﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class HttpProxy
    {

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
        
        //发送请求
        public static String sendRequest(String url, String body, String ak_id, String ak_secret)
        {
            String result = "";
            HttpResponse response = new HttpResponse();
            try
            {
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

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(realUrl);
                httpWebRequest.Method = method;
                httpWebRequest.Accept = accept;
                httpWebRequest.ContentType = content_type;
                httpWebRequest.ContentLength = Encoding.UTF8.GetBytes(body).Length;
                //httpWebRequest.Date = time;
                MethodInfo priMethod = httpWebRequest.Headers.GetType().GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
                priMethod.Invoke(httpWebRequest.Headers, new[] { "Date", date });
                httpWebRequest.Headers.Add("Authorization", authHeader);
                httpWebRequest.GetRequestStream().Write(Encoding.UTF8.GetBytes(body), 0, Encoding.UTF8.GetBytes(body).Length);
                //HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //if (HttpStatusCode.OK == httpWebResponse.StatusCode)
                //{
                //    response.setStatus(200);

                //    StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
                //    string responseContent = streamReader.ReadToEnd();
                //    streamReader.Close();
                //    httpWebResponse.Close();
                //    response.setResult(responseContent);
                //    response.setMassage("OK");
                //    result = responseContent;
                //    Console.WriteLine(result);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("请求异常！" + e.Message);
            }

            return result;
        }
    }
}
