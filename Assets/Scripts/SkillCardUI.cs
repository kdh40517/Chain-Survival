using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardUI : MonoBehaviour
{
    [Header("UI 연결 부품들")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descText;

    // 💡 1. 여기에 우리가 만든 노말/희귀/전설 픽셀 Sprite 3종을 연결해 둡니다.
    [Header("등급별 배경 Sprite (픽셀아트)")]
    public Sprite normalBackground;
    public Sprite rareBackground;
    public Sprite legendaryBackground;
    public Sprite activeBackground;

    // 💡 2. 색깔이 변해야 하는 카드의 진짜 배경 Image 컴포넌트를 연결해 주세요.
    [Header("색깔을 칠할 이미지")]
    public Image borderImage;

    private SkillData mySkillData;
      public void SetUpCard(SkillData data)
    {
        mySkillData = data;
        iconImage.sprite = data.skillIcon;
        nameText.text = data.skillName;
        descText.text = data.skillDescription;

        // 💡 1순위 체크: 액티브 스킬인가?
        if (data.cardType == CardType.ActiveSkill)
        {
            if (activeBackground != null) borderImage.sprite = activeBackground;
        }
        else // 💡 2순위: 액티브가 아니면(패시브/강화) 등급별 배경 적용
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

        // 투명도/색깔 덧칠 방지 (Color.white는 원본 그대로)
        if (borderImage != null) borderImage.color = Color.white;
    }
    public void OnClickCard()
    {
        Debug.Log("유저가 선택한 스킬: " + mySkillData.skillName);
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
                        Debug.Log((i + 1) + "단 체인 슬롯에 [" + mySkillData.skillName + "] 장착 완료!!");

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
