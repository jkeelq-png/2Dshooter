using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tanks2D
{
    public class UpgradeReloadButton : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI upgradeInfoText; 
        [SerializeField] private Button buyButton; 

        [Header("Upgrade Settings")]
        [SerializeField] private string upgradeName = "Скорость перезарядки";
        [SerializeField] private int basePrice = 20;
        [SerializeField] private float priceMultiplier = 1.5f;
        
        [Tooltip("На сколько секунд уменьшается время перезарядки за уровень")]
        [SerializeField] private float reloadTimeDecrease = 0.2f; 
        [SerializeField] private float minReloadLimit = 0.4f; // Быстрее 0.4 сек перезаряжаться нельзя

        private int _currentLevel = 1;

        private void Start()
        {
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(BuyUpgrade);
            }
            UpdateUI();
        }

        public void BuyUpgrade()
        {
            int cost = CalculatePrice();

            // Проверяем, не достигнут ли предел
            if (PlayerController2D.CurrentReloadTime <= minReloadLimit)
            {
                Debug.Log("[Shop] Перезарядка уже максимально быстрая!");
                return;
            }

            if (Wallet.TrySpendGold(cost))
            {
                _currentLevel++;
                
                // Уменьшаем время перезарядки, но не ниже лимита
                PlayerController2D.CurrentReloadTime = Mathf.Max(minReloadLimit, PlayerController2D.CurrentReloadTime - reloadTimeDecrease); 
                
                Debug.Log($"[Upgrade] Перезарядка улучшена! Новое время: {PlayerController2D.CurrentReloadTime} сек.");
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
                if (PlayerController2D.CurrentReloadTime <= minReloadLimit)
                {
                    upgradeInfoText.text = $"{upgradeName} (LVL {_currentLevel})\nМАКС.";
                }
                else
                {
                    upgradeInfoText.text = $"{upgradeName} (LVL {_currentLevel})\nЦена: {CalculatePrice()} зл.";
                }
            }
        }
    }
}
