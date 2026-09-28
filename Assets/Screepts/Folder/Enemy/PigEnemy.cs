using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Оставляем для работы с UI

public class PigEnemy : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 30;
    private int currentHP;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f; 

    [Header("UI & Visuals")]
    [SerializeField] private GameObject damageTextPrefab; 

    // ССЫЛКА НА СЛАЙДЕР: Перетащим его вручную в инспекторе!
    [SerializeField] private Slider hpSlider; 

    [Header("Damage Text Position Offset")]
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 1.2f; 

    private Animator animator;
    private bool isDying = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHP = maxHP;

        // Настраиваем слайдер при старте игры под наше здоровье
        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = maxHP;
            hpSlider.value = maxHP; // На старте полоска полная (30 из 30)
        }
    }

    void Update()
    {
        if (isDying) return;
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    public void ApplyDamage(int damage)
    {
        if (isDying) return;

        currentHP -= damage;

        // ИЗМЕНЕНИЕ: Теперь двигаем ползунок слайдера!
        if (hpSlider != null)
        {
            hpSlider.value = currentHP; // Слайдер сам мгновенно уменьшит полоску
        }

        if (damageTextPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(offsetX, offsetY, 0f);
            GameObject textGo = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);
            
            DamageText damageTextScript = textGo.GetComponent<DamageText>();
            if (damageTextScript != null)
            {
                damageTextScript.Setup(damage);
            }
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDying = true;

        if (hpSlider != null)
        {
            // Скрываем весь слайдер при смерти
            hpSlider.gameObject.SetActive(false);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        StartCoroutine(CleanUpAfterAnimation());
    }

    private IEnumerator CleanUpAfterAnimation()
    {
        yield return null;

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        Destroy(gameObject);
    }
}
