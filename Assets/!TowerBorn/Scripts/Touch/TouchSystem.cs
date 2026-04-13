using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[Serializable]
public class TouchData
{
    public Vector2 Position { get; set; }
    public bool IsActive { get; set; }
    public float StartTime { get; set; }
    public Vector2 StartPosition { get; set; }

    public float Duration => Time.unscaledTime - StartTime;
    public Vector2 DeltaFromStart => Position - StartPosition;
    public float DistanceFromStart => DeltaFromStart.magnitude;

    public void StartTouch(Vector2 position)
    {
        Position = position;
        StartPosition = position;
        IsActive = true;
        StartTime = Time.unscaledTime;
    }

    public void UpdateTouch(Vector2 position)
    {
        Position = position;
    }

    public void EndTouch()
    {
        IsActive = false;
        Position = Vector2.zero;
        StartPosition = Vector2.zero;
        StartTime = 0f;
    }
}

public enum GestureType
{
    Tap,
    Hold,
    Drag,
    Pinch
}

public class TouchEventArgs : EventArgs
{
    public Vector2 Position { get; }
    public Vector2 Delta { get; }
    public Building HoveredBuilding { get; }

    public TouchEventArgs(Vector2 position, Vector2 delta = default, Building hoveredBuilding = null)
    {
        Position = position;
        Delta = delta;
        HoveredBuilding = hoveredBuilding;
    }
}

public class GestureEventArgs : EventArgs
{
    public GestureType GestureType { get; }
    public Vector2 Position { get; }
    public Vector2 Delta { get; }
    public float PinchDelta { get; }
    public Building HoveredBuilding { get; }

    public GestureEventArgs(GestureType gestureType, Vector2 position, Vector2 delta = default,
        float pinchDelta = 0f, Building hoveredBuilding = null)
    {
        GestureType = gestureType;
        Position = position;
        Delta = delta;
        PinchDelta = pinchDelta;
        HoveredBuilding = hoveredBuilding;
    }
}

public class TouchSystem : MonoBehaviour
{
    #region Singleton
    public static TouchSystem Instance { get; private set; }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    #region Events
    public event EventHandler<TouchEventArgs> OnTouchDown;
    public event EventHandler<TouchEventArgs> OnTouchUp;

    // События жестов
    public event EventHandler<GestureEventArgs> OnTap;
    public event EventHandler<GestureEventArgs> OnHold;
    public event EventHandler<GestureEventArgs> OnDrag;
    public event EventHandler<GestureEventArgs> OnPinch;

    // Unity Events для Inspector
    public UnityEvent<Building> OnBuildingTap = new UnityEvent<Building>();
    #endregion

    #region Configuration
    [Header("Gesture Thresholds")]
    [SerializeField] private float tapMaxDuration = 0.3f;
    [SerializeField] private float holdMinDuration = 1f;
    [SerializeField] private float dragMinDistance = 5f;
    [SerializeField] private float pinchSensitivity = 0.01f;

    [Header("Raycasting")]
    [SerializeField] private float maxRaycastDistance = 30f;
    #endregion

    #region Touch State
    private readonly TouchData[] fingers = new TouchData[2];
    private Vector2 lastTouchDelta = Vector2.zero;
    private float lastPinchDistance = 0f;

    // Gesture flags
    private bool isDragging = false;
    private bool isHoldTriggered = false;
    private bool isPinching = false;

    // UI and building detection
    private bool isTouchOverUI = false;
    private Building hoveredBuilding = null;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeSingleton();
        InitializeTouchData();
        EnableTouchSupport();
    }

    private void OnDestroy()
    {
        DisableTouchSupport();
    }

    private void Update()
    {
        ProcessTouches();
        CheckHoldGesture();
    }
    #endregion

    #region Initialization
    private void InitializeTouchData()
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            fingers[i] = new TouchData();
        }
    }

    private void EnableTouchSupport()
    {
        if (!EnhancedTouchSupport.enabled)
        {
            EnhancedTouchSupport.Enable();
        }
    }

    private void DisableTouchSupport()
    {
        if (EnhancedTouchSupport.enabled)
        {
            EnhancedTouchSupport.Disable();
        }
    }
    #endregion

    #region Touch Processing
    private void ProcessTouches()
    {
        var activeTouches = Touch.activeTouches;
        int touchCount = activeTouches.Count;

        // Обрабатываем активные касания
        for (int i = 0; i < Mathf.Min(touchCount, fingers.Length); i++)
        {
            var touch = activeTouches[i];
            ProcessTouch(i, touch.screenPosition);
        }

        // Завершаем касания, которые больше не активны
        for (int i = touchCount; i < fingers.Length; i++)
        {
            if (fingers[i].IsActive)
            {
                EndTouch(i);
            }
        }

        // Обрабатываем жесты
        ProcessGestures(touchCount);
    }

    private void ProcessTouch(int fingerIndex, Vector2 position)
    {
        if (!IsValidTouchPosition(position))
            return;

        var finger = fingers[fingerIndex];

        if (!finger.IsActive)
        {
            StartTouch(fingerIndex, position);
        }
        else
        {
            UpdateTouch(fingerIndex, position);
        }
    }

    private void StartTouch(int fingerIndex, Vector2 position)
    {
        var finger = fingers[fingerIndex];
        finger.StartTouch(position);

        // Только для первого пальца
        if (fingerIndex == 0)
        {
            isTouchOverUI = IsTouchOverUI(position);

            if (!isTouchOverUI)
            {
                DetectHoveredBuilding(position);
            }

            InvokeTouchDown(position);
        }
    }

    private void UpdateTouch(int fingerIndex, Vector2 position)
    {
        var finger = fingers[fingerIndex];
        var previousPosition = finger.Position;
        finger.UpdateTouch(position);

        // Только для первого пальца обновляем дельту
        if (fingerIndex == 0)
        {
            lastTouchDelta = position - previousPosition;
        }
    }

    private void EndTouch(int fingerIndex)
    {
        var finger = fingers[fingerIndex];

        if (fingerIndex == 0)
        {
            HandlePrimaryTouchEnd();
            InvokeTouchUp(finger.Position);
        }
        else if (fingerIndex == 1)
        {
            HandleSecondaryTouchEnd();
        }

        finger.EndTouch();
    }

    private void HandlePrimaryTouchEnd()
    {
        var finger = fingers[0];

        // Проверяем на Tap только если не было других жестов
        if (!isDragging && !isHoldTriggered && !isTouchOverUI &&
            finger.Duration < tapMaxDuration)
        {
            ProcessTapGesture();
        }

        // Сбрасываем состояние
        ResetGestureState();
    }

    private void HandleSecondaryTouchEnd()
    {
        isPinching = false;
        lastPinchDistance = 0f;
    }

    private void ResetGestureState()
    {
        isDragging = false;
        isHoldTriggered = false;
        isPinching = false;
        isTouchOverUI = false;
        hoveredBuilding = null;
        lastTouchDelta = Vector2.zero;

        StopCameraDragging();
    }
    #endregion

    #region Gesture Processing
    private void ProcessGestures(int touchCount)
    {
        if (touchCount >= 2)
        {
            ProcessPinchGesture();
        }
        else if (touchCount == 1 && fingers[0].IsActive)
        {
            ProcessDragGesture();
        }
    }

    private void CheckHoldGesture()
    {
        var finger = fingers[0];
        if (!finger.IsActive || isHoldTriggered || isTouchOverUI)
            return;

        if (finger.Duration >= holdMinDuration &&
            finger.DistanceFromStart <= dragMinDistance)
        {
            ProcessHoldGesture();
        }
    }

    private void ProcessTapGesture()
    {
        var eventArgs = new GestureEventArgs(
            GestureType.Tap,
            fingers[0].Position,
            hoveredBuilding: hoveredBuilding);

        OnTap?.Invoke(this, eventArgs);

        if (hoveredBuilding != null)
        {
            OnBuildingTap?.Invoke(hoveredBuilding);
        }
    }

    private void ProcessHoldGesture()
    {
        isHoldTriggered = true;

        var eventArgs = new GestureEventArgs(
            GestureType.Hold,
            fingers[0].Position,
            hoveredBuilding: hoveredBuilding);

        OnHold?.Invoke(this, eventArgs);

        //TryStartMovingBuilding();
    }

    private void ProcessDragGesture()
    {
        var finger = fingers[0];

        if (finger.DistanceFromStart <= dragMinDistance)
            return;

        if (!isDragging)
        {
            isDragging = true;
        }

        if (IsMovingBuilding())
        {
            UpdateBuildingPosition();
        }
        else if (!isTouchOverUI)
        {
            UpdateCameraPosition();
        }

        var eventArgs = new GestureEventArgs(
            GestureType.Drag,
            finger.Position,
            lastTouchDelta,
            hoveredBuilding: hoveredBuilding);

        OnDrag?.Invoke(this, eventArgs);
    }

    private void ProcessPinchGesture()
    {
        if (!AreValidTouchPositions(fingers[0].Position, fingers[1].Position))
            return;

        float currentDistance = Vector2.Distance(fingers[0].Position, fingers[1].Position);

        if (!isPinching)
        {
            isPinching = true;
            lastPinchDistance = currentDistance;
            return;
        }

        if (lastPinchDistance > 0)
        {
            float pinchDelta = (currentDistance - lastPinchDistance) * pinchSensitivity;
            Vector2 pinchCenter = (fingers[0].Position + fingers[1].Position) * 0.5f;

            UpdateCameraZoom(-pinchDelta);

            var eventArgs = new GestureEventArgs(
                GestureType.Pinch,
                pinchCenter,
                pinchDelta: pinchDelta);

            OnPinch?.Invoke(this, eventArgs);
        }

        lastPinchDistance = currentDistance;
    }
    #endregion

    #region Validation
    private bool IsValidTouchPosition(Vector2 position)
    {
        return !float.IsInfinity(position.x) && !float.IsInfinity(position.y) &&
               !float.IsNaN(position.x) && !float.IsNaN(position.y);
    }

    private bool AreValidTouchPositions(Vector2 pos1, Vector2 pos2)
    {
        return IsValidTouchPosition(pos1) && IsValidTouchPosition(pos2);
    }
    #endregion

    #region UI Detection
    private bool IsTouchOverUI(Vector2 touchPosition)
    {
        if (EventSystem.current == null)
            return false;

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = touchPosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
    #endregion

    #region Building Detection
    private void DetectHoveredBuilding(Vector2 screenPosition)
    {
        hoveredBuilding = null;

        if (Camera.main == null || BuildingPlacer.Instance == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * maxRaycastDistance, Color.blue, 1f);
#endif

        if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance,
            BuildingPlacer.Instance.BuildingLayer))
        {
            hoveredBuilding = hit.transform.GetComponentInParent<Building>();
        }
    }
    #endregion

    #region Camera Integration
    private void UpdateCameraPosition()
    {
        CameraController.Instance?.SetTargetPos(lastTouchDelta);
    }

    private void UpdateCameraZoom(float zoomDelta)
    {
        CameraController.Instance?.SetZoomValue(zoomDelta);
    }

    private void StopCameraDragging()
    {
        CameraController.Instance?.StopDragging();
    }
    #endregion

    #region Building Integration

    private bool IsMovingBuilding()
    {
        return hoveredBuilding != null && BuildingPlacer.Instance != null &&
               BuildingPlacer.Instance.CompareMovingBuilding(hoveredBuilding);
    }

    private void UpdateBuildingPosition()
    {
        BuildingPlacer.Instance.UpdateBuildingPos(fingers[0].Position);
    }
    #endregion

    #region Event Invocation
    private void InvokeTouchDown(Vector2 position)
    {
        var eventArgs = new TouchEventArgs(position, hoveredBuilding: hoveredBuilding);
        OnTouchDown?.Invoke(this, eventArgs);
    }

    private void InvokeTouchUp(Vector2 position)
    {
        var eventArgs = new TouchEventArgs(position, lastTouchDelta, hoveredBuilding);
        OnTouchUp?.Invoke(this, eventArgs);
    }
    #endregion

    #region Public API
    /// <summary>
    /// Получить позицию основного касания
    /// </summary>
    public Vector2 GetPrimaryTouchPosition() => fingers[0].Position;

    /// <summary>
    /// Получить дельту последнего движения касания
    /// </summary>
    public Vector2 GetTouchDelta() => lastTouchDelta;

    /// <summary>
    /// Получить начальную позицию касания
    /// </summary>
    public Vector2 GetTouchStartPosition() => fingers[0].StartPosition;

    /// <summary>
    /// Проверить активность основного касания
    /// </summary>
    public bool IsPrimaryTouchActive() => fingers[0].IsActive;

    /// <summary>
    /// Получить здание под курсором
    /// </summary>
    public Building GetHoveredBuilding() => hoveredBuilding;

    /// <summary>
    /// Получить количество активных касаний
    /// </summary>
    public int GetActiveTouchCount()
    {
        int count = 0;
        for (int i = 0; i < fingers.Length; i++)
        {
            if (fingers[i].IsActive) count++;
        }
        return count;
    }

    /// <summary>
    /// Проверить, активен ли жест перетаскивания
    /// </summary>
    public bool IsDragging() => isDragging;

    /// <summary>
    /// Проверить, активен ли жест пинча
    /// </summary>
    public bool IsPinching() => isPinching;
    #endregion
}