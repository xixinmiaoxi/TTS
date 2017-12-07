package com.alibaba.idst.nls;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.BufferedReader;
import java.io.OutputStream;
import java.io.FileReader;
import java.util.UUID;

import javax.sound.sampled.AudioFormat;
import javax.sound.sampled.AudioInputStream;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.UnsupportedAudioFileException;

import jdk.nashorn.internal.parser.JSONParser;

import com.alibaba.idst.nls.request.TtsRequest;
import com.alibaba.idst.nls.response.HttpResponse;
import com.alibaba.idst.nls.utils.HttpProxy;
import com.alibaba.idst.nls.utils.HttpUtil;

public class HttpTtsDemo {
	private String url = "http://nlsapi.aliyun.com/speak?";
	private static String tts_text = "薄雾浓云愁永昼。瑞脑消金兽。佳节又重阳，玉枕纱厨，半夜凉初透。东篱把酒黄昏后。有暗香盈袖。莫道不消魂，帘卷西风，人比黄花瘦。";

	public static void main(String[] args) throws IOException {

		// 请使用https://ak-console.aliyun.com/ 页面获取的Access 信息
		// 请提前开通智能语音服务(https://data.aliyun.com/product/nls)
		String ak_id = "LTAIr8I8A5HNZvdt";// "LTAILNHYi9oS9tmi";
		String ak_secret = "rBSsm6hewfAcSH6oeA6VXF6PCAACGD";// "hZH1gI8FYIeOPc7rFxtQl8geg7aaGW";

		// 设置TTS的参数,详细参数说明详见文档部分2.1 参数配置
		HttpTtsDemo ttsDemo = new HttpTtsDemo();
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
		ttsRequest.setBackgroundMusicVolume(100);

		String url = ttsDemo.url + "encode_type=" + ttsRequest.getEncodeType()
				+ "&voice_name=" + ttsRequest.getVoiceName() + "&volume="
				+ ttsRequest.getVolume() + "&sample_rate="
				+ ttsRequest.getSampleRate() + "&speech_rate="
				+ ttsRequest.getSpeechRate() + "&pitch_rate="
				+ ttsRequest.getPitchRate() + "&tts_nus="
				+ ttsRequest.getTtsNus() + "&background_music_id="
				+ ttsRequest.getBackgroundMusicId()
				+ "&background_music_offset="
				+ ttsRequest.getBackgroundMusicOffset()
				+ "&background_music_volume="
				+ ttsRequest.getBackgroundMusicVolume();

		String fileName = UUID.randomUUID().toString().replace("-", "");
		
		// tts demo 文字转语音
		// HttpResponse response =
		// HttpUtil.sendTtsPost(tts_text,ttsRequest.getEncodeType(), fileName,
		// url, ak_id, ak_secret);

		
		// tts语音转文字
		
		  /*File file = new File("D:\\Hello.wav"); long fileSize = file.length();
		  FileInputStream fi = new FileInputStream(file); byte[] buffer = new
		  byte[(int) fileSize]; int offset = 0; int numRead = 0; while (offset
		  < buffer.length && (numRead = fi.read(buffer, offset, buffer.length -
		  offset)) >= 0) { offset += numRead; } fi.close();
		  
		  for (int i = 0; i < buffer.length; i++) { System.out.printf(buffer[i]
		  + " "); }
		  
		  try 
		  {
		  AudioInputStream audioInputStream = AudioSystem.getAudioInputStream(new File("D:\\Hello.wav"));
		  AudioFormat audioFormat = audioInputStream.getFormat();
		  System.out.print("audioFormat: " + audioFormat); HttpResponse
		  response = HttpUtil.sendAsrPost(buffer,"wav", 16000, url, ak_id,  ak_secret); 
		  } 
		  catch (UnsupportedAudioFileException e)
		  { 
			  e.printStackTrace();
		  }
		 */
		 String urls = "http://nlsapi.aliyun.com/recognize?";
		 //使用对应的ASR模型 详情见文档部分2
	       String model = "chat";
	       urls = urls+"model="+model;

	       //读取本地的语音文件
	       File file = new File("D:\\Hello.wav"); 
	       long fileSize = file.length();
			  FileInputStream fi = new FileInputStream(file); 
			  byte[] buffer = new byte[(int) fileSize]; 
			  int offset = 0;
			  int numRead = 0;
			  while (offset < buffer.length && (numRead = fi.read(buffer, offset, buffer.length -  offset)) >= 0) 
			  { 
				  offset += numRead; 
				  }
			  fi.close();

	       HttpResponse response = HttpUtil.sendAsrPost(buffer,"pcm",16000,urls,ak_id,ak_secret);
		
		
		//智能对话
		/*String urlStr = "https://nlsapi.aliyun.com/manage/qas?action=single:prepub:qa";   //提问链接
		//String urlStr = "https://nlsapi.aliyun.com/manage/qas?action=projects:list";    //获取项目projectID链接
		//String urlStr = "https://nlsapi.aliyun.com/manage/qas";                         //智能对话链接
		
		String bdStr = "{\"projectId" + "\"" + ":" + 4649 + "," + "\"question" + "\"" + ":" + "\"" + "12345" + "\"}";   //提问对话
	
		//String bdStr = "{\"offset" + "\"" + ":" + 0+ "," + "\"pageSize" + "\"" + ":" + 1 +"}";           //获取ProjectID
		
		//智能对话
		 //String bdStr = "{\"version"+"\""+":"+"\""+"2.0"+"\""+"," + "\"app_key"+"\""+":"+"\""+"nui-d9lLBVpmR0dl"+"\""+"," + "\"question"+"\""+":"+"\""+"123545"+"\"}";
		System.out.print(bdStr);
		String responseStr = HttpProxy.sendRequest(urlStr, bdStr, ak_id,
				ak_secret);
				*/
	}
}
