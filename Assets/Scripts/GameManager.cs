using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int comboScore = 0;
    public TMP_Text comboTextUI;
    public GameObject gameOverUI;
    public TMP_Text finalScoreTextUI;
    public int level = 1;
    public int currentExp = 0;
    public int maxExp = 50;
    public Slider expBar;
    public TMP_Text levelTextUI;
    public GameObject levelUpPanel;
    public SkillData[] allSkills;
    public SkillCardUI[] skillCards;
    public AudioMixer myMixer;

    [Header("UI 패널 연결 (일시정지/옵션)")]
    public GameObject pauseMenuPanel;
    public GameObject optionPanel;
    private bool isPaused = false;

    [Header("개별 스킬 전용 사물함")]
    public Dictionary<string, int> specificBonusDamage = new Dictionary<string, int>();
    public Dictionary<string, float> specificBonusSize = new Dictionary<string, float>();
    public Dictionary<string, int> specificBonusChains = new Dictionary<string, int>();
    public Dictionary<string, float> specificBonusDuration = new Dictionary<string, float>();
    public Dictionary<string, float> specificBonusTickRate = new Dictionary<string, float>();
    public Dictionary<string, float> specificBonusAngle = new Dictionary<string, float>();
    public Dictionary<string, float> specificBonusKnockback = new Dictionary<string, float>();
    public Dictionary<string, int> specificBonusSwords = new Dictionary<string, int>();
    public Dictionary<string, float> specificBonusRadius = new Dictionary<string, float>();

    [Header("글로벌 패시브 (생존/유틸)")]
    public int globalBonusMaxHP = 0;
    public int globalBonusDefense = 0;
    public float globalBonusMagneticRange = 0f;

    private void Awake() { instance = this; }
    private void Start()
    {
        UpdateExpUI();
    }

    private void Update()
    {
        if (gameOverUI != null && gameOverUI.activeSelf) return;
        if (levelUpPanel != null && levelUpPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionPanel != null && optionPanel.activeSelf)
            {
                CloseOption();
            }
            else
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenOption()
    {
        if (optionPanel != null) optionPanel.SetActive(true);
    }

    public void CloseOption()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("[Game] 종료");
        Application.Quit();
    }
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

    public void UpgradeSpecificSkill(string id, int dmg, float size, int chains, float duration, float tickRate, float angle, float knockback, int extraSwords, float radius)
    {
        if (!specificBonusDamage.ContainsKey(id)) specificBonusDamage[id] = 0;
        if (!specificBonusSize.ContainsKey(id)) specificBonusSize[id] = 0f;
        if (!specificBonusChains.ContainsKey(id)) specificBonusChains[id] = 0;
        if (!specificBonusDuration.ContainsKey(id)) specificBonusDuration[id] = 0f;
        if (!specificBonusTickRate.ContainsKey(id)) specificBonusTickRate[id] = 0f;
        if (!specificBonusAngle.ContainsKey(id)) specificBonusAngle[id] = 0f;
        if (!specificBonusKnockback.ContainsKey(id)) specificBonusKnockback[id] = 0f;
        if (!specificBonusSwords.ContainsKey(id)) specificBonusSwords[id] = 0;
        if (!specificBonusRadius.ContainsKey(id)) specificBonusRadius[id] = 0f;

        specificBonusDamage[id] += dmg;
        specificBonusSize[id] += size;
        specificBonusChains[id] += chains;
        specificBonusDuration[id] += duration;
        specificBonusTickRate[id] += tickRate;
        specificBonusAngle[id] += angle;
        specificBonusKnockback[id] += knockback;
        specificBonusSwords[id] += extraSwords;
        specificBonusRadius[id] += radius;
    }

    public void AddCombo()
    {
        comboScore++;
        comboTextUI.text = "KillScore : " + comboScore.ToString();
    }

    public void GameOver()
    {
        if (gameOverUI != null) gameOverUI.SetActive(true);

        if (finalScoreTextUI != null)
        {
            finalScoreTextUI.text = "Score : " + comboScore.ToString();
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= maxExp) LevelUp();
        UpdateExpUI();
    }

    private void LevelUp()
    {
        level++;
        currentExp -= maxExp;
        maxExp = Mathf.FloorToInt(maxExp * 1.5f);
        Debug.Log($"[Level] 레벨 업 - {level}");
        if (levelUpPanel != null) levelUpPanel.SetActive(true);
        Time.timeScale = 0f;

        ChainManager chain = GameObject.Find("Player").GetComponent<ChainManager>();
        bool hasEmptySlot = false;
        List<GameObject> equippedPrefabs = new List<GameObject>();
        List<string> equippedSkillIds = new List<string>();

        if (chain != null)
        {
            foreach (GameObject slot in chain.skillSlots)
            {
                if (slot == null) hasEmptySlot = true;
                else
                {
                    equippedPrefabs.Add(slot);
                    SkillEffect effect = slot.GetComponent<SkillEffect>();
                    if (effect != null) equippedSkillIds.Add(effect.skillId);
                }
            }
        }

        List<SkillData> availablePool = new List<SkillData>();
        foreach (SkillData skill in allSkills)
        {
            if (skill.cardType == CardType.ActiveSkill)
            {
                if (equippedPrefabs.Contains(skill.skillPrefab)) continue;
                if (!hasEmptySlot) continue;
            }
            else if (skill.cardType == CardType.SkillUpgrade)
                if (!equippedSkillIds.Contains(skill.targetSkillId)) continue;
            availablePool.Add(skill);
        }

        List<SkillData> lotteryBox = new List<SkillData>(availablePool);
        for (int i = 0; i < skillCards.Length; i++)
        {
            if (lotteryBox.Count > 0)
            {
                int totalWeight = 0;
                foreach (SkillData skill in lotteryBox) totalWeight += skill.dropWeight;
                int randomWeight = Random.Range(0, totalWeight);
                int currentWeight = 0;
                int selectedIndex = 0;
                for (int j = 0; j < lotteryBox.Count; j++)
                {
                    currentWeight += lotteryBox[j].dropWeight;
                    if (randomWeight < currentWeight)
                    {
                        selectedIndex = j;
                        break;
                    }
                }
                SkillData pickedSkill = lotteryBox[selectedIndex];
                skillCards[i].SetUpCard(pickedSkill);
                lotteryBox.RemoveAt(selectedIndex);
            }
            else skillCards[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < skillCards.Length; i++)
        {
            if (i < availablePool.Count) skillCards[i].gameObject.SetActive(true);
        }
    }

    public void CloseLevelUpUI()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void UpdateExpUI()
    {
        if (expBar != null)
        {
            expBar.maxValue = maxExp;
            expBar.value = currentExp;
        }
        if (levelTextUI != null) levelTextUI.text = "LV" + level.ToString();
    }
}