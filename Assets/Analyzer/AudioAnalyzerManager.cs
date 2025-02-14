using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.IO;

public class AudioAnalyzerManager : MonoBehaviour
{
    private AudioSource audioSource;
    private IAudioFileReader audioFileReader;
    List<Tuple<float, float, int>> audioSpikes = new();
    private MapGenerator generator;
    [SerializeField] private GameObject backButton;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        generator = new MapGenerator();
    }
    void Start()
    {
        backButton.SetActive(false);
    }

    public void Main(string filePath, string outputFilePath)
    {
        outputFilePath = Path.Combine(outputFilePath, "map.txt");
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

                audioSpikes = audioFileReader.GetSpikes();
                FinishedGenerating();
                generator.SetSpikes(audioSpikes);
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
        if (path.EndsWith(".mp3"))
        {
            return new Mp3FileReader(path);
        }
        return null;
    }
    public void FinishedGenerating()
    {
        backButton.SetActive(true);
    }
}
