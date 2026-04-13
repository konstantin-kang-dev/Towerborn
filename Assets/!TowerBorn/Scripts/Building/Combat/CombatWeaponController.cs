using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;
using static UnityEngine.GraphicsBuffer;

public class CombatWeaponController : MonoBehaviour
{
    [Inject] public EnemyDetector EnemyDetector;
    IBuilding _ownerBuilding;
    [field: SerializeField] public Transform GunPlaceHolder { get; private set; }
    [field: SerializeField] public List<CombatSpawnPoint> ProjectilesSpawnPoints { get; private set; } = new List<CombatSpawnPoint>();
    public int CurrentSpawnPointKey { get; private set; } = 0;
    public CombatSpawnPoint CurrentSpawnPoint => ProjectilesSpawnPoints[CurrentSpawnPointKey];
    public Transform CurrentSpawnPointTransform => ProjectilesSpawnPoints[CurrentSpawnPointKey].SpawnPoint;

    public BaseUnit Target {  get; private set; }

    public event Action<AttackData> OnAttack;
    public float AtkTimer { get; private set; } = 0f;

    public bool IsInitialized { get; private set; } = false;
    public void Init(IBuilding building)
    {
        _ownerBuilding = building;

        EnemyDetector.Init(building);
        EnemyDetector.StartDetection();

        IsInitialized = true;
    }

    private void FixedUpdate()
    {
        if (!IsInitialized) return;

        EnemyDetector.UpdateTick();
    }

    public void RotateTowardsTarget()
    {
        if (Target == null) return;

        Vector3 direction = Target.Position - GunPlaceHolder.transform.position;

        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            if (GunPlaceHolder.rotation != lookRotation)
            {
                GunPlaceHolder.rotation = Quaternion.RotateTowards(GunPlaceHolder.rotation, lookRotation, 8f);
            }
        }
    }

    public bool IsTargetInRange()
    {
        if (Target == null) return false;

        return Vector3.Distance(transform.position, Target.Position) < _ownerBuilding.StatsController.TotalAtkRange;
    }

    public void UpdateTarget(BaseUnit target)
    {
        Target = target;
        EnemyDetector.DetectEnemiesNow();
    }

    public virtual void HandleAttack()
    {
        AtkTimer += Time.deltaTime;

        if (AtkTimer >= _ownerBuilding.StatsController.TotalAtkInterval)
        {
            AtkTimer = 0;
            Attack();
        }
    }
    public virtual void Attack()
    {
        if (Target == null) return;
        //Debug.Log($"{gameObject.name} Attack Target: {Target} {Target == null}");
        AttackData attackData = new AttackData()
        {
            sender = _ownerBuilding,
            target = Target,
            damage = _ownerBuilding.StatsController.TotalDamage,
        };

        _ownerBuilding.BuildingAnimator.PlayAttack();
        _ownerBuilding.BuildingAudioController.PlayAttackSFX();

        TriggerAttack(attackData);

        OnAttack?.Invoke(attackData);
    }

    public void TriggerAttack(AttackData attackData)
    {
        ProjectileInfo projectileInfo = new ProjectileInfo()
        {
            config = _ownerBuilding.StatsController.CalculatedCombatStats.projectileStats,
            damage = _ownerBuilding.StatsController.TotalDamage,
            sender = _ownerBuilding,
            target = Target,
        };

        if(CurrentSpawnPoint.Vfx != null)
        {
            CurrentSpawnPoint.Vfx.Play();
        }

        GameObject projectileGO = GameObject.Instantiate(projectileInfo.config.prefab, CurrentSpawnPoint.SpawnPoint.position, Quaternion.identity);
        Projectile projectileComp = projectileGO.GetComponent<Projectile>();

        projectileComp.OnHit += _ownerBuilding.AbilitiesController.HandlePassiveAbilitiesOnHit;
        projectileComp.OnKill += _ownerBuilding.AbilitiesController.HandlePassiveAbilitiesOnKill;

        projectileComp.Init(attackData, projectileInfo);

        UpdateSpawnPointKey();
    }

    public void UpdateSpawnPointKey()
    {
        CurrentSpawnPointKey += 1;
        if (CurrentSpawnPointKey > ProjectilesSpawnPoints.Count - 1) CurrentSpawnPointKey = 0;
    }

}
