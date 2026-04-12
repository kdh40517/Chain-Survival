using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject optionPanel;       // 💡 옵션창 패널
    public Animator buttonGroupAnimator; // 💡 버튼들을 움직일 애니메이터 추가!
    public AudioMixer myMixer;

    // 슬라이더 값이 0.0001 ~ 1 사이일 때 작동하는 함수들
    public void SetMasterVolume(float volume)
    {
        // 소수점 0~1 값을 데시벨(-80~0)로 변환
        myMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
    }

    public void SetBGMVolume(float volume)
    {
        myMixer.SetFloat("BGMVol", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        myMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
    }
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