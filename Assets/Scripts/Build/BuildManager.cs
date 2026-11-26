
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

// deviceData 선택, ghost 생성, 마우스 위치 추적, 설치 가능 여부 검사, 실제 장치 생성

public class BuildManager : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public PhysicsWorld physicsWorld;

    [Header("Available Devices")]
    public DeviceData[] availableDevices;

    [Header("Build Settings")]
    public float gridSize = 0.5f;
    public float rotationStep = 15f;

    public Vector2 buildMin = new Vector2(-8, -4);
    public Vector2 buildMax = new Vector2(8, 4);

    public bool canBuild = true;

    private DeviceData selectedData;
    private GameObject ghost;
    private PhysicsBox ghostBox;
    private SpriteRenderer ghostRenderer;

    private float currentRotation;
    private bool validPlacement;

    private PlacedDevice selectedDevice;

    private List<PlacedDevice> placedDevices =
        new List<PlacedDevice>();

    public BudgetManager budgetManager;

    public event Action<PlacedDevice> OnDeviceSelected;

    public PlacedDevice SelectedDevice =>
        selectedDevice;

    private void Update()
    {
        if (!canBuild)
            return;

        HandleInput();

        if (ghost != null)
            UpdateGhost();

        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())
                return;

            if (ghost != null)
                PlaceDevice();
            else
                SelectPlacedDevice();
        }
    }

    private void HandleInput()
    {
        // 숫자키 1~5
        for (int i = 0;
             i < Mathf.Min(5, availableDevices.Length);
             i++)
        {
            KeyCode key =
                (KeyCode)((int)KeyCode.Alpha1 + i);

            if (Input.GetKeyDown(key))
                SelectDevice(i);
        }

        if (Input.GetKeyDown(KeyCode.Q))
            RotateDevice(-rotationStep);

        if (Input.GetKeyDown(KeyCode.E))
            RotateDevice(rotationStep);

        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
            ClearSelection();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
            DeleteSelectedDevice();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveSelectedDevice(Vector2.up);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveSelectedDevice(Vector2.down);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveSelectedDevice(Vector2.left);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveSelectedDevice(Vector2.right);
    }

    public void SelectDevice(int index)
    {
        if (!canBuild ||
            index < 0 ||
            index >= availableDevices.Length)
            return;

        CancelPlacement();
        ClearSelection();

        selectedData = availableDevices[index];

        if (selectedData == null ||
            selectedData.prefab == null)
            return;

        currentRotation = 0f;

        CreateGhost();
        UpdateGhost();
    }

    private void CreateGhost()
    {
        // 미리보기용 오브젝트는 직접 생성
        // 실제 장치 스크립트는 포함하지 않음
        ghost = new GameObject("DeviceGhost");

        SpriteRenderer sourceRenderer =
            selectedData.prefab
                .GetComponent<SpriteRenderer>();

        ghostRenderer =
            ghost.AddComponent<SpriteRenderer>();

        ghostRenderer.sprite = sourceRenderer.sprite;
        ghostRenderer.color =
            new Color(0f, 1f, 0f, 0.4f);

        ghostRenderer.sortingOrder = 20;

        ghost.transform.localScale =
            selectedData.prefab.transform.localScale;

        PhysicsBox sourceBox =
            selectedData.prefab.GetComponent<PhysicsBox>();

        if (sourceBox != null)
        {
            ghostBox = ghost.AddComponent<PhysicsBox>();
            ghostBox.size = sourceBox.size;
        }
    }

    private void UpdateGhost()
    {
        ghost.transform.position = GetMouseWorldPosition();

        ghost.transform.rotation =
            Quaternion.Euler(0, 0, currentRotation);

        validPlacement = ValidatePlacement(ghost, ghostBox);

        ghostRenderer.color = validPlacement
            ? new Color(0f, 1f, 0f, 0.4f)
            : new Color(1f, 0f, 0f, 0.4f);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouse = Input.mousePosition;

        mouse.z = -mainCamera.transform.position.z;

        Vector3 position =
            mainCamera.ScreenToWorldPoint(mouse);

        position.z = 0f;

        float size = Mathf.Max(0.01f, gridSize);

        position.x =
            Mathf.Round(position.x / size) * size;

        position.y =
            Mathf.Round(position.y / size) * size;

        return position;
    }

    private bool ValidatePlacement(
        GameObject target,
        PhysicsBox targetBox)
    {

        if (target == ghost &&
            selectedData != null &&
            !budgetManager.CanAfford(selectedData.cost))
        {
            return false;
        }

        Vector2 center = target.transform.position;

        // 회전된 사각형의 월드 X/Y 범위
        Vector2 extent;

        if (targetBox != null)
        {
            Vector2 x = targetBox.AxisX;
            Vector2 y = targetBox.AxisY;
            Vector2 half = targetBox.HalfSize;

            extent = new Vector2(
                Mathf.Abs(x.x) * half.x +
                Mathf.Abs(y.x) * half.y,

                Mathf.Abs(x.y) * half.x +
                Mathf.Abs(y.y) * half.y
            );
        }
        else
        {
            SpriteRenderer sr =
                target.GetComponent<SpriteRenderer>();

            extent = sr != null
                ? (Vector2)sr.bounds.extents
                : Vector2.zero;
        }

        // 장치 전체가 설치 영역 안에 있는지 확인
        if (center.x - extent.x < buildMin.x ||
            center.x + extent.x > buildMax.x ||
            center.y - extent.y < buildMin.y ||
            center.y + extent.y > buildMax.y)
            return false;

        // 물리 사각형끼리 SAT 검사
        if (targetBox != null)
        {
            foreach (PhysicsBox box in physicsWorld.Boxes)
            {
                if (box == null ||
                    box == targetBox ||
                    !box.isActiveAndEnabled)
                    continue;

                if (CollisionDetector.OBBOverlap(
                    targetBox, box))
                    return false;
            }
        }

        // 자석 등 비물리 장치를 포함한 배치 검사
        SpriteRenderer targetSprite =
            target.GetComponent<SpriteRenderer>();

        foreach (PlacedDevice device in placedDevices)
        {
            if (device == null ||
                device.gameObject == target)
                continue;

            PhysicsBox otherBox =
                device.GetComponent<PhysicsBox>();

            if (targetBox != null && otherBox != null)
                continue;

            SpriteRenderer otherSprite =
                device.GetComponent<SpriteRenderer>();

            if (targetSprite != null &&
                otherSprite != null &&
                targetSprite.bounds.Intersects(
                    otherSprite.bounds))
                return false;
        }

        return true;
    }


    private void PlaceDevice()
    {
        if (!validPlacement ||
            selectedData == null)
            return;

        if (!budgetManager.TrySpend(selectedData.cost))
            return;

        GameObject obj = Instantiate(
            selectedData.prefab,
            ghost.transform.position,
            ghost.transform.rotation
        );

        PlacedDevice device =
            obj.GetComponent<PlacedDevice>();

        if (device == null)
            device = obj.AddComponent<PlacedDevice>();

        device.Initialize(
            selectedData, physicsWorld);

        placedDevices.Add(device);

        UpdateGhost();
    }


    private void SelectPlacedDevice()
    {
        ClearSelection();

        Vector3 mouse = Input.mousePosition;
        mouse.z = -mainCamera.transform.position.z;

        Vector3 position =
            mainCamera.ScreenToWorldPoint(mouse);

        position.z = 0f;

        for (int i = placedDevices.Count - 1;
             i >= 0; i--)
        {
            PlacedDevice device = placedDevices[i];

            if (device == null)
                continue;

            PhysicsBox box =
                device.GetComponent<PhysicsBox>();

            if (box != null)
            {
                Vector2 diff =
                    (Vector2)position - box.Center;

                float x = Vector2.Dot(diff, box.AxisX);
                float y = Vector2.Dot(diff, box.AxisY);

                if (Mathf.Abs(x) > box.HalfSize.x ||
                    Mathf.Abs(y) > box.HalfSize.y)
                    continue;
            }
            else
            {
                SpriteRenderer sr =
                    device.GetComponent<SpriteRenderer>();

                if (sr == null ||
                    !sr.bounds.Contains(position))
                    continue;
            }

            selectedDevice = device;


            selectedDevice.SetSelected(true);

            OnDeviceSelected?.Invoke(selectedDevice);

            Debug.Log(
                "Selected: " +
                selectedDevice.Data.deviceName
            );

            break;
        }
    }

    private void RotateDevice(float angle)
    {
        if (ghost != null &&
            selectedData != null &&
            selectedData.canRotate)
        {
            currentRotation += angle;
            return;
        }

        if (selectedDevice != null &&
            selectedDevice.Data.canRotate)
        {
            Transform target = selectedDevice.transform;

            Quaternion previousRotation =
                target.rotation;

            target.Rotate(0, 0, angle);

            PhysicsBox box =
                selectedDevice.GetComponent<PhysicsBox>();

            if (!ValidatePlacement(
                selectedDevice.gameObject, box))
            {
                target.rotation = previousRotation;
            }
        }
    }


    public void DeleteSelectedDevice()
    {
        if (!canBuild || selectedDevice == null)
            return;

        PlacedDevice device = selectedDevice;

        int refund = device.Data.cost;

        ClearSelection();

        placedDevices.Remove(device);

        device.RemoveDevice();

        budgetManager.Refund(refund);
    }


    public void ClearSelection()
    {
        if (selectedDevice != null)
            selectedDevice.SetSelected(false);

        selectedDevice = null;

        OnDeviceSelected?.Invoke(null);
    }

    public void CancelPlacement()
    {
        if (ghost != null)
            Destroy(ghost);

        ghost = null;
        ghostBox = null;
        ghostRenderer = null;
        selectedData = null;
        validPlacement = false;
    }

    public void SetBuildEnabled(bool value)
    {
        canBuild = value;

        if (!value)
        {
            CancelPlacement();
            ClearSelection();
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector2 center =
            (buildMin + buildMax) * 0.5f;

        Vector2 size = buildMax - buildMin;

        Gizmos.DrawWireCube(
            (Vector3)center,
            (Vector3)size
        );
    }


    private void MoveSelectedDevice(Vector2 direction)
    {
        if (!canBuild || selectedDevice == null)
            return;

        Transform target = selectedDevice.transform;

        Vector3 previousPosition = target.position;

        target.position +=
            (Vector3)(direction * gridSize);

        PhysicsBox box =
            selectedDevice.GetComponent<PhysicsBox>();

        if (!ValidatePlacement(
            selectedDevice.gameObject, box))
        {
            target.position = previousPosition;
        }
    }
}
