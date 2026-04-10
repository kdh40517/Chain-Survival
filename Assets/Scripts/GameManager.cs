using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int comboScore = 0;
    public TMP_Text comboTextUI;
    public GameObject gameOverUI;
    public int level = 1;
    public int currentExp = 0;
    public int maxExp = 50;
    public Slider expBar;
    public TMP_Text levelTextUI;
    public GameObject levelUpPanel;
    public SkillData[] allSkills;
    public SkillCardUI[] skillCards;

    [Header("UI 패널 연결 (일시정지/옵션)")]
    public GameObject pauseMenuPanel; // 💡 새로 추가: 일시정지 메뉴 패널
    public GameObject optionPanel;    // 💡 새로 추가: 옵션 메뉴 패널
    private bool isPaused = false;    // 💡 게임이 멈췄는지 기억하는 변수

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

    // 🚨 매 프레임마다 ESC 키를 감시하는 Update 함수 추가!
    private void Update()
    {
        // 1. 게임 오버나 레벨업 창이 떠있을 때는 ESC 무시하기 (버그 방지)
        if (gameOverUI != null && gameOverUI.activeSelf) return;
        if (levelUpPanel != null && levelUpPanel.activeSelf) return;

        // 2. ESC 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 만약 옵션창이 켜져있다면, 게임으로 안 돌아가고 '옵션창만' 닫기 (자연스러운 뒤로가기)
            if (optionPanel != null && optionPanel.activeSelf)
            {
                CloseOption();
            }
            else // 옵션창이 없다면 일시정지 켜기/끄기
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    // ================= [일시정지 시스템 함수들] =================

    // 💡 [계속하기] 버튼에 연결할 함수
    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);
        Time.timeScale = 1f; // 시간 다시 흐르게
        isPaused = false;
    }

    // ESC 눌렀을 때 실행되는 일시정지 함수
    public void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // 시간 멈추기
        isPaused = true;
    }

    // 💡 [옵션] 버튼에 연결할 함수
    public void OpenOption()
    {
        if (optionPanel != null) optionPanel.SetActive(true);
    }

    // 💡 [닫기] 버튼에 연결할 함수
    public void CloseOption()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
    }

    // 💡 [메인 메뉴로] 버튼에 연결할 함수
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // 씬 넘어가기 전에 무조건 시간 원상복구!
        SceneManager.LoadScene("MainMenu");
    }

    // 💡 [게임 종료] 버튼에 연결할 함수
    public void QuitGame()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }

    // ============================================================

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
        Debug.Log("🎉 레벨 업!! 현재 레벨: " + level);
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