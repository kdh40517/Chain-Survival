using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardUI : MonoBehaviour
{
    [Header("UI 연결 부품들")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descText;

    [Header("등급별 배경 Sprite (픽셀아트)")]
    public Sprite normalBackground;
    public Sprite rareBackground;
    public Sprite legendaryBackground;
    public Sprite activeBackground;

    [Header("색깔을 칠할 이미지")]
    public Image borderImage;

    private SkillData mySkillData;
      public void SetUpCard(SkillData data)
    {
        mySkillData = data;
        iconImage.sprite = data.skillIcon;
        nameText.text = data.skillName;
        descText.text = data.skillDescription;

        if (data.cardType == CardType.ActiveSkill)
        {
            if (activeBackground != null) borderImage.sprite = activeBackground;
        }
        else
        {
            switch (data.rarity)
            {
                case CardRarity.Normal:
                    if (normalBackground != null) borderImage.sprite = normalBackground;
                    break;
                case CardRarity.Rare:
                    if (rareBackground != null) borderImage.sprite = rareBackground;
                    break;
                case CardRarity.Legendary:
                    if (legendaryBackground != null) borderImage.sprite = legendaryBackground;
                    break;
            }
        }

        if (borderImage != null) borderImage.color = Color.white;
    }
    public void OnClickCard()
    {
        Debug.Log($"[Card] 선택 - {mySkillData.skillName}");
        if (mySkillData.cardType == CardType.ActiveSkill)
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                ChainManager chain = player.GetComponent<ChainManager>();
                for (int i = 0; i < chain.skillSlots.Length; i++)
                {
                    if (chain.skillSlots[i] == null)
                    {
                        chain.skillSlots[i] = mySkillData.skillPrefab;
                        Debug.Log($"[Card] {i + 1}단 슬롯에 {mySkillData.skillName} 장착");

                        break;
                    }
                }
            }
        }
        else if (mySkillData.cardType == CardType.PassiveStat)
        {
            GameManager.instance.globalBonusMaxHP += mySkillData.bonusMaxHP;
            GameManager.instance.globalBonusDefense += mySkillData.bonusDefense;
            GameManager.instance.globalBonusMagneticRange += mySkillData.bonusMagneticRange;

            GameObject player = GameObject.Find("Player");
            if (player != null) {
                PlayerController pc = player.GetComponent<PlayerController>();
                pc.UpdateMaxHp();
                pc.UpdateMagneticRange();
            }
        }
        else if (mySkillData.cardType == CardType.SkillUpgrade)
        {
            GameManager.instance.UpgradeSpecificSkill(
                mySkillData.targetSkillId,
                mySkillData.specificBonusDamage,
                mySkillData.specificBonusSize,
                mySkillData.specificBonusChains,
                mySkillData.specificBonusDuration,
                mySkillData.specificBonusTickRate,
                mySkillData.specificBonusAngle,
                mySkillData.specificBonusKnockback,
                mySkillData.specificBonusSwords,
                mySkillData.specificBonusRadius
            );
        }
            GameManager.instance.CloseLevelUpUI();
    }
}
