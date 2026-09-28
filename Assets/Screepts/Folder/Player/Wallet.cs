using UnityEngine;

public class Wallet : MonoBehaviour
{
    // Глобальная переменная для хранения золота игрока
    public static int TotalGold { get; private set; }

    public static void AddGold(int amount)
    {
        if (amount <= 0) return;
        TotalGold += amount;
        Debug.Log($"[Wallet] Золото добавлено: +{amount}. Всего в кошельке: {TotalGold}");
    }
}
