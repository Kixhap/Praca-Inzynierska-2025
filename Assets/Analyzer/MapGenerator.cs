using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator
{
    private float sensitivity;
    List<Tuple<float, float, int>> audioSpikes = new();

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
        List<string> beatmapData = new();

        string directoryPath = Path.GetDirectoryName(outputFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath); // Tworzenie folderu jesli brak
        }

        if (audioSpikes == null || audioSpikes.Count == 0)
        {
            Debug.LogError("No amplitudes");
            return;
        }

        for (int i = 0; i < audioSpikes.Count; i++)
        {
            var spike = audioSpikes[i];
            float timeInMilliseconds = spike.Item2 * 1000;
            int channel = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;

            beatmapData.Add($"1;{(int)timeInMilliseconds};{channel}");

        }

        // Filtracja jak za blisko
        List<string> uniqueData = RemoveClosePoints(beatmapData);

        try
        {
            // prefiks jako pierwszy w liscie
            if (!uniqueData[0].Equals("[Beatmap]"))
            {
                uniqueData.Insert(0, "[Beatmap]");
            }

            File.WriteAllLines(outputFilePath, uniqueData);
            Debug.Log($"Beatmap saved to: {outputFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save beatmap to file. Error: {e.Message}");
        }
    }

    private List<string> RemoveClosePoints(List<string> beatmapData)
    {
        List<string> filteredData = new();
        float threshold = 10.0f; // Minimalna roznica czasu w ms

        for (int i = 0; i < beatmapData.Count; i++)
        {
            string[] parts = beatmapData[i].Split(';');
            if (parts.Length < 3) continue;

            float time1 = float.Parse(parts[1]);
            bool addCurrent = true;

            for (int j = i + 1; j < beatmapData.Count; j++)
            {
                string[] nextParts = beatmapData[j].Split(';');
                if (nextParts.Length < 3) continue;

                float time2 = float.Parse(nextParts[1]);
                if (Math.Abs(time1 - time2) < threshold)
                {
                    beatmapData[j] = ""; // usun jak za blisko
                }
            }

            if (addCurrent)
            {
                filteredData.Add(beatmapData[i]);
            }
        }

        filteredData.RemoveAll(string.IsNullOrEmpty); // Usun puste
        return filteredData;
    }
}
