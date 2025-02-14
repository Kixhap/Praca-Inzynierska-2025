using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSelect : MonoBehaviour
{
    public static int id = 0;

    public void Send(string variableToSend)
    {
        switch (variableToSend)
        {
            case "Generated":
                id = 1;
                break;
        }

        string beatmapsPath = "Assets/Beatmaps/";
        string sourceFolder = Path.Combine(beatmapsPath, variableToSend);

        // Kopiowanie mapy
        string mapSource = Path.Combine(sourceFolder, "map.txt");
        if (File.Exists(mapSource))
        {
            CopyAndReplaceFile(mapSource, beatmapsPath);
        }
        else
        {
            Debug.LogError($"Map file not found in: {sourceFolder}");
            return;
        }

        // Kopiowanie pliku audio z obs³ug¹ ró¿nych formatów
        string[] audioExtensions = { ".wav", ".mp3" };
        bool audioCopied = false;

        foreach (string extension in audioExtensions)
        {
            string audioSource = Path.Combine(sourceFolder, "song" + extension);
            if (File.Exists(audioSource))
            {
                CopyAndReplaceFile(audioSource, beatmapsPath);
                audioCopied = true;
                break;
            }
        }

        if (!audioCopied)
        {
            Debug.LogError($"No audio file found in: {sourceFolder}");
            return;
        }

        SceneManager.LoadScene("InGame");
    }

    void CopyAndReplaceFile(string sourcePath, string destinationFolder)
    {
        try
        {
            string fileName = Path.GetFileName(sourcePath);
            string destinationPath = Path.Combine(destinationFolder, fileName);

            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            File.Copy(sourcePath, destinationPath);
            Debug.Log($"Successfully copied: {sourcePath} to {destinationPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"File copy failed: {e.Message}");
        }
    }
}