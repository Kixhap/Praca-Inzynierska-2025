using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreUpdater : MonoBehaviour
{
    public int score = 0;
    public int Combo = 0;
    public float currenthealth = 1;
    public Image healthbar;
    private TMP_Text comboCounter;

    private readonly GameObject[] objects = new GameObject[10];

    public Sprite[] sprites;

    void Awake()
    {
        healthbar = GameObject.Find("HPBar").GetComponent<Image>();
        sprites = Resources.LoadAll<Sprite>("Numbers");
        comboCounter = GameObject.Find("ComboCounter").GetComponent<TMP_Text>();

        int x = 1000000000;
        for (int i = 0; i <= 9; i++)
        {
            objects[i] = GameObject.Find(x.ToString());
            x /= 10;
        }
    }
    public void ScoreLiczenie(int kombo, int value, int Wynik, float hpchange)
    {
        switch (kombo)
        {
            case 0:
                Combo = 0; break;
            case 1:
                Combo += 1; break;
        }
        score = Wynik + (Combo * value);
        ScoreUpdate(score);
        ChangeHealth(hpchange, currenthealth);
        ComboCounterUpdater(Combo);
        KeepScore.Score = score;
        if (KeepScore.Maxcombo < Combo) { KeepScore.Maxcombo = Combo; }
    }
    void ScoreUpdate(int number)
    {
        string numberString = number.ToString().PadLeft(objects.Length, '0');
        if (sprites.Length == objects.Length)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (objects[i].TryGetComponent<UnityEngine.UI.Image>(out var imageComponent))
                {
                    int digit = int.Parse(numberString[i].ToString());
                    imageComponent.sprite = sprites[digit];
                }
            }
        }
    }

    private void ComboCounterUpdater(int number)
    {
        comboCounter.text = "x" + number.ToString();
    }
    private void ChangeHealth(float x, float health)
    {
        float maxHealth = 1;
        health += x;
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        currenthealth = health;
        healthbar.rectTransform.localScale = new(currenthealth,0.43f,0);
        if (currenthealth <= 0)
        {
            SceneManager.LoadScene(3);
        }
    }

}
