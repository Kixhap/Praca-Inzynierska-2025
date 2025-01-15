using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioFileReader
{
    AudioClip ToAudioClip();
    IEnumerator AnalyzeAudio (AudioClip clip);
    List<Tuple<float, float, int>> GetSpikes();
}
