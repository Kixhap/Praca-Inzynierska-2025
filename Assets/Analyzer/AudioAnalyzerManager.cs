using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class AudioAnalyzerManager : MonoBehaviour
{
    public float sensitivity = 1.0f;
    private AudioSource audioSource;
    private IAudioFileReader audioFileReader;
    public List<float> audioAmplitudes;
    private MapGenerator generator;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        generator = new MapGenerator(sensitivity);
    }

    public void Main(string filePath, string outputFilePath)
    {
        CultureInfo customCulture = new("pl-PL");
        customCulture.NumberFormat.NumberDecimalSeparator = ",";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        StartCoroutine(LoadAndAnalyzeAudio(filePath, outputFilePath));
    }

    IEnumerator LoadAndAnalyzeAudio(string path, string outputFilePath)
    {
        audioFileReader = CreateReaderForFile(path);

        if (audioFileReader != null)
        {
            AudioClip clip = audioFileReader.ToAudioClip();
            if (clip != null)
            {
                audioSource.clip = clip;

                yield return StartCoroutine(audioFileReader.AnalyzeAudio(clip));

                audioAmplitudes = audioFileReader.GetAmplitudes();
                generator.SetAmplitudes(audioAmplitudes);
                generator.GenerateMap(outputFilePath);
            }
        }
        else
        {
            Debug.LogError("Nieobs³ugiwany format pliku!");
        }
    }

    IAudioFileReader CreateReaderForFile(string path)
    {
        if (path.EndsWith(".wav"))
        {
            return new WavFileReader(path);
        }
        // Mo¿esz dodaæ inne formaty tutaj
        return null;
    }
}
