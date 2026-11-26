
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewDeviceData",
    menuName = "Factory/Device Data")]
public class DeviceData : ScriptableObject
{
    public string deviceName;

    public GameObject prefab;

    public Sprite icon;

    public int cost = 20;

    public bool canRotate = true;
}
