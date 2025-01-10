using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator
{
    private float sensitivity;
    List<Tuple<float, float, int>> audioSpikes = new List<Tuple<float, float, int>>();

    public MapGenerator(float sensitivity)
    {
        this.sensitivity = sensitivity;
    }

    public void SetSpikes(List<Tuple<float, float, int>> spikes)
    {
        audioSpikes = spikes;
    }

    public void GenerateMap(string outputFilePath)
    {
        List<string> beatmapData = new List<string> { "[Beatmap]" };

        string directoryPath = Path.GetDirectoryName(outputFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath); // Tworzenie brakuj¹cych katalogów
        }

        if (audioSpikes == null || audioSpikes.Count == 0)
        {
            Debug.LogError("No amplitudes available for map generation.");
            return;
        }

        for (int i = 0; i < audioSpikes.Count; i++)
        {
            beatmapData.Add($"1;{audioSpikes[i]};");
        }

        try
        {
            File.WriteAllLines(outputFilePath, beatmapData);
            Debug.Log($"Beatmap saved to: {outputFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save beatmap to file. Error: {e.Message}");
        }
    }
}
