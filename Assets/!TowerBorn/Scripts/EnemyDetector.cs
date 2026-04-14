using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyDetector
{
    private IBuilding _ownerBuilding;

    [Header("Detection Settings")]
    private float _detectionRadius = 10f;
    private float _targetSearchInterval = 0.2f;

    private List<BaseUnit> _targets = new List<BaseUnit>();
    private float _lastSearchTime;

    public List<BaseUnit> Targets => _targets;
    public BaseUnit CurrentTarget => _targets.Count > 0 ? _targets[0] : null;
    public bool IsDetecting { get; private set; }
    public float LastDetectionTime { get; private set; }

    public delegate void TargetChangeEvent(BaseUnit newTarget);
    public event TargetChangeEvent OnTargetChanged;

    private BaseUnit _previousTarget;

    private Vector3 _cachedPosition;
    private float _cachedRadiusSquared;

    public void Init(IBuilding building)
    {
        _ownerBuilding = building;
    }

    public void SetRadius(float radius)
    {
        _detectionRadius = radius;
        _cachedRadiusSquared = radius * radius;
    }

    public void UpdateTick()
    {
        if (!IsDetecting) return;
        if (Time.fixedTime - _lastSearchTime >= _targetSearchInterval)
        {
            FindTargetsFromManager();
            _lastSearchTime = Time.fixedTime;
        }
    }

    public void StartDetection()
    {
        if (IsDetecting) return;
        IsDetecting = true;
        _lastSearchTime = 0f;
    }

    public void StopDetection()
    {
        if (!IsDetecting) return;
        IsDetecting = false;
        _targets.Clear();
        _previousTarget = null;
    }

    public void DetectEnemiesNow()
    {
        FindTargetsFromManager();
    }

    private void FindTargetsFromManager()
    {
        LastDetectionTime = Time.fixedTime;
        _cachedPosition = _ownerBuilding.Position;

        if (EnemyManager.Instance?.IsInitialized != true)
        {
            _targets.Clear();
            return;
        }

        var allActiveUnits = EnemyManager.Instance.GetActiveUnits();
        var targetsTemp = new List<BaseUnit>();
        var radiusSqr = _detectionRadius * _detectionRadius;

        BaseUnit closestUnit = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (var unit in allActiveUnits)
        {
            if (!IsValidTarget(unit)) continue;

            float distanceSqr = Vector3.SqrMagnitude(_cachedPosition - unit.Position);
            if (distanceSqr <= radiusSqr)
            {
                targetsTemp.Add(unit);
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestUnit = unit;
                }
            }
        }

        if (closestUnit != _previousTarget)
        {
            _previousTarget = closestUnit;
            OnTargetChanged?.Invoke(closestUnit);
        }

        _targets = targetsTemp;
    }

    private bool IsValidTarget(BaseUnit unit)
    {
        return unit != null &&
               unit.gameObject.activeInHierarchy &&
               unit.enabled &&
               unit.DamageableState != DamageableState.Invincible;
    }

    public BaseUnit GetClosestTarget()
    {
        return CurrentTarget;
    }

    public BaseUnit GetFarthestTarget()
    {
        return _targets.Count > 0 ? _targets[_targets.Count - 1] : null;
    }

    public int GetTargetCount()
    {
        return _targets.Count;
    }

    public bool HasTargets()
    {
        return _targets.Count > 0;
    }

    public BaseUnit GetTargetByIndex(int index)
    {
        if (index >= 0 && index < _targets.Count)
            return _targets[index];
        return null;
    }
}