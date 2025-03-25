using System;
using System.IO;
using System.Numerics;
using UnityEngine;

public class WavFileReader : AudioFileReaderBase
{
    private string _filePath;

    public WavFileReader(string path)
    {
        _filePath = path;
    }

    public override AudioClip ToAudioClip()
    {
        byte[] fileBytes = File.ReadAllBytes(_filePath);

        int sampleRate = BitConverter.ToInt32(fileBytes, 24);
        int channels = BitConverter.ToInt16(fileBytes, 22);
        int bitsPerSample = BitConverter.ToInt16(fileBytes, 34);
        int dataStartIndex = FindDataChunkIndex(fileBytes);

        if (dataStartIndex == -1)
        {
            throw new Exception("Nie mo¿na znaleŸæ sekcji danych w pliku WAV.");
        }

        float[][] channelSamples = ParseAudioData(fileBytes, dataStartIndex, bitsPerSample, channels);

        // laczenie kanalow do AudioClip
        int totalSamples = channelSamples[0].Length;
        float[] mergedSamples = new float[totalSamples * channels];
        for (int i = 0; i < totalSamples; i++)
        {
            for (int ch = 0; ch < channels; ch++)
            {
                mergedSamples[i * channels + ch] = channelSamples[ch][i];
            }
        }

        AudioClip clip = AudioClip.Create("WavAudio", totalSamples, channels, sampleRate, false);
        clip.SetData(mergedSamples, 0);

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

    private float[][] ParseAudioData(byte[] fileBytes, int dataStartIndex, int bitsPerSample, int channels)
    {
        int bytesPerSample = bitsPerSample / 8;
        int totalSamples = (fileBytes.Length - dataStartIndex) / bytesPerSample / channels;

        float[][] channelSamples = new float[channels][];
        for (int ch = 0; ch < channels; ch++)
        {
            channelSamples[ch] = new float[totalSamples];
        }

        for (int i = 0; i < totalSamples; i++)
        {
            for (int ch = 0; ch < channels; ch++)
            {
                int sampleIndex = dataStartIndex + (i * channels + ch) * bytesPerSample;

                if (bytesPerSample == 2)
                {
                    short sample = BitConverter.ToInt16(fileBytes, sampleIndex);
                    channelSamples[ch][i] = sample / 32768f;
                }
                else if (bytesPerSample == 1)
                {
                    byte sample = fileBytes[sampleIndex];
                    channelSamples[ch][i] = (sample - 128) / 128f;
                }
                else
                {
                    throw new NotSupportedException($"Nieobs³ugiwany rozmiar próbek: {bitsPerSample} bitów.");
                }
            }
        }

        return channelSamples;
    }

}
