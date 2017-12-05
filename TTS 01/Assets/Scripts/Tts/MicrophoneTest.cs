﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneTest : MonoBehaviour {

    public enum AudioRecordResultState { Success, NoMicrophone, TooShort }
    [SerializeField] private int maxClipLength = 300;
    [HideInInspector] public bool isRecording = false;


    private AudioClip recordedClip;
    private int _sampleWindow = 128;
    private float recordTimer = 0.0f;

    private void Update()
    {
        if (isRecording)
        {
            recordTimer += Time.deltaTime;

        }
    }


    /// <summary>
    /// 开始录制
    /// </summary>
    public AudioRecordResultState StartRecord()
    {
        if (Microphone.devices.Length <= 0)
            return AudioRecordResultState.NoMicrophone;


        recordTimer = 0;
        recordedClip = Microphone.Start(null, false, maxClipLength, 16000);

        isRecording = true;
        Debug.Log("start record-------------");
        return AudioRecordResultState.Success;
    }


    /// <summary>
    /// 获取麦克风音量
    /// </summary>
    /// <returns></returns>
    public float GetLevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1); // null means the first microphone
        if (micPosition < 0) return 0;
        recordedClip.GetData(waveData, micPosition);

        // Getting a peak on the last 128 samples
        for (int i = 0; i < _sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }



    /// <summary>
    /// 停止录制
    /// </summary>
    /// <returns>返回音频保存路径</returns>
    public AudioRecordResultState StopRecord(out string filePath)
    {
        Debug.Log("stop record---------------");

        //Capture the current clip data
        isRecording = false;
        if (recordTimer < 0.5f)
        {
            filePath = null;
            return AudioRecordResultState.TooShort;
        }

        int position = Microphone.GetPosition(null);
        var soundData = new float[recordedClip.samples * recordedClip.channels];
        recordedClip.GetData(soundData, 0);

        //Create shortened array for the data that was used for recording
        var newData = new float[position * recordedClip.channels];


        //Copy the used samples to a new array
        for (int i = 0; i < newData.Length; i++)
        {
            newData[i] = soundData[i];
        }

        //One does not simply shorten an AudioClip,
        //    so we make a new one with the appropriate length
        recordedClip = AudioClip.Create(recordedClip.name,
                                        position,
                                        recordedClip.channels,
                                        recordedClip.frequency,
                                        false);

        recordedClip.SetData(newData, 0);        //Give it the data from the old clip

        //Replace the old clip
        Microphone.End(null);

        //save to disk
        string recordedAudioPath;
        byte[] data = WavUtility.FromAudioClip(recordedClip, out recordedAudioPath, true);
        filePath = recordedAudioPath;

        return AudioRecordResultState.Success;
    }
}