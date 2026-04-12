using UnityEngine;
using TMPro;

public class ComboText : MonoBehaviour
{
    public float destroyTime = 0.5f;
    public float moveSpeed = 2.0f;

    // 💡 핵심: TMP_Text를 쓰면 UI든 월드(3D)든 유니티가 알아서 찾습니다!
    private TMP_Text textMesh;
    private Color alpha;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        if (textMesh != null) alpha = textMesh.color;
    }

    public void Setup(int comboCount)
    {
        if (textMesh == null) return;
        textMesh.text = comboCount + " Combo!";

        // 5콤보 이상일 때 노란색으로 빵 터지는 효과!
        if (comboCount >= 5) textMesh.color = Color.yellow;

        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // 글자가 위로 둥둥 떠오름
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 글자가 서서히 투명해짐
        if (textMesh != null)
        {
            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * 5f);
            textMesh.color = alpha;
        }
    }
}