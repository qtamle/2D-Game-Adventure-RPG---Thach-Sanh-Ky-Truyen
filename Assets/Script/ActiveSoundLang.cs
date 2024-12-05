using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSoundLang : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlaySFX(0);
        AudioManager.Instance.PlayBackgroundMusic(0);
        AudioManager.Instance.PlayEnvironmentMusic(0);

    }
}
