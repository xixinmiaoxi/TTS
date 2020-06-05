using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ClassLibrary1;

namespace ConsoleApp1
{
    class Program : MonoBehaviour
    {
        void Start()
        {
            #region  tts  文字--》语音
            String url = "http://nlsapi.aliyun.com/speak?";
            //String tts_text = "薄雾浓云愁永昼。瑞脑消金兽。佳节又重阳，玉枕纱厨，半夜凉初透。东篱把酒黄昏后。有暗香盈袖。莫道不消魂，帘卷西风，人比黄花瘦。";
            String tts_text = "32号谁啊，我怎么没见过？";
            //请使用https://ak-console.aliyun.com/ 页面获取的Access 信息
            //请提前开通智能语音服务(https://data.aliyun.com/product/nls)
            //String ak_id = "LTAILNHYi9oS9tmi";
            //String ak_secret = "hZH1gI8FYIeOPc7rFxtQl8geg7aaGW";

            //设置TTS的参数,详细参数说明详见文档部分2.1 参数配置
            Program ttsDemo = new Program();
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
            //HttpResponse response = HttpUtil.sendTtsPost(tts_text, ttsRequest.getEncodeType(), fileName, ttsUrlSpeak, ak_id, ak_secret);
            #endregion

            #region tts 语音--》文字
            //String urls = "http://nlsapi.aliyun.com/recognize?";
            ////使用对应的ASR模型 详情见文档部分2
            //String model = "chat";
            //urls = urls + "model=" + model;
            //FileStream fs = new FileStream(@"D:\1.wav", FileMode.Open, FileAccess.Read);
            //byte[] audioBytes = new byte[fs.Length];
            //fs.Read(audioBytes, 0, (int)fs.Length);

            //HttpResponse responseRecognize = HttpUtil.sendAsrPost(audioBytes, "pcm", 16000, urls, ak_id, ak_secret);

            //fs.Close();
            #endregion


            ////智能对话
            //String urlStr = "https://nlsapi.aliyun.com/manage/qas?action=single:prepub:qa";
            ////String urlStr = "https://nlsapi.aliyun.com/manage/qas?action=projects:list";
            //String urlStrTest = "https://nlsapi.aliyun.com/manage/qas";

            //String bdStr = "{\"projectId" + "\"" + ":" + 4649 + "," + "\"question" + "\"" + ":" + "\"" + "请问勒布朗詹姆斯最近的技术统计怎么样？" + "\"}";

            ////String bdStr = "{\"offset" + "\"" + ":" + 0+ "," + "\"pageSize" + "\"" + ":" + 1 +"}";

            //String bdStrTest = "{\"version" + "\"" + ":" + "\"" + "2.0" + "\"" + "," + "\"app_key" + "\"" + ":" + "\"" + "nui-d9lLBVpmR0dl" + "\"" + "," + "\"question" + "\"" + ":" + "\"" + "请问勒布朗詹姆斯最近的技术统计怎么样？" + "\"}";

            //String responseStrTest = HttpProxy.sendRequest(urlStrTest, bdStrTest, ak_id, ak_secret);
            //Debug.Log(responseStrTest);

            //String responseStr = HttpProxy.sendRequest(urlStr, bdStr, ak_id, ak_secret);
            // Debug.Log(responseStr);

            //语音采集
            //SoundRecord recorder = new SoundRecord();
            ////开始录音
            //string wavfile = null;
            //wavfile = "test.wav";
            //recorder.SetFileName(wavfile);
            //recorder.RecStart();
            ////结束
            //recorder.RecStop();
            //recorder = null;

        }

    }
}
