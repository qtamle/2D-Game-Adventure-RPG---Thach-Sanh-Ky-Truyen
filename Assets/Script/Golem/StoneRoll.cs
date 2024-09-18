using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneRoll : MonoBehaviour
{
    private void Start()
    {
        int stoneLayer = LayerMask.NameToLayer("StoneRoll");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");
        int turnOnLayer = LayerMask.NameToLayer("TurnOn");

        Physics2D.IgnoreLayerCollision(stoneLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(stoneLayer, playerLayer, true);
        Physics2D.IgnoreLayerCollision(stoneLayer, turnOnLayer, true);
    }
}
