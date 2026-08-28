using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject optionPanel;
    public Animator buttonGroupAnimator;
    public AudioMixer myMixer;

    public void SetMasterVolume(float volume)
    {
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

    public void ClickOption()
    {
        optionPanel.SetActive(true);

        if (buttonGroupAnimator != null)
            buttonGroupAnimator.SetBool("IsOptionOpen", true);
    }

    public void ClickCloseOption()
    {
        optionPanel.SetActive(false);

        if (buttonGroupAnimator != null)
            buttonGroupAnimator.SetBool("IsOptionOpen", false);
    }

    public void ClickQuitGame()
    {
        Application.Quit();
    }
}