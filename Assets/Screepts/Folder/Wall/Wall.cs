using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Tanks2D
{
    public class Wall : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHP = 100;
        private int _currentHP;
        private float _visualHP;

        [Header("UI Elements & Colors")]
        [SerializeField] private Slider wallHPSlider;
        [SerializeField] private Gradient hpGradient;
        [SerializeField] private TextMeshProUGUI wallHPText;

        [Header("Damage Text Settings (Всплывающий урон стены)")]
        [SerializeField] private GameObject damageTextPrefab;
        
        [Tooltip("Каким цветом будут вылетать цифры урона при ударе по стене")]
        [SerializeField] private Color textColorForWall = new Color(0.6f, 0.1f, 1f); // Ярко-фиолетовый
        
        [SerializeField] private float offsetX = 0f;
        [SerializeField] private float offsetY = 2.0f;

        [Header("Animation Settings")]
        [SerializeField] private float lerpSpeed = 5f;

        [Header("Game Over Settings (Экран конца игры)")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button restartButton;

        private Image _sliderFillImage;

        private void Start()
        {
            _currentHP = maxHP;
            _visualHP = maxHP;

            if (wallHPSlider != null)
            {
                wallHPSlider.minValue = 0;
                wallHPSlider.maxValue = maxHP;
                wallHPSlider.value = maxHP;

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
                _visualHP = Mathf.MoveTowards(_visualHP, _currentHP, lerpSpeed * maxHP * Time.deltaTime);
                
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

            Debug.Log($"[Wall] Стене нанесен урон! Осталось HP: {_currentHP}/{maxHP}");

            if (damageTextPrefab != null)
            {
                Vector3 spawnPosition = transform.position + new Vector3(offsetX, offsetY, 0f);
                GameObject textGo = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);
                
                DamageText damageTextScript = textGo.GetComponent<DamageText>();
                if (damageTextScript != null)
                {
                    damageTextScript.Setup(damageAmount, textColorForWall);
                }

                // НОВОЕ: Делаем цифры урона стены на 40% крупнее, чтобы они бросались в глаза
                // Родной масштаб префаба в DamageText.cs равен 0.1, мы сделаем 0.14
                textGo.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
            }

            UpdateHPText();

            if (_currentHP <= 0)
            {
                TriggerGameOver();
            }
        }

        private void UpdateSliderVisuals()
        {
            if (_sliderFillImage != null && hpGradient != null)
            {
                float hpNormalized = _visualHP / maxHP;
                _sliderFillImage.color = hpGradient.Evaluate(hpNormalized);
            }
            UpdateHPText();
        }

        private void UpdateHPText()
        {
            if (wallHPText != null)
            {
                wallHPText.text = $"{_currentHP} / {maxHP}";
            }
        }

        private void TriggerGameOver()
        {
            Debug.Log("[Game Over] СТЕНА РАЗРУШЕНА! Показываем экран проигрыша.");
            
            if (wallHPSlider != null) wallHPSlider.value = 0;
            if (wallHPText != null) wallHPText.text = $"0 / {maxHP}";

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
            Wallet.ResetWallet();

            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
