
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class StageManager : MonoBehaviour
{
    [Header("Stage Data")]
    public StageData stageData;

    [Header("References")]
    public FactoryGameManager gameManager;
    public BudgetManager budgetManager;
    public BuildManager buildManager;

    [Header("UI")]
    public TMP_Text stageNameText;
    public TMP_Text descriptionText;

    public bool HasNextStage =>
        !string.IsNullOrEmpty(
            stageData.nextSceneName
        );

    private void Awake()
    {
        ApplyStageData();
    }

    private void ApplyStageData()
    {
        budgetManager.initialBudget =
            stageData.initialBudget;

        gameManager.runDuration =
            stageData.runDuration;

        gameManager.requiredProducts =
            stageData.requiredProducts;

        buildManager.availableDevices =
            stageData.availableDevices;

        if (stageNameText != null)
        {
            stageNameText.text =
                $"STAGE {stageData.stageID:00}" +
                $" - {stageData.stageName}";
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                stageData.description;
        }
    }

    public void LoadNextStage()
    {
        if (gameManager.State !=
            FactoryGameState.Result)
            return;

        if (!gameManager.IsClear)
            return;

        if (!HasNextStage)
            return;

        SceneManager.LoadScene(
            stageData.nextSceneName
        );
    }

    public void RestartStage()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}
