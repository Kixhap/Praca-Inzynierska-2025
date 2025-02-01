using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator
{
    private List<Tuple<float, float, int>> beats = new List<Tuple<float, float, int>>();

    public void SetSpikes(List<Tuple<float, float, int>> beats)
    {
        this.beats = beats;
    }

    public void GenerateMap(string outputFilePath)
    {
        List<string> beatmapData = new List<string> { "[Beatmap]" };

        foreach (var beat in beats)
        {
            int timeMs = (int)(beat.Item2 * 1000);
            int lane = GetLaneBasedOnBPM(beat.Item2);
            beatmapData.Add($"1;{timeMs};{lane}");
        }

        File.WriteAllLines(outputFilePath, beatmapData);
        Debug.Log($"Map generated: {outputFilePath}");
    }

    private int GetLaneBasedOnBPM(float time)
    {
        return (Mathf.FloorToInt(time * 4) % 2 == 0) ? -1 : 1;
    }
}