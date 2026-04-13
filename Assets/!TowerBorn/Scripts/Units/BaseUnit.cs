using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class BaseUnit : MonoBehaviour
{
    [SerializeField] ParticleSystem deathVFX;
    public UnitConfig Config {  get; private set; }

    public CombatStats CalculatedStats;

    public DebuffsManager DebuffsManager;
    public UnitAnimator UnitAnimator { get; private set; }

    public DamageableState DamageableState { get; private set; }
    public Vector3 Position { get; private set; }
    public Collider Collider { get; private set; }
    public UnitMovementType MovementType { get; private set; }
    public EntityUI EntityUI { get; private set; }

    public event Action<BaseUnit> OnDestroyed;
    public event Action<float> OnHpChanged;
    public event Action<float> OnDamageTaken;

    public float BaseMoveSpeed { get; private set; }
    public float TotalMoveSpeed => BaseMoveSpeed - (BaseMoveSpeed * DebuffsManager.CalculateDebuffValue(DebuffType.Slowing) / 100f);

    int _currentDestinationIndex = 0;
    Vector3 _currentDestination;

    public float TotalHp {  get; private set; }
    float currentHp = 0;
    public float CurrentHp {
        get { return currentHp; }
        set 
        {
            if(currentHp != value)
            {
                currentHp = value;
                OnHpChanged?.Invoke(value);
            }
        } 
    }
    public bool IsDead { get; private set; }

    float debuffsTimer = 0f;
    float debuffsCheckInterval = 0.2f;

    float pathCheckTimer = 0f;
    float pathCheckInterval = 0.5f;

    bool isInitialized = false;

    private void Awake()
    {
        Collider = GetComponent<Collider>();

        UnitAnimator = GetComponentInChildren<UnitAnimator>();
        EntityUI = GetComponentInChildren<EntityUI>();

        DebuffsManager = new DebuffsManager();
        DebuffsManager.Init(this);

        
    }

    public void ResetUnit()
    {
        OnDamageTaken -= PlayerStats.Instance.AddDamageDealed;
        OnHpChanged -= HandleHpChange;
        isInitialized = false;
    }

    public virtual void Init(UnitConfig config)
    {
        Config = config;
        CalculateStats();

        DamageableState = DamageableState.Vulnerable;

        OnDamageTaken += PlayerStats.Instance.AddDamageDealed;
        OnHpChanged += HandleHpChange;

        MovementType = CalculatedStats.movementType;
        IsDead = false;

        TotalHp = CalculatedStats.GetStatValue( CombatStatType.Hp);
        BaseMoveSpeed = CalculatedStats.GetStatValue(CombatStatType.MoveSpeed);
        CurrentHp = TotalHp;

        EntityUI.SetLevelValue(Config.level);
        UnitAnimator.ChangeAnimation(UnitAnimationState.Run);


        isInitialized = true;
    }

    void CalculateStats()
    {
        CalculatedStats = Config.combatStats.Clone();
        CalculatedStats.CacheStats();
        CalculatedStats.CalculateHpWithMultiplier(Config.level, ProjectConstants.BASE_ENEMY_HP_MULTIPLIER_PER_LEVEL);
    }

    void Start()
    {

    }

    void Update()
    {
        if (!isInitialized) return;

        Move();
    }

    protected virtual void FixedUpdate()
    {
        if (!isInitialized) return;
        Position = transform.position;
        UpdateDebuffsDuration();

    }

    void Move()
    {
        bool isFinishedPath = IsFinishedPath();
        if (isFinishedPath)
        {
            UpdateDestination();
        }
        else
        {
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, _currentDestination, TotalMoveSpeed * Time.deltaTime);

            Vector3 direction = nextPosition - transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);
            }

            transform.position = nextPosition;
        }

        Debug.DrawLine(transform.position, _currentDestination, Color.blue);
    }

    bool IsFinishedPath()
    {
        float distance = Vector3.Distance(Position, _currentDestination);

        return distance <= 0.1f;
    }
    
    void UpdateDestination()
    {
        _currentDestinationIndex += 1;
        Transform nextPoint = EnemyManager.Instance.GetDestination(_currentDestinationIndex);

        if(nextPoint == null)
        {
            FinishPath();
            return;
        }
        else
        {
            SetDestination(_currentDestinationIndex, nextPoint.position);
        }
    }

    public void SetDestination(int index, Vector3 destination)
    {
        _currentDestinationIndex = index;
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle / 2.5f;
        _currentDestination = destination + new Vector3(randomOffset.x, 0, randomOffset.y);
    }


    public void TakeDamage(float damage)
    {
        damage *= UnityEngine.Random.Range(0.95f, 1.05f);

        CurrentHp -= damage;
        AudioManager.Instance.PlaySFX(SfxType.EnemyHit);
        DamageTextPool.Instance.ShowDamageText(EntityUI.transform.position, damage, Color.white);

        OnDamageTaken?.Invoke(damage);

        if (CurrentHp <= 0) Die();
    }

    public void AddDebuff(Debuff debuff)
    {
        Debuff copiedDebuff = debuff.Clone();

        switch (copiedDebuff.type)
        {
            case DebuffType.Slowing:
                break;
            case DebuffType.PeriodicDamage:
                copiedDebuff.OnSecondsUpdate += () =>
                {
                    TakeDamage(copiedDebuff.value);
                };
                break;
            default:
                break;
        }
        DebuffsManager.AddDebuff(copiedDebuff);
    }

    public void HandleHpChange(float hp)
    {
        EntityUI.SetHpProgress(hp / TotalHp);
    }

    public bool IsDeadlyDamage(float damage)
    {
        return damage >= CurrentHp;
    }
    public bool IsFullHp()
    {
        return CurrentHp == TotalHp;
    }
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        OnDestroyed?.Invoke(this);
        OnDestroyed = null;

        AudioManager.Instance.PlaySFX(SfxType.EnemyDeath);
        if(deathVFX != null)
        {
            deathVFX.transform.parent = null;
            deathVFX.Play();
        }
        PlayerStats.Instance.AddKill();

        DebuffsManager.Clear();

        EnemyManager.Instance.ReturnToPool(this, Config);
    }

    public void UpdateDebuffsDuration()
    {
        debuffsTimer += Time.fixedDeltaTime;

        if (debuffsTimer < debuffsCheckInterval) return;

        DebuffsManager.UpdateTimer(debuffsTimer);

        UnitAnimator.SetAnimatorSpeed(TotalMoveSpeed / BaseMoveSpeed);

        debuffsTimer -= debuffsCheckInterval;
    }

    public void FinishPath()
    {
        Die();
        LivesManager.Instance.SpendLive();
    }

    /*
    void OnDrawGizmos()
    {
        if(!isInitialized) return;

        if (DebuffsManager.IsDebuffedBy(DebuffType.PeriodicDamage))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
    */
}