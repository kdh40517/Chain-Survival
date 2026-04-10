using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject optionPanel;       // 💡 옵션창 패널
    public Animator buttonGroupAnimator; // 💡 버튼들을 움직일 애니메이터 추가!

    public void ClickStartGame()
    {
        SceneManager.LoadScene("GameScenes");
    }

    // 💡 [옵션] 버튼 누를 때
    public void ClickOption()
    {
        optionPanel.SetActive(true); // 옵션창 켜기

        // 버튼 애니메이터의 IsOptionOpen 스위치를 켜서(true) 화면 밖으로 내보냄!
        if (buttonGroupAnimator != null)
            buttonGroupAnimator.SetBool("IsOptionOpen", true);
    }

    // 💡 [닫기] 버튼 누를 때
    public void ClickCloseOption()
    {
        optionPanel.SetActive(false); // 옵션창 끄기

        // 버튼 애니메이터의 IsOptionOpen 스위치를 꺼서(false) 화면 안으로 다시 부름!
        if (buttonGroupAnimator != null)
            buttonGroupAnimator.SetBool("IsOptionOpen", false);
    }

    public void ClickQuitGame()
    {
        Application.Quit();
    }
}