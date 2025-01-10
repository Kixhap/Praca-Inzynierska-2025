using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioFileReader
{
    AudioClip ToAudioClip();
    IEnumerator AnalyzeAudio(AudioClip clip);
    List<float> GetAmplitudes();
}
