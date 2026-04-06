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

    [Header("개별 스킬 전용 사물함")]
    public Dictionary<string, int> specificBonusDamage = new Dictionary<string, int>();
    public Dictionary<string, float> specificBonusSize = new Dictionary<string, float>();
    public Dictionary<string, int> specificBonusChains = new Dictionary<string, int>();
    public Dictionary<string, float> specificBonusDuration = new Dictionary<string, float>();
    public Dictionary<string, float> specificBonusTickRate = new Dictionary<string, float>();
    public Dictionary<string, float> specificBonusAngle = new Dictionary<string, float>();
    public Dictionary<string, float> specificBonusKnockback = new Dictionary<string, float>();
    public Dictionary<string, int> specificBonusSwords = new Dictionary<string, int>(); // 추가 검 개수
    public Dictionary<string, float> specificBonusRadius = new Dictionary<string, float>(); // 추가 반경

    [Header("글로벌 패시브 (생존/유틸)")]
    public int globalBonusMaxHP = 0;
    public int globalBonusDefense = 0;
    public float globalBonusMagneticRange = 0f;
    

    private void Awake() { instance = this; }
    private void Start()
    {
        UpdateExpUI();
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
        comboTextUI.text = "COMBO : " + comboScore.ToString();
    }
    public void GameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
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
        if (currentExp >= maxExp)
        {
            LevelUp();
        }
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
                foreach (SkillData skill in lotteryBox)
                {
                    totalWeight += skill.dropWeight;
                }
                int randomWeight = Random.Range(0, totalWeight);
                int currentWeight = 0;
                int selectedIndex = 0;
                for (int j = 0; j < lotteryBox.Count; j++)
                {
                    currentWeight += lotteryBox[j].dropWeight;
                    if (randomWeight < currentWeight)
                    {
                        selectedIndex = j; // 당첨!
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
        if (levelTextUI != null)
        {
            levelTextUI.text = "LV" + level.ToString();
        }
    }
}
