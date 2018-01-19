#if !NETFX_CORE 
using System;
using System.IO;
using UnityEngine;
#else
using System;
using System.IO;
using Windows.Storage;
using UnityEngine;
#endif

//[RequireComponent(typeof(AudioSource))]
public class RecordingWav : MonoBehaviour
{
    string filePath = null;

    private AudioSource m_audioSource;
    private AudioClip m_audioClip;

    public const int SamplingRate = 16000;
    private const int HEADER_SIZE = 44;

    public bool isRecording = false;

    public Byte[] speech_Byte;
    // Use this for initialization  
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();

#if !NETFX_CORE
        filePath = Application.persistentDataPath + "/";
#else
        filePath = ApplicationData.Current.LocalFolder.Path + "/";
#endif
    }

    // Update is called once per frame  
    void Update()
    {

    }
    public void StartRecording()
    {
        Microphone.End(null);
        foreach (string d in Microphone.devices)
        {
            Debug.Log("Devid :" + d);
        }

        m_audioClip = Microphone.Start(null, false, 60, SamplingRate);
    }

    public string StopRecording()
    {
        int audioLength = 0;
        int lastPos = Microphone.GetPosition(null);

        if (Microphone.IsRecording(null))
        {
            audioLength = lastPos / SamplingRate;
        }
        else
        {
            audioLength = 1;
        }
        Microphone.End(null);

        if (audioLength <= 1.0f)
        {
            return null;
        }
        string fileName = System.Guid.NewGuid().ToString("N");
        SaveWav(fileName, m_audioClip);
        return fileName;
    }

    bool SaveWav(string filename, AudioClip clip)
    {
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }

        filePath = filePath + filename;

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using (FileStream fileStream = CreateEmpty(filePath))
        {
            ConvertAndWrite(fileStream, clip);
        }
        return true;
    }

    FileStream CreateEmpty(string filePath)
    {
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++)
        {
            fileStream.WriteByte(emptyByte);
        }
        return fileStream;
    }

    void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        float[] samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];

        Byte[] bytesData = new Byte[samples.Length * 2];

        int rescaleFactor = 32767; //to convert float to Int16  

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);

            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        speech_Byte = bytesData;

        fileStream.Write(bytesData, 0, bytesData.Length);

        WriteHeader(fileStream, clip);
    }
    void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        using (FileStream stream = fileStream)
        {
            int hz = clip.frequency;
            int channels = clip.channels;
            int samples = clip.samples;

            stream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            stream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(stream.Length - 8);
            stream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            stream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            stream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            stream.Write(subChunk1, 0, 4);

            UInt16 two = 2;
            UInt16 one = 1;

            Byte[] audioFormat = BitConverter.GetBytes(one);
            stream.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(channels);
            stream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(hz);
            stream.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2    
            stream.Write(byteRate, 0, 4);

            UInt16 blockAlign = (ushort)(channels * 2);
            stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(bps);
            stream.Write(bitsPerSample, 0, 2);

            Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
            stream.Write(datastring, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(samples * 2 * channels);
            stream.Write(subChunk2, 0, 4);
        }
    }

}

