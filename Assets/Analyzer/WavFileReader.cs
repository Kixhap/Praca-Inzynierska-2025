using System;
using System.Collections;
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

        float[][] channelSamples = ParseAudioData(fileBytes, dataStartIndex, bitsPerSample, channels);

        // £¹czenie danych kana³ów dla AudioClip
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

    public IEnumerator AnalyzeAudio(AudioClip clip)
    {
        int sampleSize = clip.frequency;
        int totalSamples = clip.samples;
        int channels = clip.channels;

        float[] samples = new float[totalSamples * channels];
        clip.GetData(samples, 0);

        float spikeThreshold = 0.2f;

        for (int i = 0; i < totalSamples; i += sampleSize)
        {
            int remainingSamples = totalSamples - i;
            int currentSampleSize = Mathf.Min(sampleSize, remainingSamples);

            for (int channel = 0; channel < channels; channel++)
            {
                float[] sampleChunk = new float[currentSampleSize];
                for (int j = 0; j < currentSampleSize; j++)
                {
                    sampleChunk[j] = samples[(i + j) * channels + channel];
                }

                float averageAmplitude = CalculateAverageAmplitude(sampleChunk);
                audioAmplitudes.Add(averageAmplitude);

                if (averageAmplitude > spikeThreshold)
                {
                    float time = i / (float)clip.frequency;
                    Debug.Log($"Spike on channel {channel} at time: {time:F10}, amplitude: {averageAmplitude}");
                    spikes.Add(new Tuple<float, float, int>(averageAmplitude, time, channel));
                }
            }

            yield return null;
        }
    }

}

