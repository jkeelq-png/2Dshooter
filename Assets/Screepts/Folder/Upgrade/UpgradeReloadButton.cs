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
        [SerializeField] private float reloadTimeDecrease = 0.2f; 
        [SerializeField] private float minReloadLimit = 0.4f; 

        private int _currentLevel = 1;

        private void Start()
        {
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(BuyUpgrade);
            }
            UpdateUI();
        }

        private void Update()
        {
            // НОВОЕ: Управляем доступностью кнопки
            if (buyButton != null)
            {
                // Если достигнут максимум, кнопка серая всегда
                if (PlayerController2D.CurrentReloadTime <= minReloadLimit)
                {
                    buyButton.interactable = false;
                    return;
                }

                // Иначе проверяем золото
                int cost = CalculatePrice();
                buyButton.interactable = (Wallet.TotalGold >= cost);
            }
        }

        public void BuyUpgrade()
        {
            int cost = CalculatePrice();

            if (PlayerController2D.CurrentReloadTime <= minReloadLimit) return;

            if (Wallet.TrySpendGold(cost))
            {
                _currentLevel++;
                PlayerController2D.CurrentReloadTime = Mathf.Max(minReloadLimit, PlayerController2D.CurrentReloadTime - reloadTimeDecrease); 
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
