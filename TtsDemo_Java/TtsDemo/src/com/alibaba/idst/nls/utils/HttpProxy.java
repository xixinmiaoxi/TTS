package com.alibaba.idst.nls.utils;

import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.Reader;
import java.io.StringWriter;
import java.io.UnsupportedEncodingException;
import java.io.Writer;
import java.net.HttpURLConnection;
import java.net.URL;
import java.security.MessageDigest;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;

import com.sun.org.apache.xerces.internal.impl.dv.util.Base64;


public class HttpProxy {

	//计算MD5+BASE64
    public static String MD5Base64(String s) throws UnsupportedEncodingException {
        if (s == null) {
            return null;
        }

        String encodeStr = "";

        //string 编码必须为utf-8
        byte[] utfBytes = s.getBytes("UTF-8");

        MessageDigest mdTemp;
        try {
            mdTemp = MessageDigest.getInstance("MD5");
            mdTemp.update(utfBytes);
            byte[] md5Bytes = mdTemp.digest();
            encodeStr = Base64.encode(md5Bytes);
        } catch (Exception e) {
            throw new Error("Failed to generate MD5 : " + e.getMessage());
        }
        return encodeStr;
    }

    //计算 HMAC-SHA1
    public static String HMACSha1(String data, String key) {
        String result;
        try {

            SecretKeySpec signingKey = new SecretKeySpec(key.getBytes(), "HmacSHA1");
            Mac mac = Mac.getInstance("HmacSHA1");
            mac.init(signingKey);
            byte[] rawHmac = mac.doFinal(data.getBytes());
            result = Base64.encode(rawHmac);
        } catch (Exception e) {
            throw new Error("Failed to generate HMAC : " + e.getMessage());
        }
        return result;
    }

    public static String toGMTString(Date date) {
        SimpleDateFormat df = new SimpleDateFormat("E, dd MMM yyyy HH:mm:ss z", Locale.UK);
        df.setTimeZone(new java.util.SimpleTimeZone(0, "GMT"));
        return df.format(date);
    }

    //发送请求
    public static String sendRequest(String url, String body, String ak_id, String ak_secret) {
        OutputStream out = null;
        InputStream in = null;
        HttpURLConnection conn = null;
        String result = "";
        try {
            URL realUrl = new URL(url);

            //http header 参数
            String method = "POST";
            String accept = "application/json";
            String content_type = "application/json";
            String date = toGMTString(new Date());

            // 1.对body做MD5+BASE64加密
            String bodyMd5 = MD5Base64(body);
            String stringToSign = method + "\n" + accept + "\n" + bodyMd5 + "\n" + content_type + "\n" + date ;
            // 2.计算 HMAC-SHA1
            String signature = HMACSha1(stringToSign, ak_secret);
            // 3.得到 authorization header
            String authHeader = "Dataplus " + ak_id + ":" + signature;

            conn = (HttpURLConnection) realUrl.openConnection();
            conn.setRequestMethod(method);
            conn.setRequestProperty("Accept", accept);
            conn.setRequestProperty("Content-Type", content_type);
            conn.setRequestProperty("Date", date);
            conn.setRequestProperty("Authorization", authHeader);
            conn.setDoOutput(true);
            conn.setDoInput(true);

            out = conn.getOutputStream();
            write(body, out, "utf-8");
            out.flush();

            int rc = conn.getResponseCode();
            System.out.printf("RC: "+rc);
            if (rc == 200) {
                in = conn.getInputStream();
            } else {
                in = conn.getErrorStream();
            }
            result = toString(in, "utf-8");
            System.out.printf("result: "+result);
        } catch (Exception e) {
            System.out.println("请求异常：" + e);
            e.printStackTrace();
        }

        finally {
            try {
                if (conn != null) {
                    conn.disconnect();
                }
                if (out != null) {
                    out.close();
                }
                if (in != null) {
                    in.close();
                }
            } catch (IOException ex) {
                ex.printStackTrace();
            }
        }
        return result;
    }

    private static void write(String data, OutputStream output, String encoding)
            throws IOException {
        if (data != null) {
            if (encoding == null) {
                write(data, output);
            } else {
                output.write(data.getBytes(encoding));
            }
        }
    }

    private static void write(String data, OutputStream output)
            throws IOException {
        if (data != null) {
            output.write(data.getBytes());
        }
    }

    private static String toString(InputStream input, String encoding)
            throws IOException {
        StringWriter sw = new StringWriter();
        copy(input, sw, encoding);
        return sw.toString();
    }

private static void copy(InputStream input, Writer output, String encoding)
      throws IOException {
  if (encoding == null) {
      copy(input, output);
  } else {
      InputStreamReader in = new InputStreamReader(input, encoding);
      copy(in, output);
  }
}

private static void copy(InputStream input, Writer output)
      throws IOException {
  InputStreamReader in = new InputStreamReader(input);
  copy(in, output);
}

private static int copy(Reader input, Writer output) throws IOException {
  long count = copyLarge(input, output);
  if (count > Integer.MAX_VALUE) {
      return -1;
  }
  return (int) count;
}

private static long copyLarge(Reader input, Writer output) throws IOException {
  char[] buffer = new char[1024 * 4];
  long count = 0;
  int n = 0;
  while (-1 != (n = input.read(buffer))) {
      output.write(buffer, 0, n);
      count += n;
  }
  return count;
}
}
