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
