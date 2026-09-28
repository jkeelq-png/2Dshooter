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

    [Header("Damage Text Position Offset")]
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 1.2f; 

    // Сделали приватным — теперь инспектор идёт лесом, код найдёт всё сам!
    private Image hpBarImage; 
    private Animator animator;
    private bool isDying = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHP = maxHP;

        // ЖЕЛЕЗОБЕТОННЫЙ ПОИСК: ищем компонент Image в объектах внутри свинки
        hpBarImage = GetComponentInChildren<Image>();

        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = 1f;
            
            // На всякий случай проверяем, что у картинки включен нужный режим
            if (hpBarImage.type != Image.Type.Filled)
            {
                hpBarImage.type = Image.Type.Filled;
                hpBarImage.fillMethod = Image.FillMethod.Horizontal;
            }
        }
        else
        {
            // Если Canvas материнский, ищем по имени объекта на сцене
            GameObject globalBar = GameObject.Find("Pig_HP_Bar");
            if (globalBar != null)
            {
                hpBarImage = globalBar.GetComponent<Image>();
            }
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

        if (hpBarImage != null)
        {
            // Теперь деление точно дробное и картинка живая со сцены
            float healthPercentage = (float)currentHP / maxHP;
            hpBarImage.fillAmount = Mathf.Clamp01(healthPercentage);
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

        if (hpBarImage != null)
        {
            // Скрываем только картинку, чтобы не сломать весь Canvas игры
            hpBarImage.gameObject.SetActive(false);
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
