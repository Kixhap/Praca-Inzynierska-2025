using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AudioFileReaderBase : IAudioFileReader
{
    protected List<float> beatTimes = new List<float>();
    protected float bpm = 120;
    protected int fftWindowSize = 1024;
    protected float beatThreshold = 0.1f;
    private Queue<float> energyBuffer = new Queue<float>();
    private const int bufferSize = 20; // Liczba ostatnich okien do analizy

    public abstract AudioClip ToAudioClip();


    public IEnumerator AnalyzeAudio(AudioClip clip)
    {
        int channels = clip.channels;
        int totalSamples = clip.samples;
        float[] samples = new float[totalSamples * channels];
        clip.GetData(samples, 0);

        int currentIndex = 0;

        // Przetwarzamy próbki oknem o d³ugoœci fftWindowSize z 50% nak³adem
        while (currentIndex + fftWindowSize < totalSamples)
        {
            // Tworzymy okno próbek – uœredniamy wszystkie kana³y
            float[] windowSamples = new float[fftWindowSize];
            for (int i = 0; i < fftWindowSize; i++)
            {
                float sum = 0f;
                for (int ch = 0; ch < channels; ch++)
                {
                    sum += samples[(currentIndex + i) * channels + ch];
                }
                windowSamples[i] = sum / channels;
            }

            // Obliczamy energiê okna
            float totalEnergy = CalculateEnergy(windowSamples);

            // Sprawdzamy oba warunki: adaptacyjny próg energetyczny i analizê FFT
            if (IsBeat(totalEnergy) && IsBeatInFrequencyRange(windowSamples, clip.frequency))
            {
                float time = currentIndex / (float)clip.frequency;
                // Dodajemy beat tylko, gdy odstêp miêdzy beatami wynosi co najmniej 0.1 s
                if (beatTimes.Count == 0 || time - beatTimes[beatTimes.Count - 1] > 0.1f)
                {
                    beatTimes.Add(time);
                    Debug.Log($"Beat detected at: {time:F2}s");
                }
            }

            // Przesuniecie okna o po³owê d³ugoœci (50% overlap)
            currentIndex += fftWindowSize / 2;
            yield return null;
        }

    }

    // Wykonuje FFT na podanych próbkach i analizuje energiê w zakresie 40-150 Hz.
    private bool IsBeatInFrequencyRange(float[] samples, int sampleRate)
    {
        Complex[] fftBuffer = new Complex[fftWindowSize];
        for (int i = 0; i < fftWindowSize; i++)
        {
            fftBuffer[i] = new Complex(samples[i], 0);
        }

        FFT(fftBuffer);

        // Ustal zakres niskich czêstotliwoœci (40-150 Hz)
        int lowEnd = (int)(40 * fftWindowSize / sampleRate);
        int highEnd = (int)(150 * fftWindowSize / sampleRate);

        float maxEnergy = 0f;
        for (int i = lowEnd; i <= highEnd && i < fftBuffer.Length; i++)
        {
            maxEnergy = Mathf.Max(maxEnergy, (float)fftBuffer[i].Magnitude);
        }

        // Warunek – energia w analizowanym paœmie musi byæ wiêksza od beatThreshold
        return maxEnergy > beatThreshold;
    }

    //implementacja FFT.
    private void FFT(Complex[] buffer)
    {
        int n = buffer.Length;
        int bits = (int)Math.Log(n, 2);

        // Permutacja bitowa
        for (int i = 0; i < n; i++)
        {
            int j = ReverseBits(i, bits);
            if (i < j)
            {
                (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
            }
        }

        // Motyw "Butterfly"
        for (int len = 2; len <= n; len *= 2)
        {
            double angle = -2 * Math.PI / len;
            Complex wLen = new Complex(Math.Cos(angle), Math.Sin(angle));

            for (int i = 0; i < n; i += len)
            {
                Complex w = Complex.One;
                for (int j = 0; j < len / 2; j++)
                {
                    Complex u = buffer[i + j];
                    Complex v = w * buffer[i + j + len / 2];
                    buffer[i + j] = u + v;
                    buffer[i + j + len / 2] = u - v;
                    w *= wLen;
                }
            }
        }
    }
    // Odwraca bity liczby, wykorzystywane przy permutacji bitowej FFT.
    private int ReverseBits(int num, int bitSize)
    {
        int result = 0;
        for (int i = 0; i < bitSize; i++)
        {
            result = (result << 1) | (num & 1);
            num >>= 1;
        }
        return result;
    }

    // Oblicza avg energie (z kwadratów wartosci) danego okna próbek.
    private float CalculateEnergy(float[] samples)
    {
        float sum = 0f;
        foreach (var sample in samples)
        {
            sum += sample * sample;
        }
        return sum / samples.Length;
    }


    public List<Tuple<float, float, int>> GetSpikes()
    {
        var result = new List<Tuple<float, float, int>>();
        foreach (var time in beatTimes)
        {
            result.Add(new Tuple<float, float, int>(1f, time, 0));
        }
        return result;
    }

    private bool IsBeat(float energy)
    {
        if (energyBuffer.Count >= bufferSize)
        {
            energyBuffer.Dequeue(); // usun najstarszy wpis
        }
        energyBuffer.Enqueue(energy); // Dodaje nowa wartosc


        if (energyBuffer.Count < bufferSize)
            return energy > beatThreshold;

        // avg
        float sum = 0f;
        foreach (var e in energyBuffer)
        {
            sum += e;
        }
        float avgEnergy = sum / bufferSize;

        // odchylenie
        float variance = 0f;
        foreach (var e in energyBuffer)
        {
            variance += (e - avgEnergy) * (e - avgEnergy);
        }
        variance /= bufferSize;
        float stdDev = Mathf.Sqrt(variance);

        return energy > avgEnergy + stdDev; // beat, gdy energia > œredni¹ + odchylenie
    }
}
