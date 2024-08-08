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

    public GameObject player;
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
                ChangeFace();
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

    private void ChangeFace()
    {
        Vector3 scale = transform.localScale;

        if (player.transform.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x) * -1;
        }else
        {
            scale.x = Mathf.Abs(scale.x);
        }

        transform.localScale = scale;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
