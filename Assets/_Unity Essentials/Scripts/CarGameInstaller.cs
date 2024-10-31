using UnityEngine;
using Zenject;

public class CarGameInstaller : MonoInstaller
{
    [SerializeField] private GameObject PlayerCarPrefab;

    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
        //Container.BindFactory<PlayerController, PlayerController.Factory>().FromComponentInNewPrefab(PlayerCarPrefab).AsSingle();

    }
}