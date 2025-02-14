using UnityEngine;
using System.IO;
using NLayer;
using System;

public class Mp3FileReader : AudioFileReaderBase
{
    private string _filePath;

    public Mp3FileReader(string path)
    {
        _filePath = path;
    }

    public override AudioClip ToAudioClip()
    {
        using (var fileStream = File.OpenRead(_filePath))
        using (var mp3Reader = new MpegFile(fileStream))
        {
            // Pobranie parametr�w audio
            int sampleRate = mp3Reader.SampleRate;
            int channels = mp3Reader.Channels;

            // Pobranie d�ugo�ci w sekundach
            float duration = (float)mp3Reader.Duration.TotalSeconds;

            // Obliczenie liczby pr�bek (uwzgl�dnia czas trwania)
            int totalSampleFrames = (int)(sampleRate * duration);

            // Bufor na pr�bki
            float[] pcmData = new float[totalSampleFrames * channels];

            // Pobranie pr�bek bezpo�rednio w formacie float
            int samplesRead = mp3Reader.ReadSamples(pcmData, 0, pcmData.Length);

            // Tworzenie AudioClip
            AudioClip clip = AudioClip.Create(
                "Mp3Audio",
                samplesRead / channels,  // Liczba pr�bek na kana�
                channels,
                sampleRate,
                false
            );

            clip.SetData(pcmData, 0);
            return clip;
        }
    }
}
