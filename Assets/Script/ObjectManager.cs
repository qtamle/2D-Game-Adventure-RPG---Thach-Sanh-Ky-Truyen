using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public List<GameObject> objectsToRemove = new List<GameObject>();

    public void RemoveAllObjects()
    {
        foreach (GameObject obj in objectsToRemove)
        {
            if (obj != null && obj.activeInHierarchy) 
            {
                Destroy(obj);
                Debug.Log("Đã hủy GameObject: " + obj.name);
            }
        }
    }
}
