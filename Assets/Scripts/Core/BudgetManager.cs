
using System;
using UnityEngine;

public class BudgetManager : MonoBehaviour
{
    [Min(0)]
    public int initialBudget = 100;

    public int CurrentBudget { get; private set; }

    public int UsedBudget =>
        initialBudget - CurrentBudget;

    public event Action<int> OnBudgetChanged;

    private void Awake()
    {
        ResetBudget();
    }

    public bool CanAfford(int cost)
    {
        return cost >= 0 && CurrentBudget >= cost;
    }

    public bool TrySpend(int cost)
    {
        if (!CanAfford(cost))
            return false;

        CurrentBudget -= cost;

        OnBudgetChanged?.Invoke(CurrentBudget);

        return true;
    }

    public void Refund(int amount)
    {
        if (amount < 0)
            return;

        CurrentBudget = Mathf.Min(
            initialBudget,
            CurrentBudget + amount
        );

        OnBudgetChanged?.Invoke(CurrentBudget);
    }

    public void ResetBudget()
    {
        CurrentBudget = initialBudget;

        OnBudgetChanged?.Invoke(CurrentBudget);
    }
}
