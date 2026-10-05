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

    [Header("Attack Settings (Настройки атаки стены)")]
    [SerializeField] private int attackDamage = 10; 
    [SerializeField] private float attackRate = 1.5f; 
    [Tooltip("Дистанция по оси X, ближе которой свинка начинает бить стену")]
    [SerializeField] private float attackDistanceX = 1.0f; 
    private float _nextAttackTime = 0f;
    
    private bool _isAtWall = false; 
    private Tanks2D.Wall _targetWall; 

    [Header("UI & Visuals")]
    [SerializeField] private GameObject damageTextPrefab; 
    [SerializeField] private GameObject goldTextPrefab; 
    [SerializeField] private Slider hpSlider; 

    [Header("Damage Text Position Offset")]
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 1.2f; 

    private Animator animator;
    private bool isDying = false;
    private int _goldValue;
    
    // НОВОЕ: Запоминаем, босс этот объект или нет
    private bool _isBoss = false; 

    public void SetTargetWall(Tanks2D.Wall wall)
    {
        _targetWall = wall;
    }

    // ИСПРАВЛЕНО: Теперь принимает два параметра
    public void Initialize(int goldReward, bool isBoss)
    {
        _goldValue = goldReward;
        _isBoss = isBoss;
    }

    public void Initialize(int goldReward)
    {
        _goldValue = goldReward;
        _isBoss = false;
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
        if (isDying || Tanks2D.ShopController.IsPaused) return;

        if (_targetWall != null)
        {
            float distanceX = Mathf.Abs(transform.position.x - _targetWall.transform.position.x);

            if (distanceX <= attackDistanceX)
            {
                _isAtWall = true;

                if (Time.time >= _nextAttackTime)
                {
                    if (animator != null) animator.SetTrigger("Attack"); 
                    _targetWall.TakeDamage(attackDamage);
                    _nextAttackTime = Time.time + attackRate;
                }
            }
            else
            {
                _isAtWall = false;
            }
        }
        else
        {
            _isAtWall = false;
        }

        if (!_isAtWall)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
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

        if (currentHP <= 0) Die();
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;

        // ИСПРАВЛЕНО: Передаем спавнеру точный флаг — босс умер или обычный моб
        if (Tanks2D.EnemySpawner2D.Instance != null)
        {
            Tanks2D.EnemySpawner2D.Instance.RegisterEnemyDeath(_isBoss);
        }

        Wallet.AddGold(_goldValue);

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
                textMesh.text = $"+{_goldValue}";
                textMesh.color = new Color(1f, 0.84f, 0f); 
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
