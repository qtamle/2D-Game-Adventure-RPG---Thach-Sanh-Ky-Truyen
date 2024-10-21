using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class CoinsData
{
    public float coins;
}
public class CoinManager : MonoBehaviour
{
    private static string coinsFilePath = Application.dataPath + "/saveload/coins.json";

    public float coins;

    private void Start()
    {
        LoadCoins();
    }

    public void SaveCoins()
    {
        CoinsData data = new CoinsData { coins = coins };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(coinsFilePath, json);
        Debug.Log("Coins saved to: " + coinsFilePath);
    }

    public void LoadCoins()
    {
        if (File.Exists(coinsFilePath))
        {
            string json = File.ReadAllText(coinsFilePath);
            CoinsData data = JsonUtility.FromJson<CoinsData>(json);
            coins = data.coins;
            Debug.Log("Coins loaded: " + coins);
        }
        else
        {
            Debug.LogWarning("Coins file not found: " + coinsFilePath);
        }
    }
}
