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
                case "Airman":
                    id = 0;
                    break;
                case "Bad":
                    id = 1;
                    break;
                case "Night":
                    id = 2;
                    break;
                case "secret":
                    id = 3;
                    break;
            }
        CopyAndReplaceFile("Assets/Beatmaps/" + variableToSend + "/map.txt", "Assets/Beatmaps/");
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