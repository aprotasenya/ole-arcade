using UnityEngine;

public class GameStatePresenter : MonoBehaviour {
    [SerializeField] GameStateModel model;
    [SerializeField] GameStateView view;

    private void Awake()
    {
        model.SetCollectiblesCount(0);
        model.SetGameWon(false);

        Collectible.onCreate += model.AddCollectible;
        Collectible.onPickup += model.RemoveCollectible;
        model.OnCollectibleCountChanged += view.UpdateCounter;
        model.OnCollectibleCountChanged += CheckGameWinOnCount;
    }

    private void OnDestroy()
    {
        UnsubscribeAll();
    }

    private void UnsubscribeAll()
    {
        Collectible.onCreate -= model.AddCollectible;
        Collectible.onPickup -= model.RemoveCollectible;
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
