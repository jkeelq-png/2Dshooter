using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class PigEnemy : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 30;
    private int currentHP;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f; 

    [Header("UI & Visuals")]
    [SerializeField] private GameObject damageTextPrefab; 

    // Ссылка на Слайдер из инспектора
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
            hpSlider.value = maxHP; 
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

        if (hpSlider != null)
        {
            hpSlider.value = currentHP; 
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
            // Скрываем весь слайдер при смерти, чтобы он не висел в воздухе над трупом
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

    // ИСПРАВЛЕННАЯ КОРУТИНА: Ждёт включения и окончания анимации смерти
    private IEnumerator CleanUpAfterAnimation()
    {
        if (animator != null)
        {
            // 1. Ждем окончания текущего кадра, чтобы триггер смерти точно применился в Unity
            yield return new WaitForEndOfFrame();

            // 2. Даем аниматору немного времени (до 0.15 сек) на переход в состояние "Die"
            float maxTransitionWait = 0.15f;
            float elapsed = 0f;
            
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Die") && elapsed < maxTransitionWait)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // 3. Получаем точную длину анимации смерти и ждем её полного окончания
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
        }
        else
        {
            // Если аниматора нет, просто исчезаем через полсекунды
            yield return new WaitForSeconds(0.5f);
        }

        // Окончательно удаляем свинку со сцены
        Destroy(gameObject);
    }
}
