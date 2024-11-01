using UnityEngine;
using Zenject;
using TMPro;

public class GameManagerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameStateModel>().FromComponentOn(gameObject).AsSingle();
        Container.Bind<GameStateView>().FromComponentOn(gameObject).AsSingle();
        Container.Bind<CelebrationConfetti>().FromComponentInChildren().AsSingle();
        
    }
}