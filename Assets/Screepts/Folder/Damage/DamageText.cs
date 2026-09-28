using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header("Movement & Fade")]
    [SerializeField] private float moveSpeed = 1f;       
    [SerializeField] private float fadeDuration = 0.8f;   
    [SerializeField] private Vector3 randomOffset = new Vector3(0.3f, 0f, 0f); 

    private TextMeshPro textMeshPro;
    private Color startColor;
    private float lifetime;

    // МЕТОД ДЛЯУСТАНОВКИ ЦИФРЫ УРОНА
    public void Setup(int damageAmount)
    {
        // Находим компонент текста, если он еще не найден
        if (textMeshPro == null) textMeshPro = GetComponent<TextMeshPro>();
        
        if (textMeshPro != null)
        {
            textMeshPro.text = damageAmount.ToString(); // Меняем текст на число урона
            startColor = textMeshPro.color;
        }
    }

    void Start()
    {
        if (textMeshPro == null) textMeshPro = GetComponent<TextMeshPro>();
        if (textMeshPro != null) startColor = textMeshPro.color;

        // Блокировка поворота и масштаба, чтобы текст не косило от свинки
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); 

        transform.position += new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(-randomOffset.y, randomOffset.y),
            0f
        );

        Destroy(gameObject, fadeDuration);
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);

        lifetime += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, lifetime / fadeDuration);

        if (textMeshPro != null)
        {
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }
    }
}
