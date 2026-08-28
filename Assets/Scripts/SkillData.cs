using UnityEngine;
public enum CardType
{
    ActiveSkill,
    PassiveStat,
    SkillUpgrade
}
public enum CardRarity
{
    Normal,
    Rare,
    Legendary
}

[CreateAssetMenu(fileName = "New Skill Data", menuName = "Scriptable Object/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("등장 확률 (가중치)")]
    public int dropWeight = 10;

    [Header("스킬 기본 정보")]
    public CardType cardType;
    public string skillName;

    [TextArea]
    public string skillDescription;
    public Sprite skillIcon;

    [Header("액티브 스킬 전용 (CardType이 ActiveSkill일 때만 사용)")]
    public GameObject skillPrefab;

    [Header("패시브 스탯 전용 (CardType이 PassiveStat일 때만 사용)")]
    public int bonusMaxHP;
    public int bonusDefense;
    public float bonusMagneticRange;

    [Header("특정 스킬 전용 강화 (CardType이 SkillUpgrade일 때 사용)")]
    public string targetSkillId;
    public int specificBonusDamage;
    public float specificBonusSize;
    public int specificBonusChains;
    public float specificBonusDuration;
    public float specificBonusTickRate;
    public float specificBonusAngle;
    public float specificBonusKnockback;
    public int specificBonusSwords;
    public float specificBonusRadius;

    [Header("카드 등급")]
    public CardRarity rarity = CardRarity.Normal;
}
