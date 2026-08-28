using UnityEngine;
using TMPro;

public class ComboText : MonoBehaviour
{
    public float destroyTime = 0.5f;
    public float moveSpeed = 2.0f;

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

        if (comboCount >= 5) textMesh.color = Color.yellow;

        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        if (textMesh != null)
        {
            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * 5f);
            textMesh.color = alpha;
        }
    }
}