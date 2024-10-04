using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Managinginputs : MonoBehaviour
{
    private int InputAcount;
    private int InputBcount;
    private TMP_Text InputT;
    private TMP_Text InputB;

    private SpriteRenderer SPTop;
    private SpriteRenderer SPBot;
    private Image SPCTop;
    private Image SPCBot;

    private HitHandler TopHH;
    private HitHandler BotHH;

    [SerializeField]
    private KeyCode T1;
    [SerializeField]
    private KeyCode T2;
    [SerializeField]
    private KeyCode B1;
    [SerializeField]
    private KeyCode B2;

    [SerializeField]
    private AudioSource HitSound;
    [SerializeField]
    Sprite SpriteTop;
    [SerializeField]
    Sprite SpriteTopPressed;
    [SerializeField]
    Sprite SpriteCounterTop;
    [SerializeField]
    Sprite SpriteCounterTopPressed;
    [SerializeField]
    Sprite SpriteBottom;
    [SerializeField]
    Sprite SpriteBottomPressed;
    [SerializeField]
    Sprite SpriteCounterBottom;
    [SerializeField]
    Sprite SpriteCounterBottomPressed;

    [SerializeField]
    ParticleSystem particletop;
    [SerializeField]
    ParticleSystem particletop2;
    [SerializeField]
    ParticleSystem particlebottom;
    [SerializeField]
    ParticleSystem particlebottom2;




    private void Start()
    {
        T1 = (KeyCode)PlayerPrefs.GetInt("T1Key", (int)KeyCode.X);
        T2 = (KeyCode)PlayerPrefs.GetInt("T2Key", (int)KeyCode.Z);
        B1 = (KeyCode)PlayerPrefs.GetInt("B1Key", (int)KeyCode.Period);
        B2 = (KeyCode)PlayerPrefs.GetInt("B2Key", (int)KeyCode.Slash);
        Debug.Log(T1 + " " + T2);
        Debug.Log(B1 + " " + B2);


        SPCTop = GameObject.Find("InputACounterBG").GetComponent<Image>();
        SPCBot = GameObject.Find("InputBCounterBG").GetComponent<Image>();
        SPTop = GameObject.Find("TLineCrosshair").GetComponent<SpriteRenderer>();
        SPBot = GameObject.Find("BLineCrosshair").GetComponent<SpriteRenderer>();
        InputT = GameObject.Find("InputACounter").GetComponent<TMP_Text>();
        InputB = GameObject.Find("InputBCounter").GetComponent<TMP_Text>();
        TopHH = SPTop.GetComponent<HitHandler>();
        BotHH = SPBot.GetComponent<HitHandler>();
    }
    void Update()
    {
        if (Input.GetKeyDown(T2))
        {
            TopLineClick();
            TopHH.CheckForNotes(T2);
        }
        if (Input.GetKeyDown(B2))
        {
            BottomLineClick();
            TopHH.CheckForNotes(B2);
        }
        if (Input.GetKeyDown(T1))
        {
            TopLineClick();
            BotHH.CheckForNotes(T1);
        }
        if (Input.GetKeyDown(B1))
        {
            BottomLineClick();
            BotHH.CheckForNotes(B1);
        }
        if (Input.GetKeyUp(T1) || Input.GetKeyUp(T2))
        {
            TopLineRelease();
        }
        if (Input.GetKeyUp(B1) || Input.GetKeyUp(B2))
        {
            BottomLineRelease();
        }
    }
    public void TopLineClick()
    {
        HitSound.Play();
        SPTop.sprite = SpriteTopPressed;
        InputAcount += 1;
        InputT.text = InputAcount.ToString();
        SPCTop.sprite = SpriteCounterTopPressed;
        particletop.Play();
        particletop2.Play();
    }
    public void BottomLineClick()
    {
        HitSound.Play();
        SPBot.sprite = SpriteBottomPressed;
        InputBcount += 1;
        InputB.text = InputBcount.ToString();
        SPCBot.sprite = SpriteCounterBottomPressed;
        particlebottom.Play();
        particlebottom2.Play();
    }
    public void TopLineRelease()
    {
        particletop.Stop();
        particletop2.Stop();
        SPTop.sprite = SpriteTop;
        SPCTop.sprite = SpriteCounterTop;
    }
    public void BottomLineRelease()
    {
        particlebottom.Stop();
        particlebottom2.Stop();
        SPBot.sprite = SpriteBottom;
        SPCBot.sprite = SpriteCounterBottom;
    }
}
