using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ScoreScreenHandler : MonoBehaviour
{
    private int score;
    private int maxcombo;
    public GameObject[] objects = new GameObject[10];
    public Sprite[] sprites;
    [SerializeField] TMP_Text comboCounter;
    void Awake()
    {
        sprites = Resources.LoadAll<Sprite>("Numbers");
        score = KeepScore.Score;
        maxcombo = KeepScore.Maxcombo;
        int x = 1000000000;
        for (int i = 0; i <= 9; i++)
        {
            objects[i] = GameObject.Find(x.ToString());
            x /= 10;
        }
    }

    public void Start()
    {
        ScoreUpdate(score);
        MaxcomboUpdate(maxcombo);
    }

    private void ScoreUpdate(int number)
    {
        string numberString = number.ToString().PadLeft(objects.Length, '0');
        if (sprites.Length == objects.Length)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (objects[i].TryGetComponent<Image>(out var imageComponent))
                {
                    int digit = int.Parse(numberString[i].ToString());
                    imageComponent.sprite = sprites[digit];
                }
            }
        }
    }
    private void MaxcomboUpdate(int maxcombo)
    {
        comboCounter.text = "x" + maxcombo.ToString();
    }
    public void Back()
    {
        SceneManager.LoadScene("Main Menu");
    }
}

