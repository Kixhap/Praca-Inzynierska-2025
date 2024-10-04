using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
public class DataProcessor : MonoBehaviour
{
    public static void Main(string filePath, string outputFilePath)
    {
        CultureInfo customCulture = new("pl-PL");
        customCulture.NumberFormat.NumberDecimalSeparator = ",";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        outputFilePath = Application.dataPath + outputFilePath;

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            bool isInGeneralSection = false;
            int modeValue = -1;

            foreach (var line in lines)
            {
                if (line.Trim() == "[General]")
                {
                    isInGeneralSection = true;
                }
                else if (isInGeneralSection)
                {
                    if (line.StartsWith("Mode:"))
                    {
                        if (int.TryParse(line["Mode:".Length..].Trim(), out modeValue))
                        {
                            break;
                        }
                    }
                }
            }

            switch (modeValue)
            {
                case 0:
                    ParseMode0(filePath, outputFilePath);
                    break;
                case 1:
                    ParseMode1(filePath, outputFilePath);
                    break;
                case 2:
                    ParseMode2(filePath, outputFilePath);
                    break;
                case 3:
                    ParseMode3(filePath, outputFilePath);
                    break;
                default:
                    Debug.LogError("Unsupported Mode value");
                    break;
                }
            }
        
            catch (Exception e)
            {
            Debug.LogError($"An error occurred: {e.Message}");
            }
    }

    private static void ParseMode0(string inputFilePath, string outputFilePath)// osuStd
    {
        List<string> outputLines = new();
        float lasttime = 0;
        float lastline = 0;
        try
        {
            string[] lines = File.ReadAllLines(inputFilePath);
            bool isBeatmapSection = false;

            foreach (string line in lines)
            {
                if (line.Trim() == "[HitObjects]")
                {
                    isBeatmapSection = true;
                    continue;
                }
                if (isBeatmapSection)
                {
                    string[] values = line.Split(',');

                    if (float.TryParse(values[2], out float time) &&
                    float.TryParse(values[1], out float linia))
                    {
                        if (lasttime != time || lastline!=linia)
                        {
                         if (384 - linia < 192) { linia = 1; } else { linia = -1; };
                         outputLines.Add($"1;{time};{linia}");lasttime = time; lastline = linia;
                        }
                    }
                }
            }
            outputLines.Insert(0, "[Beatmap]");
            File.WriteAllLines(outputFilePath, outputLines);
        }
        catch (Exception e)
        {
            Debug.LogError($"An error occurred while parsing: {e.Message}");
        }

    }

    private static void ParseMode1(string inputFilePath, string outputFilePath)// osuTaiko
    {
        List<string> outputLines = new();
        float Lasttime = 0;
        float Lastline = 0;
        try
        {
            string[] lines = File.ReadAllLines(inputFilePath);
            bool isBeatmapSection = false;

            foreach (string line in lines)
            {
                if (line.Trim() == "[HitObjects]")
                {
                    isBeatmapSection = true;
                    continue;
                }
                if (isBeatmapSection)
                {
                    string[] values = line.Split(',');

                    if (float.TryParse(values[2], out float time) &&
                        float.TryParse(values[4], out float linia))
                    {
                        if ((linia == Lastline ? 1 : 0) + (time == Lasttime ? 1 : 0) == 1)
                            {
                            if (linia < 8) { linia = 1; } else { linia = -1; };
                            outputLines.Add($"1;{time};{linia}");Lasttime = time; Lastline = linia;

                        }
                    }
                    else
                    {
                        Debug.LogError($"Error parsing values. Values: {string.Join(", ", values)}");
                    }
                }
            }
            outputLines.Insert(0, "[Beatmap]");
            File.WriteAllLines(outputFilePath, outputLines);
        }
        catch (Exception e)
        {
            Debug.LogError($"An error occurred while parsing: {e.Message}");
        }

    }
    private static void ParseMode2(string inputFilePath, string outputFilePath)// osuCatch
    {
        List<string> outputLines = new();
        float lasttime = 0;
        float lastline = 0;
        try
        {
            string[] lines = File.ReadAllLines(inputFilePath);
            bool isBeatmapSection = false;

            foreach (string line in lines)
            {
                if (line.Trim() == "[HitObjects]")
                {
                    isBeatmapSection = true;
                    continue;
                }
                if (isBeatmapSection)
                {
                    string[] values = line.Split(',');
                    if (float.TryParse(values[2], out float time) &&
                        float.TryParse(values[0], out float linia))
                    {
                        if ((linia == lastline ? 1 : 0) + (time == lasttime ? 1 : 0) <= 1)
                        {
                            if (512 - linia < 256) { linia = 1; } else { linia = -1; };
                            outputLines.Add($"1;{time};{linia}");
                            lasttime = time; lastline = linia;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Error parsing values. Values: {string.Join(", ", values)}");
                    }
                }
            }
            outputLines.Insert(0, "[Beatmap]");
            File.WriteAllLines(outputFilePath, outputLines);
        }
        catch (Exception e)
        {
            Debug.LogError($"An error occurred while parsing: {e.Message}");
        }
    }
    private static void ParseMode3(string inputFilePath, string outputFilePath)// osuMania
    {
        List<string> outputLines = new();
        float lasttime = 0;
        float lastline = 0;
        float sliderlast1=0, sliderlast2=0;
        try
        {
            string[] lines = File.ReadAllLines(inputFilePath);
            int columnCount = GetColumnCount(lines);
            bool isBeatmapSection = false;

            foreach (string line in lines)
            {
                if (line.Trim() == "[HitObjects]")
                {
                    isBeatmapSection = true;
                    continue;
                }
                if (isBeatmapSection)
                {
                    string[] values = line.Split(',');

                    if (int.TryParse(values[3], out int objType))
                    {
                        if (objType == 128)
                        {
                            int lineIndex;
                            string endTime;

                            if (int.TryParse(values[0], out int x) &&
                                float.TryParse(values[2], out float time))
                            {
                                if
                                    ((sliderlast1 <= time && time <= sliderlast2 ? 1 : 0) +
                                    (time == lasttime ? 1 : 0) +
                                    (((x * columnCount / 512 > (columnCount - 1) / 2) ? 1 : -1) == lastline ? 1 : 0) <=1 )

                                {
                                    endTime = values[5].Split(':')[0];

                                    lineIndex = (x * columnCount / 512 > (columnCount - 1) / 2) ? 1 : -1;
                                    outputLines.Add($"{objType};{time};{endTime};{lineIndex}");
                                    sliderlast1 = time;sliderlast2 = float.Parse(endTime);
                                    lastline = lineIndex;
                                }
                            }
                            else
                            {
                                Debug.LogError($"Error parsing values for Type 128. Values: {string.Join(", ", values)}");
                            }
                        }
                        else if (objType == 1)
                        {
                            int lineIndex;
                            if (int.TryParse(values[0], out int x) &&
                                float.TryParse(values[2], out float time))
                            {
                                if ((sliderlast1 <= time && time <= sliderlast2 ? 1 : 0) +
                                    (time == lasttime ? 1 : 0) +
                                    (((x * columnCount / 512 > (columnCount - 1) / 2) ? 1 : -1) == lastline ? 1 : 0) <= 1)
                                {
                                    lineIndex = (x * columnCount / 512 > (columnCount - 1) / 2) ? 1 : -1;
                                    outputLines.Add($"{objType};{time};{lineIndex}");
                                    lasttime = time;
                                    lastline= lineIndex;
                                }
                            }
                            else
                            {
                                Debug.LogError($"Error parsing values for Type 1. Values: {string.Join(", ", values)}");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"Error parsing objType. Values: {string.Join(", ", values)}");
                    }
                }
            }
            outputLines.Insert(0, "[Beatmap]");
            File.WriteAllLines(outputFilePath, outputLines);
        }
        catch (Exception e)
        {
            Debug.LogError($"An error occurred while parsing: {e.Message}");
        }
    }

    private static int GetColumnCount(string[] lines)
    {
        string difficultyLine = Array.Find(lines, line => line.StartsWith("CircleSize:"));
        int columnCount = int.Parse(Regex.Match(difficultyLine, @"\d+").Value);
        return columnCount;
    }
}