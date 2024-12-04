using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBoss : MonoBehaviour
{
    private SaveBoss saveboss;
    public string bossName;

    public GameObject[] hideObjects;
    public GameObject[] showObjects;
    public GameObject[] hideOthers;

    private void Start()
    {
        saveboss = FindObjectOfType<SaveBoss>();
        CheckGhostTree();
        CheckGhostTreeShow();
        HideObjects();
    }

    private void CheckGhostTree()
    {
        foreach (GameObject go in hideObjects)
        {
            if (!saveboss.IsBossDefeated(bossName))
            {
                go.SetActive(false);
            }
        }
    }

    private void CheckGhostTreeShow()
    {
        foreach (GameObject go in hideObjects)
        {
            if (saveboss.IsBossDefeated(bossName))
            {
                go.SetActive(true);
            }
        }
    }

    private void HideObjects()
    {
        foreach (GameObject go in hideOthers)
        {
            go.SetActive(false);
        }
    }
}
