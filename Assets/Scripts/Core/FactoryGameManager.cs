
using System;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class FactoryGameManager : MonoBehaviour
{
    [Header("References")]
    public PhysicsWorld physicsWorld;
    public BuildManager buildManager;
    public ScoreManager scoreManager;

    public ProductSpawner[] spawners;

    [Header("Stage Settings")]
    public float runDuration = 40f;
    public int requiredProducts = 20;

    [Header("Runtime")]
    [SerializeField]
    private FactoryGameState state;

    public FactoryGameState State => state;

    public float RemainingTime { get; private set; }

    public bool IsClear =>
        scoreManager.GoodCount >= requiredProducts;

    public event Action<FactoryGameState> OnStateChanged;

    private void Start()
    {
        Retry();
    }

    private void FixedUpdate()
    {
        if (state != FactoryGameState.Running)
            return;

        RemainingTime = Mathf.Max(
            0f,
            RemainingTime - Time.fixedDeltaTime
        );

        // 제한 시간 종료
        if (RemainingTime <= 0f)
        {
            FinishRun();
            return;
        }

        // 모든 제품 생성 및 처리 완료
        if (AllSpawnersFinished() &&
            scoreManager.ProcessedCount >= GetSpawnedCount())
        {
            FinishRun();
        }
    }

    public void StartRun()
    {
        if (state != FactoryGameState.Build)
            return;

        SetState(FactoryGameState.Running);
    }

    public void TogglePause()
    {
        if (state == FactoryGameState.Running)
        {
            SetState(FactoryGameState.Paused);
        }
        else if (state == FactoryGameState.Paused)
        {
            SetState(FactoryGameState.Running);
        }
    }

    public void FinishRun()
    {
        if (state != FactoryGameState.Running)
            return;

        scoreManager.FinishProduction(
            GetSpawnedCount()
        );

        SetState(FactoryGameState.Result);
    }

    public void Retry()
    {
        SetState(FactoryGameState.Build);

        physicsWorld.ResetSimulation();

        foreach (ProductSpawner spawner in spawners)
        {
            if (spawner != null)
                spawner.ResetSpawner();
        }

        scoreManager.ResetScore();

        RemainingTime = runDuration;
    }

    public void ResetAll()
    {
        Retry();

        buildManager.ResetAllDevices();
    }

    private void SetState(FactoryGameState newState)
    {
        state = newState;

        bool isRunning =
            state == FactoryGameState.Running;

        bool isBuild =
            state == FactoryGameState.Build;

        physicsWorld.isSimulating = isRunning;

        buildManager.SetBuildEnabled(isBuild);

        foreach (ProductSpawner spawner in spawners)
        {
            if (spawner != null)
                spawner.isSpawning = isRunning;
        }

        OnStateChanged?.Invoke(state);
    }

    public int GetSpawnedCount()
    {
        int count = 0;

        foreach (ProductSpawner spawner in spawners)
        {
            if (spawner != null)
                count += spawner.spawnedCount;
        }

        return count;
    }

    public int GetTotalProductCount()
    {
        int count = 0;

        foreach (ProductSpawner spawner in spawners)
        {
            if (spawner != null)
                count += spawner.maxSpawnCount;
        }

        return count;
    }

    private bool AllSpawnersFinished()
    {
        foreach (ProductSpawner spawner in spawners)
        {
            if (spawner == null)
                continue;

            if (spawner.spawnedCount <
                spawner.maxSpawnCount)
                return false;
        }

        return true;
    }
}
