using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI goldDisplay; 
    public TextMeshProUGUI upgradeInfoText; 
    public Button buyButton; 

    [Header("Upgrade Settings")]
    public string upgradeName = "Урон пули";
    public int basePrice = 10;
    public float priceMultiplier = 1.5f;
    public int damagePerLevel = 5; // На сколько увеличится урон за один апгрейд

    // ГЛОБАЛЬНАЯ переменная урона, к ней сможет обратиться пуля из любого места
    public static int BulletDamage { get; private set; } = 10; 
    private int currentLevel = 1;

    void Start()
    {
        // Сбрасываем урон до базового при старте игры
        BulletDamage = 10;

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuyUpgrade);
        }

        UpdateUI();
    }

    void Update()
    {
        if (goldDisplay != null)
        {
            goldDisplay.text = $"Золото: {Wallet.TotalGold}";
        }
    }

    public void BuyUpgrade()
    {
        int cost = CalculatePrice();

        if (Wallet.TrySpendGold(cost))
        {
            currentLevel++;
            
            // УВЕЛИЧИВАЕМ ГЛОБАЛЬНЫЙ УРОН
            BulletDamage += damagePerLevel; 
            
            Debug.Log($"Улучшение куплено! Новый урон пули: {BulletDamage}");
            UpdateUI();
        }
    }

    private int CalculatePrice()
    {
        return Mathf.RoundToInt(basePrice * Mathf.Pow(priceMultiplier, currentLevel - 1));
    }

    void UpdateUI()
    {
        if (upgradeInfoText != null)
        {
            upgradeInfoText.text = $"{upgradeName} (LVL {currentLevel})\nЦена: {CalculatePrice()}";
        }
    }
}
