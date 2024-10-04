using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public AudioMixer main;

    public Slider Hit;
    public Slider Music;
    public Slider Master;

    public KeyCode t1Key;
    public KeyCode t2Key;
    public KeyCode b1Key;
    public KeyCode b2Key;

    [SerializeField]
    private GameObject T1;
    [SerializeField]
    private GameObject T2;
    [SerializeField]
    private GameObject B1;
    [SerializeField]
    private GameObject B2;

    private Dictionary<string, KeyCode> specialCharacterMap;
    private const string SMasterVolume = "MasterVolume";
    private const string SHitVolume = "HitVolume";
    private const string SMusicVolume = "MusicVolume";
    private const string T1Key = "T1Key";
    private const string T2Key = "T2Key";
    private const string B1Key = "B1Key";
    private const string B2Key = "B2Key";

    private void Start()
    {

        Hit.value = PlayerPrefs.GetFloat(SHitVolume, 0);
        Master.value = PlayerPrefs.GetFloat(SMasterVolume, 0);
        Music.value = PlayerPrefs.GetFloat(SMusicVolume, 0);

        t1Key = (KeyCode)PlayerPrefs.GetInt(T1Key, (int)KeyCode.Z);
        t2Key = (KeyCode)PlayerPrefs.GetInt(T2Key, (int)KeyCode.X);
        b1Key = (KeyCode)PlayerPrefs.GetInt(B1Key, (int)KeyCode.Period);
        b2Key = (KeyCode)PlayerPrefs.GetInt(B2Key, (int)KeyCode.Slash);
        InitializeSpecialCharacterMap();
    }

    public void MainVolume(float volume)
    {
        main.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat(SMasterVolume, volume);
    }

    public void HitVolume(float volume)
    {
        main.SetFloat("HitVolume", volume);
        PlayerPrefs.SetFloat(SHitVolume, volume);
    }

    public void MusicVolume(float volume)
    {
        main.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat(SMusicVolume, volume);
    }

    private void InitializeSpecialCharacterMap()
    {
        specialCharacterMap = new Dictionary<string, KeyCode>
        {
            {"!", KeyCode.Exclaim},
            {"@", KeyCode.At},
            {"#", KeyCode.Hash},
            {"$", KeyCode.Dollar},
            {"%", KeyCode.Percent},
            {"^", KeyCode.Caret},
            {"&", KeyCode.Ampersand},
            {"*", KeyCode.Asterisk},
            {"(", KeyCode.LeftParen},
            {")", KeyCode.RightParen},
            {"_", KeyCode.Underscore},
            {"+", KeyCode.Plus},
            {"-", KeyCode.Minus},
            {"=", KeyCode.Equals},
            {"[", KeyCode.LeftBracket},
            {"]", KeyCode.RightBracket},
            {"{", KeyCode.LeftCurlyBracket},
            {"}", KeyCode.RightCurlyBracket},
            {";", KeyCode.Semicolon},
            {":", KeyCode.Colon},
            {"'", KeyCode.Quote},
            {"\"", KeyCode.DoubleQuote},
            {",", KeyCode.Comma},
            {".", KeyCode.Period},
            {"<", KeyCode.Less},
            {">", KeyCode.Greater},
            {"/", KeyCode.Slash},
            {"?", KeyCode.Question}
        };
    }

    private TMP_InputField GetInputFieldByKey(int key)
    {
        return key switch
        {
            0 => T1.GetComponent<TMP_InputField>(),
            1 => T2.GetComponent<TMP_InputField>(),
            2 => B1.GetComponent<TMP_InputField>(),
            3 => B2.GetComponent<TMP_InputField>(),
            _ => null,
        };
    }
    private string GetCode(int key)
    {
        return key switch
        {
            0 => T1Key,
            1 => T2Key,
            2 => B1Key,
            3 => B2Key,
            _ => null,
        };
    }

    public void Change(int Key)
    {
        if (specialCharacterMap.TryGetValue(GetInputFieldByKey(Key).text, out KeyCode value))
        {
            PlayerPrefs.SetInt(GetCode(Key), (int)value);
        }
        else
        {
            Enum.TryParse(GetInputFieldByKey(Key).text.ToUpper(), out value);
            PlayerPrefs.SetInt(GetCode(Key), (int)value);
        }
    }

    public void SaveSettings()
    {
        Debug.Log((KeyCode)PlayerPrefs.GetInt(T1Key));
        Debug.Log((KeyCode)PlayerPrefs.GetInt(T2Key));
        Debug.Log((KeyCode)PlayerPrefs.GetInt(B1Key));
        Debug.Log((KeyCode)PlayerPrefs.GetInt(B2Key));
        PlayerPrefs.Save();
    }

    public void DefaultSettings()
    {
        PlayerPrefs.SetInt(T1Key, (int)KeyCode.Z);
        PlayerPrefs.SetInt(T2Key, (int)KeyCode.X);
        PlayerPrefs.SetInt(B1Key, (int)KeyCode.Period);
        PlayerPrefs.SetInt(B2Key, (int)KeyCode.Slash);
    }
}