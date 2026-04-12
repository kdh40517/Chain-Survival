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
    public int bonusMaxHP;           // 최대 체력 증가
    public int bonusDefense;         // 방어력 증가 (받는 피해 감소)
    public float bonusMagneticRange; // 자석 획득 범위 증가

    [Header("특정 스킬 전용 강화 (CardType이 SkillUpgrade일 때 사용)")]
    public string targetSkillId;
    public int specificBonusDamage;
    public float specificBonusSize;
    public int specificBonusChains;
    public float specificBonusDuration;
    public float specificBonusTickRate;
    public float specificBonusAngle;
    public float specificBonusKnockback;
    public int specificBonusSwords; // 이 강화 카드를 먹으면 검 개수 추가!
    public float specificBonusRadius; // 이 강화 카드를 먹으면 반경 증가!

    [Header("카드 등급")]
    public CardRarity rarity = CardRarity.Normal;
}
