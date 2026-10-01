using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tanks2D
{
    public class UpgradeButton : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI goldDisplay; 
        [SerializeField] private TextMeshProUGUI upgradeInfoText; 
        [SerializeField] private Button buyButton; 

        [Header("Upgrade Settings")]
        [SerializeField] private string upgradeName = "Урон пули";
        [SerializeField] private int basePrice = 10;
        [SerializeField] private float priceMultiplier = 1.5f;
        [SerializeField] private int damagePerLevel = 1; 

        public static int BulletDamage { get; private set; } = 10; 
        private int _currentLevel = 1;

        private void Start()
        {
            BulletDamage = 10;

            if (buyButton != null)
            {
                buyButton.onClick.AddListener(BuyUpgrade);
            }

            UpdateUI();
        }

        private void Update()
        {
            // Обновляем текст золота
            if (goldDisplay != null)
            {
                goldDisplay.text = $"Золото: {Wallet.TotalGold}";
            }

            // НОВОЕ: Блокируем кнопку, если не хватает золота
            if (buyButton != null)
            {
                int cost = CalculatePrice();
                buyButton.interactable = (Wallet.TotalGold >= cost);
            }
        }

        public void BuyUpgrade()
        {
            int cost = CalculatePrice();

            if (Wallet.TrySpendGold(cost))
            {
                _currentLevel++;
                BulletDamage += damagePerLevel; 
                UpdateUI();
            }
        }

        private int CalculatePrice()
        {
            return Mathf.RoundToInt(basePrice * Mathf.Pow(priceMultiplier, _currentLevel - 1));
        }

        private void UpdateUI()
        {
            if (upgradeInfoText != null)
            {
                upgradeInfoText.text = $"{upgradeName} (LVL {_currentLevel})\nЦена: {CalculatePrice()} зл.";
            }
        }
    }
}
