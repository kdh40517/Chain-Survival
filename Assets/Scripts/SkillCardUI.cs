using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardUI : MonoBehaviour
{
    [Header("UI 연결 부품들")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descText;
    private SkillData mySkillData;
    public void SetUpCard(SkillData data)
    {
        mySkillData = data;
        iconImage.sprite = data.skillIcon;
        nameText.text = data.skillName;
        descText.text = data.skillDescription;
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
            GameManager.instance.globalBonusDamage += mySkillData.bonusDamage;
            GameManager.instance.globalBonusSize += mySkillData.bonusSize;
            GameManager.instance.globalBonusDuration += mySkillData.bonusDuration;
            GameManager.instance.globalBonusAngle += mySkillData.bonusAngle;
            Debug.Log($"💪 패시브 획득! 현재 글로벌 추가 크기: {GameManager.instance.globalBonusSize}");
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
                mySkillData.specificBonusKnockback
            );
        }
            GameManager.instance.CloseLevelUpUI();
    }
}
