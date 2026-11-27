
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewStageData",
    menuName = "Factory/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("Stage Information")]
    public int stageID;

    public string stageName;

    [TextArea(2, 5)]
    public string description;

    [Header("Stage Rules")]
    public int initialBudget = 100;

    public float runDuration = 40f;

    public int requiredProducts = 20;

    [Header("Available Devices")]
    public DeviceData[] availableDevices;

    [Header("Scene Transition")]
    public string nextSceneName;
}
