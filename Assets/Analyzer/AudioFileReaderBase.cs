using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioFileReaderBase : IAudioFileReader
{
    protected List<float> audioAmplitudes = new List<float>();
    protected List<Tuple<float, float, int>> spikes = new List<Tuple<float, float, int>>();  // Tuple of (Amplitude, Time, Channel)

    public abstract AudioClip ToAudioClip();

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
        }
        yield return null;
    }

    protected float CalculateAverageAmplitude(float[] sampleChunk)
    {
        float sum = 0f;
        foreach (var sample in sampleChunk)
        {
            sum += Mathf.Abs(sample);
        }
        return sum / sampleChunk.Length;
    }

    public List<float> GetAmplitudes()
    {
        return audioAmplitudes;
    }

    public List<Tuple<float, float, int>> GetSpikes()
    {
        return spikes;
    }
}
