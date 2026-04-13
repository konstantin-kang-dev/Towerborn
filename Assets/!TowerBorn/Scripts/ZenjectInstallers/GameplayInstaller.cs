using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private BuildingsManager _buildingsManager;

    public override void InstallBindings()
    {
        Container.Bind<BuildingsManager>().FromInstance(_buildingsManager).AsSingle().NonLazy();

        Container.Bind<BuildingStatsController>().AsTransient(); 
        Container.Bind<BuffsController>().AsTransient();
        Container.Bind<EnemyDetector>().AsTransient();
        Container.Bind<AbilitiesController>().AsTransient();
    }
}
