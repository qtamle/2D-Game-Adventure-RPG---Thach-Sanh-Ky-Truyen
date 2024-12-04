using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{

    void Start()
    {
    }

    public void CotTruyen(int timelineIndex)
    {
        // Lưu chỉ số Timeline vào PlayerPrefs
        PlayerPrefs.SetInt("TimelineIndex", timelineIndex);
        PlayerPrefs.Save();

        // Chuyển sang scene Story1
        SceneManager.LoadScene("Story1");
    }

    public void MoDau()
    {
        SceneManager.LoadScene("Làng");
    }
    public void ChanTinh()
    {
        SceneManager.LoadScene("Python Remake 1");
    }
    public void DaiBang()
    {
        SceneManager.LoadScene("Remake Eagle");
    }
    public void LiThong()
    {
        SceneManager.LoadScene("Final Boss");
    }
    public void LiThongPhase2()
    {
        SceneManager.LoadScene("Final Boss Phase 2");
    }

    public void Lang()
    {
        SceneManager.LoadScene("Làng");
    }

    public void CocTinh()
    {
        SceneManager.LoadScene("Toad 1");
    }
}
