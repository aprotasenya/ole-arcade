using UnityEngine;
using Zenject;

public class CheeseGameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().To<PlayerController2D>().FromComponentInHierarchy().AsSingle();
    }
}