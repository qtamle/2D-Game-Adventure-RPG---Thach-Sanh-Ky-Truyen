using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{
    public Animator animator;
    private int currentIndex = 0;
    private int totalAnim = 5;

    public Button nextButton;
    public Button backButton;

    void Start()
    {
        nextButton.onClick.AddListener(NextAnimation);
        backButton.onClick.AddListener(BackAnimation);

        UpdateButtonInteractable(); // Cập nhật trạng thái nút ban đầu

    }

    void NextAnimation()
    {
        if (currentIndex < totalAnim - 1 ) {
            currentIndex++;
            animator.SetTrigger("Next");
            DisableButton();
        }
        
    }

    void BackAnimation()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            animator.SetTrigger("Back");
            DisableButton(); // Vô hiệu hóa nút tạm thời
        }
        else
        {
            Debug.Log("Back het duoc roi");
        }
    }

    void DisableButton()
    {
        nextButton.interactable = false;
        backButton.interactable = false;

        Invoke(nameof(EnableButton), 1f);
    }

    void EnableButton()
    {
        UpdateButtonInteractable(); // Bật lại các nút dựa trên trạng thái hiện tại
    }

    void UpdateButtonInteractable()
    {
        // Cập nhật trạng thái nút dựa trên currentIndex
        nextButton.interactable = currentIndex < totalAnim - 1;
        backButton.interactable = currentIndex > 0;
    }
}
