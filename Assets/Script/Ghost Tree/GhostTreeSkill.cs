using System.Collections;
using UnityEngine;

public class GhostTreeSkill : MonoBehaviour
{
    [Header("Tie")]
    public GameObject vineObject;
    public GameObject warningPrefab;
    public float timeAppears = 1.5f;
    public float warningDuration = 1f;

    [Header("Hands")]
    public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;
    public Transform leftHandStartPosition;
    public Transform rightHandStartPosition;
    public float handMovementDuration = 2f;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnVine());
        StartCoroutine(MoveHands());
    }

    private IEnumerator SpawnVine()
    {
        Vector3 spawnWarning = playerTransform.position;
        GameObject warning = Instantiate(warningPrefab, spawnWarning, Quaternion.identity);

        yield return new WaitForSeconds(warningDuration);

        Destroy(warning);

        yield return new WaitForSeconds(timeAppears);

        Vector3 spawn = spawnWarning;
        GameObject vine = Instantiate(vineObject, spawn, Quaternion.identity);
    }

    private IEnumerator MoveHands()
    {
        // Instantiate hands
        GameObject leftHand = Instantiate(leftHandPrefab, leftHandStartPosition.position, Quaternion.identity);
        GameObject rightHand = Instantiate(rightHandPrefab, rightHandStartPosition.position, Quaternion.identity);

        Vector3 leftHandStart = leftHandStartPosition.position;
        Vector3 rightHandStart = rightHandStartPosition.position;
        Vector3 leftHandEnd = playerTransform.position + Vector3.left * 5f; // Adjust position
        Vector3 rightHandEnd = playerTransform.position + Vector3.right * 5f; // Adjust position

        float elapsedTime = 0f;

        while (elapsedTime < handMovementDuration)
        {
            float t = elapsedTime / handMovementDuration;

            // Move hands
            leftHand.transform.position = Vector3.Lerp(leftHandStart, leftHandEnd, t);
            rightHand.transform.position = Vector3.Lerp(rightHandStart, rightHandEnd, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure hands end up at their final positions
        leftHand.transform.position = leftHandEnd;
        rightHand.transform.position = rightHandEnd;

        // Move hands back to start positions
        elapsedTime = 0f;

        while (elapsedTime < handMovementDuration)
        {
            float t = elapsedTime / handMovementDuration;

            // Move hands back
            leftHand.transform.position = Vector3.Lerp(leftHandEnd, leftHandStart, t);
            rightHand.transform.position = Vector3.Lerp(rightHandEnd, rightHandStart, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure hands return to their starting positions
        leftHand.transform.position = leftHandStart;
        rightHand.transform.position = rightHandStart;
    }
}
