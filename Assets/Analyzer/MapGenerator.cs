using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator
{
    private List<Tuple<float, float, int>> beats = new List<Tuple<float, float, int>>();
    private int? lastLane;

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
            int? lane = GetLaneWithBias(beat.Item2);
            beatmapData.Add($"1;{timeMs};{lane}");
        }

        File.WriteAllLines(outputFilePath, beatmapData);
        Debug.Log($"Map generated: {outputFilePath}");
    }

    private int? GetLaneWithBias(float time)
    {
        if ((UnityEngine.Random.Range(0f, 1f) > 0.85f && lastLane != null) || (lastLane == 1))
        {
           lastLane = (lastLane == 1) ? -1 : 1;
           return lastLane;
        }
        else
        {
            return (Mathf.FloorToInt(time * 4) % 2 == 0) ? -1 : 1;
        }
    }
}