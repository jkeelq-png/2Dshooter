using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tanks2D
{
    public class UpgradeFireRateButton : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI upgradeInfoText; 
        [SerializeField] private Button buyButton; 

        [Header("Upgrade Settings")]
        [SerializeField] private string upgradeName = "Скорострельность";
        [SerializeField] private int basePrice = 25; 
        [SerializeField] private float priceMultiplier = 1.6f;
        [SerializeField] private float fireRateDecrease = 0.04f; 
        [SerializeField] private float minFireRateLimit = 0.1f; 

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
            if (buyButton != null)
            {
                if (PlayerController2D.CurrentFireRate <= minFireRateLimit)
                {
                    buyButton.interactable = false;
                    return;
                }

                int cost = CalculatePrice();
                buyButton.interactable = (Wallet.TotalGold >= cost);
            }
        }

        public void BuyUpgrade()
        {
            int cost = CalculatePrice();

            if (PlayerController2D.CurrentFireRate <= minFireRateLimit) return;

            if (Wallet.TrySpendGold(cost))
            {
                _currentLevel++;
                PlayerController2D.CurrentFireRate = Mathf.Max(minFireRateLimit, PlayerController2D.CurrentFireRate - fireRateDecrease); 
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
                if (PlayerController2D.CurrentFireRate <= minFireRateLimit)
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
