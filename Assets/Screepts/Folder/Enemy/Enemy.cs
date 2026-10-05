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
    [SerializeField] private float attackDistanceX = 1.2f; 
    private float _nextAttackTime = 0f;
    
    private bool _isAtWall = false; 
    private Tanks2D.Wall _targetWall; // Переменная, которую заполнит спавнер

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

    // Метод, через который Спавнер принудительно передает ссылку на стену
    public void SetTargetWall(Tanks2D.Wall wall)
    {
        _targetWall = wall;
        if (wall != null)
        {
            Debug.Log($"[PigEnemy] Свинка {name} успешно получила ссылку на стену! Координата стены X: {wall.transform.position.x}");
        }
    }

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

        // Если спавнер почему-то не передал стену, ищем её сами аварийно
        if (_targetWall == null)
        {
            _targetWall = Object.FindFirstObjectByType<Tanks2D.Wall>();
        }
    }

    void Update()
    {
        if (isDying || Tanks2D.ShopController.IsPaused) return;

        if (_targetWall != null)
        {
            // Считаем расстояние строго по горизонтали X
            float distanceX = Mathf.Abs(transform.position.x - _targetWall.transform.position.x);

            // ЖЕЛЕЗОБЕТОННАЯ ПРОВЕРКА: Если подошли на дистанцию атаки
            if (distanceX <= attackDistanceX)
            {
                if (!_isAtWall)
                {
                    _isAtWall = true;
                    Debug.Log($"[PigEnemy] Свинка {name} ДОШЛА ДО СТЕНЫ! Дистанция: {distanceX}. Останавливаемся.");
                }

                // Логика атаки по таймеру
                if (Time.time >= _nextAttackTime)
                {
                    if (animator != null) animator.SetTrigger("Attack"); 
                    _targetWall.TakeDamage(attackDamage);
                    _nextAttackTime = Time.time + attackRate;
                }
            }
            else
            {
                // Если мы уже пересекли стену (пролетели мимо из-за высокой скорости) — принудительно разворачиваем и стопим!
                // Проверяем, не пролетели ли мы координату стены
                bool passedWallRightToLeft = (speed < 0 || transform.right.x < 0) && (transform.position.x < _targetWall.transform.position.x);
                bool passedWallLeftToRight = (speed > 0 || transform.right.x > 0) && (transform.position.x > _targetWall.transform.position.x);

                if (passedWallRightToLeft || passedWallLeftToRight)
                {
                    _isAtWall = true; // Принудительно стопим, так как стена уже позади/под нами
                    return;
                }

                _isAtWall = false;
            }
        }
        else
        {
            _isAtWall = false;
        }

        // ДВИЖЕНИЕ: Если не у стены — бежим вперед
        if (!_isAtWall)
        {
            // Поддержка как классического Translate, так и движения на основе локальных осей
            transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
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
