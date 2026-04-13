using UnityEngine;

public class BuildingMeshController : MonoBehaviour
{
    [SerializeField] private GameObject defaultModelGO;
    public GameObject DefaultModelGO => defaultModelGO;

    [SerializeField] private GameObject brokenModelGO;
    public GameObject BrokenModelGO => brokenModelGO;

    [SerializeField] private GameObject buildingProcessModelGO;
    public GameObject BuildingProcessModelGO => buildingProcessModelGO;

    [SerializeField] ParticleSystem spawnVFX;
    void Start()
    {
        
    }

    public void PlaySpawnVFX()
    {
        spawnVFX.Play();
    }
}
