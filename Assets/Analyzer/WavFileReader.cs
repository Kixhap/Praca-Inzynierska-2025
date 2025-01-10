using System;
using System.IO;
using UnityEngine;

public class WavFileReader : AudioFileReaderBase
{
    private string filePath;

    public WavFileReader(string path)
    {
        filePath = path;
    }

    public override AudioClip ToAudioClip()
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);

        int sampleRate = BitConverter.ToInt32(fileBytes, 24);
        int channels = BitConverter.ToInt16(fileBytes, 22);
        int bitsPerSample = BitConverter.ToInt16(fileBytes, 34);
        int dataStartIndex = FindDataChunkIndex(fileBytes);

        if (dataStartIndex == -1)
        {
            throw new Exception("Nie mo¿na znaleŸæ sekcji danych w pliku WAV.");
        }

        float[] audioData = ParseAudioData(fileBytes, dataStartIndex, bitsPerSample);

        AudioClip clip = AudioClip.Create("WavAudio", audioData.Length, channels, sampleRate, false);
        clip.SetData(audioData, 0);
        return clip;
    }

    private int FindDataChunkIndex(byte[] fileBytes)
    {
        for (int i = 0; i < fileBytes.Length - 4; i++)
        {
            if (fileBytes[i] == 'd' && fileBytes[i + 1] == 'a' && fileBytes[i + 2] == 't' && fileBytes[i + 3] == 'a')
            {
                return i + 8;
            }
        }
        return -1;
    }

    private float[] ParseAudioData(byte[] fileBytes, int dataStartIndex, int bitsPerSample)
    {
        int bytesPerSample = bitsPerSample / 8;
        int sampleCount = (fileBytes.Length - dataStartIndex) / bytesPerSample;

        float[] audioData = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            int sampleIndex = dataStartIndex + i * bytesPerSample;

            if (bytesPerSample == 2)
            {
                short sample = BitConverter.ToInt16(fileBytes, sampleIndex);
                audioData[i] = sample / 32768f;
            }
            else if (bytesPerSample == 1)
            {
                byte sample = fileBytes[sampleIndex];
                audioData[i] = (sample - 128) / 128f;
            }
            else
            {
                throw new NotSupportedException($"Nieobs³ugiwany rozmiar próbek: {bitsPerSample} bitów.");
            }
        }
        return audioData;
    }
}
