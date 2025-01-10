using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioFileReaderBase : IAudioFileReader
{
    protected List<float> audioAmplitudes = new List<float>();

    public abstract AudioClip ToAudioClip();

    public IEnumerator AnalyzeAudio(AudioClip clip)
    {
        int sampleSize = clip.frequency;
        int totalSamples = clip.samples;

        float[] samples = new float[totalSamples];
        clip.GetData(samples, 0);

        for (int i = 0; i < totalSamples; i += sampleSize)
        {
            int remainingSamples = totalSamples - i;
            int currentSampleSize = Mathf.Min(sampleSize, remainingSamples);

            float[] sampleChunk = new float[currentSampleSize];
            Array.Copy(samples, i, sampleChunk, 0, currentSampleSize);

            float averageAmplitude = CalculateAverageAmplitude(sampleChunk);
            audioAmplitudes.Add(averageAmplitude);

            yield return null;
        }
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
}
