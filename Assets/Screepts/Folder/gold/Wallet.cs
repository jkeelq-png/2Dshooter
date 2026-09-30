using UnityEngine;

public class Wallet : MonoBehaviour
{
    public static int TotalGold { get; private set; }

    private void Awake()
    {
        ResetWallet(); 
    }

    public static void AddGold(int amount)
    {
        if (amount <= 0) return;
        TotalGold += amount;
        Debug.Log($"[Wallet] Золото добавлено: +{amount}. Всего в кошельке: {TotalGold}");
    }

    // НОВОЕ: Метод для проверки баланса и списания золота
    public static bool TrySpendGold(int amount)
    {
        if (amount <= 0) return false;

        if (TotalGold >= amount)
        {
            TotalGold -= amount;
            Debug.Log($"[Wallet] Золото списано: -{amount}. Осталось: {TotalGold}");
            return true; // Успешно списано
        }

        Debug.Log($"[Wallet] Недостаточно золота! Нужно: {amount}, есть: {TotalGold}");
        return false; // Золота не хватает
    }

    public static void ResetWallet()
    {
        TotalGold = 0;
    }
}
