﻿using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using NLayer;
using System;
using System.Collections.Generic;
public class ObjectPlacer : MonoBehaviour
{
    [System.Serializable]
    public class PrefabCodePair
    {
        public GameObject prefab;
        public string code;
    }

    [SerializeField]
    private Scroller scroller;
    [SerializeField]
    private GameObject loading;
    [SerializeField]
    private GameObject finisher;

    public PrefabCodePair[] prefabCodePairs;
    public string filePath;
    public AudioClip song;
    public Transform parentObject;

    void Start()
    {
        StartCoroutine(LoadAudio());
        StartCoroutine(PlaceObjectsCoroutine());
        loading.SetActive(true);

    }

    private IEnumerator LoadAudio()
    {
        string basePath = Path.Combine(Application.dataPath, "Beatmaps/song");
        string wavPath = basePath + ".wav";
        string mp3Path = basePath + ".mp3";

        // 1. Próbuj załadować WAV
        if (File.Exists(wavPath))
        {
            UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("file:///" + wavPath, AudioType.WAV);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                song = DownloadHandlerAudioClip.GetContent(req);
                Debug.Log($"WAV załadowany: {song.length}s");
                yield break;
            }
        }

        if (File.Exists(mp3Path))
        {
            try
            {
                using (var fileStream = File.OpenRead(mp3Path))
                using (var mp3Reader = new MpegFile(fileStream))
                {
                    int sampleRate = mp3Reader.SampleRate;
                    int channels = mp3Reader.Channels;
                    float duration = (float)mp3Reader.Duration.TotalSeconds; // Konwersja TimeSpan → float

                    // Obliczenie liczby próbek
                    int totalFrames = (int)(sampleRate * duration * channels);

                    Debug.Log($"MP3 Parametry: {sampleRate}Hz, {channels} kanałów, {duration}s");

                    // Pobranie danych PCM
                    float[] pcmData = new float[totalFrames];
                    int samplesRead = mp3Reader.ReadSamples(pcmData, 0, pcmData.Length);

                    // Tworzenie AudioClip
                    song = AudioClip.Create("MP3_Song", samplesRead / channels, channels, sampleRate, false);
                    song.SetData(pcmData, 0);

                    Debug.Log($"MP3 załadowany: {song.length}s");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Błąd MP3: {e.Message}");
            }
        }

        else
        {
            Debug.LogError("Nie znaleziono pliku audio");
        }
    }
    private IEnumerator PlaceObjectsCoroutine()
    {

        AudioSource mapa = GameObject.Find("Map").GetComponent<AudioSource>();
        while (song == null) //czeka na piosenke
        {
            yield return null;
        }
        mapa.clip = song;
        bool isBeatmapSection = false;
        float lastX= 0;
        filePath = "Assets/Beatmaps/map.txt";
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.Trim() == "[Beatmap]")
            {
                isBeatmapSection = true;
                continue;
            }

            if (isBeatmapSection)
            {
                string[] parts = line.Split(';');

                if (parts.Length == 3)
                {
                    string code = parts[0];

                    if (float.TryParse(parts[1].Trim(), out float x) && float.TryParse(parts[2].Trim(), out float y))
                    {
                        foreach (var pair in prefabCodePairs)
                        {
                            if (pair.code == code)
                            {
                                if (i == lines.Length - 1)
                                {
                                    GenerateLastKey(x / 100, y, pair.prefab);
                                    lastX = x / 100;
                                }
                                else
                                {
                                    GenerateKey(x / 100, y, pair.prefab);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse coordinates: " + parts[1].Trim() + ", " + parts[2].Trim());
                    }
                }

                if (parts.Length == 4)
                {
                    string code = parts[0];

                    if (float.TryParse(parts[1], out float x1) && float.TryParse(parts[2], out float x2) && float.TryParse(parts[3], out float y))
                    {
                        foreach (var pair in prefabCodePairs)
                        {
                            if (pair.code == code)
                            {
                                if (i == lines.Length - 1)
                                {
                                    GenerateLastSlider(x1 / 100, x2 / 100, y, pair.prefab);
                                    lastX = x2 / 100;
                                }
                                else
                                {
                                    GenerateSlider(x1 / 100, x2 / 100, y, pair.prefab);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse coordinates: " + parts[1].Trim() + ", " + parts[2].Trim() + ", " + parts[3].Trim());
                    }
                }
            }

            yield return null;
        }
        mapa.Play();

        loading.SetActive(false);
        scroller.startMap(lastX);
    }

    private void GenerateLastKey(float x, float y, GameObject prefab)
    {
        Vector3 position = new(x, y, 0);
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        newObject.transform.SetParent(parentObject);

        Vector3 position2 = new(x+3, y, 0);
        GameObject newObject2 = Instantiate(finisher, position2, Quaternion.identity);
        newObject2.transform.SetParent(parentObject);
    }

    private void GenerateLastSlider(float x1, float x2, float y, GameObject prefab)
    {
        float diff = x2 - x1;
        float x = diff / 2;
        x += x1;
        Vector3 position = new(x, y, 0);
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        newObject.transform.localScale = new Vector3(diff, newObject.transform.localScale.y, newObject.transform.localScale.z);
        newObject.transform.SetParent(parentObject);

        Vector3 position2 = new(x2 + 3, y, 0);
        GameObject newObject2 = Instantiate(finisher, position2, Quaternion.identity);
        newObject2.transform.SetParent(parentObject);
    }
    private void GenerateKey(float x, float y, GameObject prefab)
    {
        if (prefab != null)
        {
            Vector3 position = new(x, y, 0);
            GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
            newObject.transform.SetParent(parentObject);
        }
    }

    private void GenerateSlider(float x1, float x2, float y, GameObject prefab)
    {
        if (prefab != null)
        {
            float diff = x2 - x1;
            float x = diff / 2;
            x += x1;
            Vector3 position = new(x, y, 0);
            GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
            newObject.transform.localScale = new Vector3(diff, newObject.transform.localScale.y, newObject.transform.localScale.z);
            newObject.transform.SetParent(parentObject);
        }
    }
}

