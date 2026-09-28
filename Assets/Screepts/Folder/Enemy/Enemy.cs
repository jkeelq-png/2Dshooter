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

    [SerializeField] private Slider hpSlider; 

    [Header("Damage Text Position Offset")]
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 1.2f; 

    private Animator animator;
    private bool isDying = false;

    // ЗОЛОТО: Переменная, где свинка хранит свою стоимость
    private int _goldValue;

    // ЗОЛОТО: Этот метод вызовет спавнер в момент создания врага
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

        // ЗОЛОТО: В момент смерти пули мгновенно отправляем золото в кошелек!
        Wallet.AddGold(_goldValue);

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
