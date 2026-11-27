
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("References")]
    public PhysicsWorld physicsWorld;
    public BudgetManager budgetManager;

    public ProductCollector[] collectors;

    [Header("Score Settings")]
    public int goodProductPoint = 100;
    public int failedProductPenalty = 50;
    public int budgetBonusPoint = 5;

    public int UnprocessedCount { get; private set; }

    public int GoodCount
    {
        get
        {
            int count = 0;

            foreach (ProductCollector collector in collectors)
            {
                if (collector != null)
                    count += collector.collectedCount;
            }

            return count;
        }
    }

    public int WrongCount
    {
        get
        {
            int count = 0;

            foreach (ProductCollector collector in collectors)
            {
                if (collector != null)
                    count += collector.failedCount;
            }

            return count;
        }
    }

    public int WasteCount =>
        physicsWorld.WastedCount;

    public int FailedCount =>
        WrongCount + WasteCount + UnprocessedCount;

    public int ProcessedCount =>
        GoodCount + WrongCount + WasteCount;

    public int FinalScore
    {
        get
        {
            int score =
                GoodCount * goodProductPoint
                - FailedCount * failedProductPenalty
                + budgetManager.CurrentBudget
                    * budgetBonusPoint;

            return Mathf.Max(0, score);
        }
    }

    public void FinishProduction(int totalSpawned)
    {
        UnprocessedCount = Mathf.Max(
            0,
            totalSpawned - ProcessedCount
        );
    }

    public void ResetScore()
    {
        UnprocessedCount = 0;

        foreach (ProductCollector collector in collectors)
        {
            if (collector != null)
                collector.ResetCollector();
        }
    }
}
