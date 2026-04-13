using System;
using System.Collections;
using UnityEngine;
using TowerBorn.SaveSystem;
using static UnityEngine.UI.ScrollRect;
using Zenject;
using static UnityEngine.Rendering.STP;

public enum BuildingState
{
    Idle, Attack
}
public class Building : MonoBehaviour, IBuilding
{
    public int InstanceId { get; private set; }
    public BuildingConfig BuildingConfig { get; private set; }

    public Vector3 Position { get; private set; }
    [Inject] public BuildingStatsController StatsController { get; private set; }

    [Inject] protected BuildingsManager _buildingsManager;

    [Header("Core Modules")]

    public BuildingAudioController BuildingAudioController { get; private set; }
    public BuildingTransparency BuildingTransparency { get; private set; }

    public BuildingUI BuildingUI { get; private set; }
    public BuildingAnimator BuildingAnimator { get; private set; }

    public Vector3Int PlacementCell { get; private set; }

    [Header("Appearance")]
    [SerializeField] Animation _spawnAnimation;
    public BuildingBoundaries BuildingBoundaries { get; private set; }
    public BuildingVFXController BuildingVFXController { get; private set; }

    [Header("Other")] 
    public Collider Collider { get; private set; }
    public bool IsInitialized { get; private set; } = false;
    public BuildingState CombatState => CombatStateMachine.CurrentState.StateType;

    [Inject] public AbilitiesController AbilitiesController { get; private set; }

    public CombatStateMachine CombatStateMachine { get; private set; }
    public CombatWeaponController CombatWeaponController { get; private set; }
    public BaseUnit Target { get; private set; }


    #region EVENTS
    public event Action<int> OnUpgrade;
    public event Action<Buff> OnBuffAdded;
    public event Action<Buff> OnBuffRemoved;
    #endregion

    private void Awake()
    {
        InstanceId = GetInstanceID();
    }

    public virtual void Init()
    {
        _buildingsManager.AddBuilding(this);

        _spawnAnimation.Play();
        BuildingTransparency.RestoreOriginalMaterials();

        OnUpgrade += (int level) =>
        {
            BuildingVFXController.PlayUpgradeVFX();
            BuildingUI.SetLevelValue(level);
            CombatWeaponController.EnemyDetector.SetRadius(StatsController.TotalAtkRange);
            BuildingAnimator.SetAttackSpeed(StatsController.BaseAtkInterval / StatsController.TotalAtkInterval);

            Debug.Log($"Upgraded building, base atk speed: {StatsController.AtkSpeed}, total atk speed: {StatsController.TotalAtkSpeed}");
        };

        OnBuffAdded += (Buff buff) =>
        {
            if (buff.type == BuffType.AtkSpeed)
            {
                BuildingAnimator.SetAttackSpeed(StatsController.BaseAtkInterval / StatsController.TotalAtkInterval);
            }
        };

        CombatStateMachine.ChangeState(BuildingState.Idle);

        IsInitialized = true;
    }

    public virtual void SetData(BuildingConfig config)
    {
        BuildingConfig = config;
        SetupComponents();

        StatsController.CalculateStats(BuildingConfig.Level);
      
        if(CombatWeaponController != null)
        {
            CombatWeaponController.EnemyDetector.SetRadius(StatsController.TotalAtkRange);
        }

        if(BuildingBoundaries != null)
        {
            BuildingBoundaries.SetAtkRangeSize(StatsController.TotalAtkRange);
        }
    }

    protected virtual void SetupComponents()
    {
        StatsController.Init(BuildingConfig.CombatStats);

        BuildingUI = GetRequiredComponent<BuildingUI>((comp) =>
        {
            comp.SetLevelBackgroundTier((int)BuildingConfig.BuildingTier);
        });

        Collider = GetRequiredComponent<Collider>();

        BuildingAnimator = GetRequiredComponent<BuildingAnimator>((comp) =>
        {
            comp.Init(this);
        });

        BuildingTransparency = GetRequiredComponent<BuildingTransparency>((comp) =>
        {
            comp.CollectMeshRenderers();
        }); 

        BuildingAudioController = GetRequiredComponent<BuildingAudioController>((comp) =>
        {
            comp.Init(this);
        }); 


        BuildingBoundaries = GetRequiredComponent<BuildingBoundaries>();

        BuildingVFXController = GetRequiredComponent<BuildingVFXController>((comp) =>
        {
            comp.Init(this);
        });

        CombatWeaponController = GetRequiredComponent<CombatWeaponController>((comp) =>
        {
            comp.Init(this);
        });

        CombatStateMachine = new CombatStateMachine();
        IState idleState = new IdleState(this, BuildingState.Idle);
        IState attackState = new AttackState(this, BuildingState.Attack);
        CombatStateMachine.AddState(idleState);
        CombatStateMachine.AddState(attackState);

        Transform abilitiesParent = transform.Find("Abilities");
        AbilitiesController.Init(this, abilitiesParent);
    }
    private T GetRequiredComponent<T>(Action<T> onSuccess = null) where T : Component
    {
        T component = GetComponentInChildren<T>();
        if (component != null)
        {
            onSuccess?.Invoke(component);
        }
        else
        {
            Debug.LogError($"[Building_{InstanceId}] {typeof(T).Name} not found.");
        }
        return component;
    }

    void Start()
    {

    }

    protected virtual void Update()
    {
        if (!IsInitialized) return;
    }

    protected virtual void FixedUpdate()
    {
        if (!IsInitialized) return;

        StatsController.UpdateTick();
        AbilitiesController.UpdateTick();
        CombatStateMachine.UpdateTick();
    }

    public virtual void AddBuff(Buff buff)
    {
        StatsController.AddBuff(buff);
        OnBuffAdded?.Invoke(buff);
    }
    public virtual void RemoveBuff(Buff buff)
    {
        StatsController.RemoveBuff(buff);
        OnBuffRemoved?.Invoke(buff);
    }

    public void Upgrade()
    {
        BuildingConfig.Upgrade();
        StatsController.CalculateStats(BuildingConfig.Level);
        OnUpgrade?.Invoke(BuildingConfig.Level);
    }

    public void HandleStartMoving()
    {
        BuildingBoundaries.SetVisibility(true);
    }

    public void UpdatePlacePosition(Vector3Int cellPos)
    {
        PlacementCell = cellPos;
        Position = transform.position;
    }

    public void HandlePlace()
    {
        BuildingBoundaries.SetVisibility(false);
    }

    public void PlaySpawnVFX()
    {
        BuildingVFXController.PlaySpawnVFX();
    }

    public void DestroyBuilding()
    {
        Destroy(gameObject);
    }
}