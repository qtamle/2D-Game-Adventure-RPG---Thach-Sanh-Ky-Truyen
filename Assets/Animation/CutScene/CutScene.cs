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

    public void CotTruyen()
    {
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

    public void Lang()
    {
        SceneManager.LoadScene("Làng");
    }

    public void CocTinh()
    {
        SceneManager.LoadScene("Toad 1");
    }
}
