
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeviceToolbarUI : MonoBehaviour
{
    public BuildManager buildManager;

    public Button[] buttons;
    public TMP_Text[] buttonTexts;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        DeviceData[] devices =
            buildManager.availableDevices;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            bool available =
                i < devices.Length &&
                devices[i] != null;

            buttons[i].gameObject.SetActive(
                available
            );

            buttons[i].onClick.RemoveAllListeners();

            if (!available)
                continue;

            DeviceData data = devices[i];

            buttonTexts[i].text =
                $"[{i + 1}] {data.deviceName}" +
                $"\n{data.cost} G";

            buttons[i].onClick.AddListener(
                () => buildManager.SelectDevice(index)
            );
        }
    }
}
