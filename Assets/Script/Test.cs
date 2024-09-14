using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform fireStartPoint;     // Điểm tạo lửa
    public Transform fireEndPoint;       // Điểm đích
    public ParticleSystem firePrefab;    // Prefab của lửa
    public float moveSpeed = 5f;         // Tốc độ di chuyển của lửa
    private bool isFireActive = false;   // Kiểm tra xem lửa có đang hoạt động không

    void Update()
    {
        // Kiểm tra phím nhấn, ở đây sử dụng phím "Q"
        if (Input.GetKeyDown(KeyCode.Q) && !isFireActive)
        {
            // Tạo lửa khi nhấn phím Q
            ShootFire();
        }
    }

    void ShootFire()
    {
        // Tạo lửa tại vị trí điểm bắt đầu
        ParticleSystem fireInstance = Instantiate(firePrefab, fireStartPoint.position, Quaternion.Euler(130f,0f,0f));

        // Gán coroutine di chuyển lửa
        StartCoroutine(MoveFire(fireInstance));
    }

    System.Collections.IEnumerator MoveFire(ParticleSystem fireInstance)
    {
        isFireActive = true;
        while (fireInstance != null && fireInstance.transform.position != fireEndPoint.position)
        {
            // Di chuyển lửa từ điểm A đến điểm B
            float step = moveSpeed * Time.deltaTime;
            fireInstance.transform.position = Vector3.MoveTowards(fireInstance.transform.position, fireEndPoint.position, step);

            // Kiểm tra nếu lửa đã đến đích
            if (Vector3.Distance(fireInstance.transform.position, fireEndPoint.position) < 0.001f)
            {
                // Hủy lửa sau khi đến đích
                fireInstance.Stop();
                Destroy(fireInstance.gameObject, fireInstance.main.duration);
            }

            yield return null;
        }
        isFireActive = false;
    }
}
