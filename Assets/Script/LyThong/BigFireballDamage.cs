using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigFireballDamage : MonoBehaviour
{
    public delegate void OnHitPlayerCallback();
    public OnHitPlayerCallback onHitPlayer;

    public float damage;
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>();

    private void Start()
    {
        int myLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");

        Physics2D.IgnoreLayerCollision(myLayer, playerLayer, true);
    }
    void OnParticleCollision(GameObject other)
    {
        // Kiểm tra nếu đối tượng va chạm là Player và chưa bị sát thương
        if (other.CompareTag("Player") && !hitObjects.Contains(other))
        {
            hitObjects.Add(other); // Đánh dấu Player là đã bị sát thương

            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            StatusEffects status = other.GetComponentInChildren<StatusEffects>();
            if (player != null)
            {
                player.TakeDamage(damage, 0.5f, 0.65f, 0.1f);
                status.ApplyBleed();
            }

            Debug.Log("Particle collided with Player!");

            onHitPlayer?.Invoke();
        }
    }

    public void ResetCollision()
    {
        hitObjects.Clear();
    }
}
