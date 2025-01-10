using System;
using System.IO;
using UnityEngine;

public class Mp3FileReader : AudioFileReaderBase
{
    private string filePath;

    public Mp3FileReader(string path)
    {
        filePath = path;
    }

    public override AudioClip ToAudioClip()
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);

        // Sprawdzanie, czy plik jest poprawnym MP3 (proste sprawdzenie nagłówka)
        if (!IsMp3File(fileBytes))
        {
            throw new Exception("Plik nie jest prawidłowym plikiem MP3.");
        }

        float[] audioData = ParseMp3ToPCM(fileBytes, out int sampleRate, out int channels);

        AudioClip clip = AudioClip.Create("Mp3Audio", audioData.Length, channels, sampleRate, false);
        clip.SetData(audioData, 0);
        return clip;
    }

    private bool IsMp3File(byte[] fileBytes)
    {
        return fileBytes.Length > 2 &&
               fileBytes[0] == 0xFF &&
               (fileBytes[1] & 0xE0) == 0xE0; // Nagłówek MP3
    }

    private float[] ParseMp3ToPCM(byte[] fileBytes, out int sampleRate, out int channels)
    {
        sampleRate = 44100;  // Zakładamy standardową częstotliwość próbkowania
        channels = 2;        // Zakładamy stereo

        int bytesPerSample = 2; // 16-bitowe próbki
        int totalSamples = (fileBytes.Length - 44) / bytesPerSample; // Przykładowa konwersja

        float[] audioData = new float[totalSamples];
        for (int i = 0; i < totalSamples; i++)
        {
            int sampleIndex = 44 + i * bytesPerSample;
            short sample = BitConverter.ToInt16(fileBytes, sampleIndex);
            audioData[i] = sample / 32768f; // Normalizacja
        }

        return audioData;
    }
}
