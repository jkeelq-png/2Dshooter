using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Tanks2D
{
    public class Wall : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int _baseMaxHP = 100;
        private int _currentHP;
        private float _visualHP;

        // ГЛОБАЛЬНАЯ СТАЦИОНАРНАЯ ПЕРЕМЕННАЯ ДЛЯ МАГАЗИНА
        // Кнопка улучшения будет увеличивать это значение
        public static int CurrentMaxHP { get; set; } = 100;

        [Header("UI Elements & Colors")]
        [SerializeField] private Slider wallHPSlider;
        [SerializeField] private Gradient hpGradient;
        [SerializeField] private TextMeshProUGUI wallHPText;

        [Header("Damage Text Settings (Всплывающий урон стены)")]
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private Color textColorForWall = new Color(0.6f, 0.1f, 1f);
        [SerializeField] private float offsetX = 0f;
        [SerializeField] private float offsetY = 2.0f;

        [Header("Animation Settings")]
        [SerializeField] private float lerpSpeed = 5f;

        [Header("Game Over Settings (Экран конца игры)")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button restartButton;

        private Image _sliderFillImage;

        // Ссылка на текущую активную стену на сцене, чтобы магазин мог приказать ей обновить UI
        public static Wall ActiveInstance { get; private set; }

        private void Awake()
        {
            ActiveInstance = this;
        }

        private void Start()
        {
            // При старте уровня задаем стене актуальное прокачанное здоровье
            // Если игрок еще ничего не качал, оно будет равно базовым 100
            if (CurrentMaxHP == 100) 
            {
                CurrentMaxHP = _baseMaxHP;
            }

            _currentHP = CurrentMaxHP;
            _visualHP = CurrentMaxHP;

            if (wallHPSlider != null)
            {
                wallHPSlider.minValue = 0;
                wallHPSlider.maxValue = CurrentMaxHP;
                wallHPSlider.value = CurrentMaxHP;

                _sliderFillImage = wallHPSlider.fillRect.GetComponent<Image>();
                UpdateSliderVisuals();
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (!Mathf.Approximately(_visualHP, _currentHP))
            {
                _visualHP = Mathf.MoveTowards(_visualHP, _currentHP, lerpSpeed * CurrentMaxHP * Time.deltaTime);
                
                if (wallHPSlider != null)
                {
                    wallHPSlider.value = _visualHP;
                    UpdateSliderVisuals();
                }
            }
        }

        public void TakeDamage(int damageAmount)
        {
            _currentHP = Mathf.Max(0, _currentHP - damageAmount);

            Debug.Log($"[Wall] Стене нанесен урон! Осталось HP: {_currentHP}/{CurrentMaxHP}");

            if (damageTextPrefab != null)
            {
                Vector3 spawnPosition = transform.position + new Vector3(offsetX, offsetY, 0f);
                GameObject textGo = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);
                
                DamageText damageTextScript = textGo.GetComponent<DamageText>();
                if (damageTextScript != null)
                {
                    damageTextScript.Setup(damageAmount, textColorForWall);
                }

                textGo.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
            }

            UpdateHPText();

            if (_currentHP <= 0)
            {
                TriggerGameOver();
            }
        }

        // МЕТОД РЕМОНТА И ОБНОВЛЕНИЯ (Вызывается из кнопки магазина при покупке апгрейда)
        public void UpgradeAndRepair()
        {
            _currentHP = CurrentMaxHP; // Полностью лечим стену
            _visualHP = CurrentMaxHP;  // Мгновенно подтягиваем ползунок

            if (wallHPSlider != null)
            {
                wallHPSlider.maxValue = CurrentMaxHP; // Увеличиваем шкалу слайдера
                wallHPSlider.value = CurrentMaxHP;
            }

            UpdateSliderVisuals();
        }

        private void UpdateSliderVisuals()
        {
            if (_sliderFillImage != null && hpGradient != null)
            {
                float hpNormalized = _visualHP / CurrentMaxHP;
                _sliderFillImage.color = hpGradient.Evaluate(hpNormalized);
            }
            UpdateHPText();
        }

        private void UpdateHPText()
        {
            if (wallHPText != null)
            {
                wallHPText.text = $"{_currentHP} / {CurrentMaxHP}";
            }
        }

        private void TriggerGameOver()
        {
            Debug.Log("[Game Over] СТЕНА РАЗРУШЕНА! Показываем экран проигрыша.");
            
            if (wallHPSlider != null) wallHPSlider.value = 0;
            if (wallHPText != null) wallHPText.text = $"0 / {CurrentMaxHP}";

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            Time.timeScale = 0f; 
        }

        public void RestartGame()
        {
            Debug.Log("[Game] Перезапуск уровня...");
            Time.timeScale = 1f; 
            
            // Сбрасываем прокачанное здоровье стены до базового при полном перезапуске игры
            CurrentMaxHP = _baseMaxHP;
            
            Wallet.ResetWallet();

            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
