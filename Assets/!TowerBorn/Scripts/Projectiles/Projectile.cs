using System;
using UnityEngine;

[Serializable]
public enum ProjectileType
{
    Straight, Mortar, Instant
}
public class ProjectileInfo
{
    public ProjectileStats config;
    public float damage;
    public IBuilding sender;
    public BaseUnit target;
}
public class Projectile : MonoBehaviour
{
    ProjectileInfo projectileInfo;
    IBuilding Owner => projectileInfo.sender;
    ProjectileStats config => projectileInfo.config;
    ProjectileType projectileType => config.projectileType;

    BaseUnit Target => projectileInfo.target;
    AttackData AttackData { get; set; }

    Vector3 StartPosition;
    Vector3 TargetPosition;

    float liveDuration = 0f;

    [SerializeField] ParticleSystem impactVFX;
    [SerializeField] TrailRenderer trail;

    public event Action<AttackData> OnHit;
    public event Action<AttackData> OnKill;

    bool isInitialized = false;
    public void Init(AttackData attackData, ProjectileInfo info)
    {
        projectileInfo = info;

        AttackData = attackData;

        StartPosition = transform.position;
        TargetPosition = Target.Collider.ClosestPoint(transform.position);

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        if (Target as MonoBehaviour != null && Target.isActiveAndEnabled)
        {
            TargetPosition = Target.Collider.ClosestPoint(transform.position);
        }

        switch (projectileType)
        {
            case ProjectileType.Straight:

                MoveToPosition(TargetPosition);
                break;
            case ProjectileType.Mortar:
                TargetPosition.y = 0f;
                MoveToPositionInParabola(TargetPosition);
                break;
            case ProjectileType.Instant:
                HandleDealDamage();
                break;
            default:
                break;
        }

        liveDuration += Time.deltaTime;
    }

    void MoveToPosition(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, config.moveSpeed * Time.deltaTime);
        transform.LookAt(TargetPosition);
        if (ProjectUtils.HasReachedTarget(transform.position, TargetPosition, 0.05f)) HandleDealDamage();
    }
    void MoveToPositionInParabola(Vector3 position)
    {
        float t = liveDuration * config.moveSpeed / 5f;
        if (t > 1f) t = 1f;

        transform.position = ProjectUtils.Parabola(StartPosition, TargetPosition, config.mortarHeight, t);
        //transform.LookAt(TargetPosition);
        if (t >= 1f) HandleDealDamage();
    }

    void HandleDealDamage()
    {
        if (impactVFX != null)
        {
            impactVFX.transform.parent = null;
            impactVFX.Play();
        }
        if (trail != null)
        {
            trail.transform.parent = null;
        }

        if (config.splashRadius > 0f)
        {
            DealSplashDamage();
        }
        else if (Target as MonoBehaviour != null)
        {
            DealDamage(Target);
        }

        DestroyMe();
    }

    void DealSplashDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(TargetPosition, config.splashRadius, LayerMask.GetMask("Enemy"));

        foreach (Collider collider in colliders)
        {
            BaseUnit enemy = collider.GetComponent<BaseUnit>();
            if (enemy == null) continue;

            DealDamage(enemy);
        }
    }

    void DealDamage(BaseUnit target)
    {
        bool isDeadlyDamage = target.IsDeadlyDamage(projectileInfo.damage);

        AttackData.target = target;
        HandleAbilitiesOnHit();
        target.TakeDamage(projectileInfo.damage);

        if (isDeadlyDamage) HandleAbilitiesOnKill();
    }


    void HandleAbilitiesOnHit()
    {
        OnHit?.Invoke(AttackData);
    }
    void HandleAbilitiesOnKill()
    {
        OnKill?.Invoke(AttackData);
    }

    void DestroyMe()
    {
        if (impactVFX != null)
        {
            impactVFX.transform.SetParent(null);
            Destroy(impactVFX.gameObject, 1f);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (projectileInfo == null) return;
        Gizmos.DrawWireSphere(transform.position, config.splashRadius);
    }
}