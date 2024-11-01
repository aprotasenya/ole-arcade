using UnityEngine;
using Zenject;

public class GameStatePresenter : MonoBehaviour {
    [Inject] readonly GameStateModel model;
    [Inject] readonly GameStateView view;

    private void Awake()
    {
        model.SetCollectiblesCount(0);
        model.SetGameWon(false);

        Collectible.OnCreated += model.AddCollectible;
        Collectible.OnCollected += model.RemoveCollectible;
        model.OnCollectibleCountChanged += view.UpdateCounter;
        model.OnCollectibleCountChanged += CheckGameWinOnCount;
    }

    private void OnDestroy()
    {
        UnsubscribeAll();
    }

    private void UnsubscribeAll()
    {
        Collectible.OnCreated -= model.AddCollectible;
        Collectible.OnCollected -= model.RemoveCollectible;
        model.OnCollectibleCountChanged -= view.UpdateCounter;
        model.OnCollectibleCountChanged -= CheckGameWinOnCount;
    }

    private void CheckGameWinOnCount(int count)
    {
        if (count <= 0)
        {
            model.SetGameWon(true);
            view.Celebrate();
        }
    }

}
