using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;
public class CameraShake1 : MonoBehaviour
{
    public ShakeData spikeShake;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            CameraShakerHandler.Shake(spikeShake);
        }
    }

}
