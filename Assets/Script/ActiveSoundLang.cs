using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSoundLang : MonoBehaviour
{
    void Start()
    {
        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(0);
                audioManager.PlayBackgroundMusic(0);
                audioManager.PlayEnvironmentMusic(0);
            }
            else
            {
                Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'AudioManager'.");
        }
    }


}
