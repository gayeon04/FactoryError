
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("References")]
    public FactoryGameManager gameManager;
    public ScoreManager scoreManager;

    [Header("HUD Texts")]
    public TMP_Text stateText;
    public TMP_Text timerText;
    public TMP_Text productionText;
    public TMP_Text failedText;
    public TMP_Text spawnedText;
    public TMP_Text scoreText;

    [Header("Controls")]
    public GameObject deviceToolbar;

    public Button startButton;
    public Button pauseButton;
    public Button retryButton;
    public Button resetAllButton;

    public TMP_Text pauseButtonText;

    private void Update()
    {
        UpdateHUD();
        UpdateButtons();
    }

    private void UpdateHUD()
    {
        stateText.text =
            gameManager.State.ToString().ToUpper();

        int seconds = Mathf.CeilToInt(
            gameManager.RemainingTime
        );

        timerText.text =
            $"{seconds / 60:00}:{seconds % 60:00}";

        productionText.text =
            $"PRODUCTION : {scoreManager.GoodCount}" +
            $" / {gameManager.requiredProducts}";

        failedText.text =
            $"FAILED : {scoreManager.FailedCount}";

        spawnedText.text =
            $"SUPPLY : {gameManager.GetSpawnedCount()}" +
            $" / {gameManager.GetTotalProductCount()}";

        scoreText.text =
            $"SCORE : {scoreManager.FinalScore}";
    }

    private void UpdateButtons()
    {
        FactoryGameState state = gameManager.State;

        bool isBuild =
            state == FactoryGameState.Build;

        bool isRunning =
            state == FactoryGameState.Running;

        bool isPaused =
            state == FactoryGameState.Paused;

        startButton.interactable = isBuild;

        pauseButton.interactable =
            isRunning || isPaused;

        retryButton.interactable = !isBuild;

        resetAllButton.interactable = isBuild;

        pauseButtonText.text =
            isPaused ? "RESUME" : "PAUSE";

        deviceToolbar.SetActive(isBuild);
    }
}
