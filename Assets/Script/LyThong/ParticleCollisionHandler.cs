using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionHandler : MonoBehaviour
{
    public delegate void OnHitPlayerCallback();
    public OnHitPlayerCallback onHitPlayer;

    public float damage;
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>();

    void OnParticleCollision(GameObject other)
    {
        // Kiểm tra nếu đối tượng va chạm là Player và chưa bị sát thương
        if (other.CompareTag("Player") && !hitObjects.Contains(other))
        {
            hitObjects.Add(other); // Đánh dấu Player là đã bị sát thương

            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage, 0.5f, 0.65f, 0.1f);
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
