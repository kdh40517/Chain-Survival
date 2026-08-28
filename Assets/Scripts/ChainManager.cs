using System.Collections;
using UnityEngine;

public class ChainManager : MonoBehaviour
{
    [Header("콤보 연출")]
    public GameObject comboTextPrefab;

    public GameObject[] skillSlots = new GameObject[6];

    public float[] damegeMultipliers = { 1.5f, 2.0f, 3.0f, 5.0f, 7.0f, 10.0f };
    public float[] sizeMultipliers = { 1.0f, 1.0f, 2.0f, 3.0f, 5.0f, 10.0f };
    public float[] chainProbabilities = { 10f, 15f, 20f, 5f, 3f, 1f };
    private float lastChainTime = 0f;
    public void StartChainReaction(Vector2 targetPos)
    {
        if (Time.time - lastChainTime < 0.2f) return;
        lastChainTime = Time.time;
        StartCoroutine(ChainRoutine(targetPos));
    }
    IEnumerator ChainRoutine(Vector2 targetPos)
    {
        for (int i = 0; i < 6; i++)
        {
            if (skillSlots[i] == null) break;
            float dice = Random.Range(0f, 100f);
            if (dice <= chainProbabilities[i])
            {
                Debug.Log($"[Chain] {i + 1}단 발동 - {skillSlots[i].name}");
                if (comboTextPrefab != null)
                {
                    Vector3 textSpawnPos = transform.position + new Vector3(0f, 1.5f, 0f);
                    GameObject textObj = Instantiate(comboTextPrefab, textSpawnPos, Quaternion.identity);

                    ComboText comboScript = textObj.GetComponent<ComboText>();
                    if (comboScript != null)
                    {
                        comboScript.Setup(i + 1);
                    }
                }
                GameObject newSkill = Instantiate(skillSlots[i], targetPos, Quaternion.identity);
                SkillEffect effectScript = newSkill.GetComponent<SkillEffect>();
                if (effectScript != null)
                {
                    if (effectScript.spwnOnPlayer)
                    {
                        newSkill.transform.SetParent(transform);
                        newSkill.transform.localPosition = Vector3.zero;
                    }
                    
                    effectScript.targetSizeMult = sizeMultipliers[i];
                    effectScript.skillDamage = Mathf.RoundToInt(1 * damegeMultipliers[i]);
                    effectScript.chainLevel = i + 1;
                }
                float currentSizeMult = sizeMultipliers[i];
                yield return new WaitForSeconds(0.15f);
            }
            else
            {
                Debug.Log($"[Chain] {i + 1}단 실패 - 연쇄 종료");
                break;
            }
        }
    }
}
