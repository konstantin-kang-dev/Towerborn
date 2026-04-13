using UnityEditor;
using UnityEngine;

public class BuildingVFXController: MonoBehaviour
{
    IBuilding owner;
    [SerializeField] ParticleSystem spawnVFX;
    [SerializeField] ParticleSystem upgradeVFX;
    [SerializeField] ParticleSystem mergeAvailableVFX;

    private void Start()
    {
        
    }

    public void Init(IBuilding owner)
    {
        this.owner = owner;
    }
    public void PlaySpawnVFX()
    {
        spawnVFX.Play();
    }
    public void PlayUpgradeVFX()
    {
        upgradeVFX.Play();
    }
    public void PlayMergeAvailableVFX()
    {
        mergeAvailableVFX.gameObject.SetActive(true);
        mergeAvailableVFX.Play();
    }
    public void StopMergeAvailableVFX()
    {
        mergeAvailableVFX.Stop();
    }
}