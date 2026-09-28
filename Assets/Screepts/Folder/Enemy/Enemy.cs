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
    [SerializeField] private GameObject goldTextPrefab; // Префаб текста с .png монеткой внутри

    [SerializeField] private Slider hpSlider; 

    [Header("Damage Text Position Offset")]
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 1.2f; 

    private Animator animator;
    private bool isDying = false;

    // Внутренняя переменная для золота, которую заполнит спавнер
    private int _goldValue;

    // Метод инициализации золота (вызывается из EnemySpawner2D)
    public void Initialize(int goldReward)
    {
        _goldValue = goldReward;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHP = maxHP;

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
        if (isDying) return;
        isDying = true;

        // Железобетонно начисляем золото в кошелек
        Wallet.AddGold(_goldValue);

        // Спавним текст полученного золота прямо над свинкой
        if (goldTextPrefab != null && _goldValue > 0)
        {
            Vector3 spawnPosition = transform.position + new Vector3(offsetX, offsetY + 0.3f, 0f);
            GameObject textGo = Instantiate(goldTextPrefab, spawnPosition, Quaternion.identity);
            
            DamageText goldTextScript = textGo.GetComponent<DamageText>();
            if (goldTextScript != null)
            {
                goldTextScript.Setup(_goldValue); 
            }

            TMPro.TMP_Text textMesh = textGo.GetComponent<TMPro.TMP_Text>();
            if (textMesh != null)
            {
                // Выводим только знак "+" и цифру, так как .png монетка встроена в сам префаб
                textMesh.text = $"+{_goldValue}";
                textMesh.color = new Color(1f, 0.84f, 0f); // Красивый золотой цвет
            }
        }

        if (hpSlider != null)
        {
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
        if (animator != null)
        {
            yield return new WaitForEndOfFrame();

            float maxTransitionWait = 0.15f;
            float elapsed = 0f;
            
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Die") && elapsed < maxTransitionWait)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

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
