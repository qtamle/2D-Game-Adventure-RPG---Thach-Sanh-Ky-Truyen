using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{

    public Button nextButton;
    public Button backButton;

    void Start()
    {
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

}
