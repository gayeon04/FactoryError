
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeviceSettingsUI : MonoBehaviour
{
    [Header("References")]
    public BuildManager buildManager;

    [Header("Panel")]
    public GameObject settingsPanel;

    public TMP_Text deviceNameText;
    public TMP_Text deviceCostText;

    [Header("Slider 1")]
    public GameObject sliderGroup1;
    public TMP_Text labelText1;
    public TMP_Text valueText1;
    public Slider slider1;

    [Header("Slider 2")]
    public GameObject sliderGroup2;
    public TMP_Text labelText2;
    public TMP_Text valueText2;
    public Slider slider2;

    private PlacedDevice currentDevice;

    private Action<float> setter1;
    private Action<float> setter2;

    private string unit1 = "";
    private string unit2 = "";

    private void OnEnable()
    {
        buildManager.OnDeviceSelected +=
            ShowDevice;

        slider1.onValueChanged.AddListener(
            OnSlider1Changed);

        slider2.onValueChanged.AddListener(
            OnSlider2Changed);
    }

    private void Start()
    {
        ShowDevice(buildManager.SelectedDevice);
    }

    private void OnDisable()
    {
        buildManager.OnDeviceSelected -=
            ShowDevice;

        slider1.onValueChanged.RemoveListener(
            OnSlider1Changed);

        slider2.onValueChanged.RemoveListener(
            OnSlider2Changed);
    }

    private void ShowDevice(PlacedDevice device)
    {
        currentDevice = device;

        setter1 = null;
        setter2 = null;

        sliderGroup1.SetActive(false);
        sliderGroup2.SetActive(false);

        if (device == null)
        {
            settingsPanel.SetActive(false);
            return;
        }

        settingsPanel.SetActive(true);

        deviceNameText.text =
            device.Data.deviceName;

        deviceCostText.text =
            $"{device.Data.cost} G";

        // Conveyor
        if (device.TryGetComponent<Conveyor>(
            out Conveyor conveyor))
        {
            SetupSlider1(
                "이동 속력",
                -8f, 8f,
                conveyor.beltSpeed,
                " m/s",
                value => conveyor.beltSpeed = value
            );

            SetupSlider2(
                "최대 가속도",
                1f, 30f,
                conveyor.maxAcceleration,
                " m/s²",
                value =>
                    conveyor.maxAcceleration = value
            );
        }

        // Launcher
        else if (device.TryGetComponent<Launcher>(
            out Launcher launcher))
        {
            SetupSlider1(
                "발사 각도",
                0f, 85f,
                launcher.launchAngle,
                "°",
                value => launcher.launchAngle = value
            );

            SetupSlider2(
                "발사 속력",
                3f, 16f,
                launcher.launchSpeed,
                " m/s",
                value => launcher.launchSpeed = value
            );
        }

        // Magnet
        else if (device.TryGetComponent<Magnet>(
            out Magnet magnet))
        {
            SetupSlider1(
                "자력 세기",
                0f, 150f,
                magnet.strength,
                "",
                value => magnet.strength = value
            );

            SetupSlider2(
                "영향 범위",
                1f, 6f,
                magnet.range,
                " m",
                value => magnet.range = value
            );
        }

        // FrictionPlate
        else if (
            device.TryGetComponent<FrictionPlate>(
                out FrictionPlate plate))
        {
            SetupSlider1(
                "마찰계수",
                0f, 1f,
                plate.friction,
                "",
                value => plate.friction = value
            );
        }

        // Reflector
        else if (device.TryGetComponent<PhysicsBox>(
            out PhysicsBox box))
        {
            SetupSlider1(
                "반발계수",
                0f, 1f,
                box.restitution,
                "",
                value => box.restitution = value
            );
        }
    }

    private void SetupSlider1(
        string label,
        float min,
        float max,
        float current,
        string unit,
        Action<float> setter)
    {
        sliderGroup1.SetActive(true);

        labelText1.text = label;

        setter1 = null;

        slider1.minValue = min;
        slider1.maxValue = max;

        slider1.SetValueWithoutNotify(current);

        unit1 = unit;
        setter1 = setter;

        valueText1.text =
            slider1.value.ToString("0.##") + unit;
    }

    private void SetupSlider2(
        string label,
        float min,
        float max,
        float current,
        string unit,
        Action<float> setter)
    {
        sliderGroup2.SetActive(true);

        labelText2.text = label;

        setter2 = null;

        slider2.minValue = min;
        slider2.maxValue = max;

        slider2.SetValueWithoutNotify(current);

        unit2 = unit;
        setter2 = setter;

        valueText2.text =
            slider2.value.ToString("0.##") + unit;
    }

    private void OnSlider1Changed(float value)
    {
        if (currentDevice == null ||
            !buildManager.canBuild ||
            setter1 == null)
            return;

        setter1.Invoke(value);

        valueText1.text =
            value.ToString("0.##") + unit1;
    }

    private void OnSlider2Changed(float value)
    {
        if (currentDevice == null ||
            !buildManager.canBuild ||
            setter2 == null)
            return;

        setter2.Invoke(value);

        valueText2.text =
            value.ToString("0.##") + unit2;
    }

    public void DeleteDevice()
    {
        buildManager.DeleteSelectedDevice();
    }

    public void ClosePanel()
    {
        buildManager.ClearSelection();
    }
}
