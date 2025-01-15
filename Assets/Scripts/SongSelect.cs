using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSelect : MonoBehaviour
{
    public static int id=0;
    public void Send(string variableToSend)
    {
         switch(variableToSend)
            {
                case "Generated":
                    id = 1;
                    break;
            }
        CopyAndReplaceFile("Assets/Beatmaps/" + variableToSend + "/map.txt", "Assets/Beatmaps/");
        CopyAndReplaceFile("Assets/Beatmaps/" + variableToSend + "/song.wav", "Assets/Beatmaps/");
    }

    void CopyAndReplaceFile(string sourcePath, string destinationFolder)
    {
        string fileName = Path.GetFileName(sourcePath);
        string destinationPath = Path.Combine(destinationFolder, fileName);

        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }
        File.Copy(sourcePath, destinationPath);

        SceneManager.LoadScene("InGame");
    }

}