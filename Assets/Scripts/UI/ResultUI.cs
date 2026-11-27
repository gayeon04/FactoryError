
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [Header("References")]
    public FactoryGameManager gameManager;
    public ScoreManager scoreManager;
    public BudgetManager budgetManager;

    [Header("Panel")]
    public GameObject resultPanel;

    [Header("Texts")]
    public TMP_Text titleText;
    public TMP_Text finalScoreText;

    public TMP_Text goodCountText;
    public TMP_Text wrongCountText;
    public TMP_Text wasteCountText;
    public TMP_Text unprocessedCountText;

    public TMP_Text remainingBudgetText;

    public StageManager stageManager;

    public GameObject nextStageButton;
    public GameObject gameCompleteText;

    private void OnEnable()
    {
        gameManager.OnStateChanged += OnStateChanged;
    }

    private void Start()
    {
        OnStateChanged(gameManager.State);
    }

    private void OnDisable()
    {
        gameManager.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(
        FactoryGameState state)
    {
        if (state == FactoryGameState.Result)
        {
            ShowResult();
        }
        else
        {
            resultPanel.SetActive(false);
        }
    }

    private void ShowResult()
    {
        resultPanel.SetActive(true);

        bool isClear = gameManager.IsClear;

        titleText.text = isClear
            ? "PRODUCTION COMPLETE"
            : "PRODUCTION FAILED";

        titleText.color = isClear
            ? new Color(0.4f, 0.9f, 0.6f)
            : new Color(1f, 0.4f, 0.4f);

        finalScoreText.text =
            $"{scoreManager.FinalScore:N0} P";

        goodCountText.text =
            $"정상 생산 : {scoreManager.GoodCount}" +
            $" / {gameManager.requiredProducts}";

        wrongCountText.text =
            $"분류 실패 : {scoreManager.WrongCount}";

        wasteCountText.text =
            $"폐기 제품 : {scoreManager.WasteCount}";

        unprocessedCountText.text =
            $"미처리 제품 : " +
            $"{scoreManager.UnprocessedCount}";

        remainingBudgetText.text =
            $"남은 예산 : " +
            $"{budgetManager.CurrentBudget} G";


        nextStageButton.SetActive(
        isClear && stageManager.HasNextStage
    );

            if (gameCompleteText != null)
            {
                gameCompleteText.SetActive(
                    isClear && !stageManager.HasNextStage
                );
            }
    }
}
