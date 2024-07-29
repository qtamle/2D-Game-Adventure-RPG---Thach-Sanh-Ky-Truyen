using System.Collections;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject bullet;         
    public Transform bulletPos;        
    public float detectionRadius = 5f; 
    public LayerMask playerLayer;      
    public float shootInterval = 2f;   

    private float shootTimer = 0f;    
    private bool isPlayerInRange = false;

    private EnemyMove enemyMove;

    private void Start()
    {   
        enemyMove = GetComponent<EnemyMove>();
    }
    private void Update()
    {
        // Kiểm tra xem Player có nằm trong bán kính phát hiện không
        isPlayerInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (isPlayerInRange)
        {
            if (enemyMove != null)
            {
                enemyMove.enabled = false;
            }
            shootTimer += Time.deltaTime;

            if (shootTimer >= shootInterval)
            {
                shootTimer = 0f; 
                Shoot();
            }
        }
        else
        {
            if (enemyMove != null && !enemyMove.enabled)
            {
                enemyMove.enabled = true;
            }
        }
    }

    private void Shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
