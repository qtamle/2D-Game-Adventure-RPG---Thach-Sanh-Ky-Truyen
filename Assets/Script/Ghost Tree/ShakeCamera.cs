using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class ShakeCamera : MonoBehaviour
{
    public ShakeData deathShake;

    public void DeathShake()
    {
        CameraShakerHandler.Shake(deathShake);
    }
}
