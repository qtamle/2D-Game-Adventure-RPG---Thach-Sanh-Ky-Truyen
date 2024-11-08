using UnityEngine;

public class TailSkill : MonoBehaviour
{
    public GameObject tailPrefab; 
    public float tailGrowTime = 1.5f;  
    public float tailRetractTime = 1.5f;  
    private Vector3 spawnPosition;  
    private Vector3 retractPosition; 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ActivateTailSkill(transform.position);
        }
    }

    public void ActivateTailSkill(Vector3 position)
    {
        spawnPosition = new Vector3(position.x, position.y - 20f, position.z);  

        GameObject tail = Instantiate(tailPrefab, spawnPosition, Quaternion.identity);

        retractPosition = tail.transform.position;

        StartCoroutine(GrowAndRetractTail(tail));
    }

    private System.Collections.IEnumerator GrowAndRetractTail(GameObject tail)
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = new Vector3(tail.transform.position.x, tail.transform.position.y + 15f, tail.transform.position.z);

        while (elapsedTime < tailGrowTime)
        {
            elapsedTime += Time.deltaTime;
            tail.transform.position = Vector3.Lerp(retractPosition, targetPosition, elapsedTime / tailGrowTime);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < tailRetractTime)
        {
            elapsedTime += Time.deltaTime;
            tail.transform.position = Vector3.Lerp(targetPosition, retractPosition, elapsedTime / tailRetractTime);
            yield return null;
        }

        Destroy(tail);
    }
}
