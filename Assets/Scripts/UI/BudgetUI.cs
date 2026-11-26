
using TMPro;
using UnityEngine;

public class BudgetUI : MonoBehaviour
{
    public BudgetManager budgetManager;

    public TMP_Text budgetText;

    private void OnEnable()
    {
        budgetManager.OnBudgetChanged +=
            UpdateBudget;
    }

    private void Start()
    {
        UpdateBudget(
            budgetManager.CurrentBudget
        );
    }

    private void OnDisable()
    {
        budgetManager.OnBudgetChanged -=
            UpdateBudget;
    }

    private void UpdateBudget(int budget)
    {
        budgetText.text =
            $"BUDGET : {budget} / " +
            $"{budgetManager.initialBudget} G";
    }
}
