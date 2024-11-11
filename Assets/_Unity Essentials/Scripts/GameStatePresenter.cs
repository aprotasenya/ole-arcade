using UnityEngine;
using Zenject;

public class GameStatePresenter : MonoBehaviour {
    [Inject] readonly GameStateModel model;
    [Inject] readonly GameStateView view;
    [SerializeField] GameObject collectiblePrefab;
    [SerializeField] ICollectible.CollectibleType itemType;

    private void Start()
    {
        itemType = collectiblePrefab.GetComponent<ICollectible>().type;
        //model.SetCollectiblesCount(itemType, 0);
        model.SetGameWon(false);
    }

    private void OnEnable()
    {
        SubscribeAll();
    }

    private void OnDisable()
    {
        UnsubscribeAll();
    }

    private void SubscribeAll()
    {
        Collectible.OnCreated += model.AddCollectibleCallback;
        Collectible.OnCollected += model.RemoveCollectibleCallback;
        model.OnCollectibleCountChanged += view.UpdateCounter;
        //model.OnCollectibleCountChanged += CheckGameWinOnCount;
    }

    private void UnsubscribeAll()
    {
        Collectible.OnCreated -= model.AddCollectibleCallback;
        Collectible.OnCollected -= model.RemoveCollectibleCallback;
        model.OnCollectibleCountChanged -= view.UpdateCounter;
        //model.OnCollectibleCountChanged -= CheckGameWinOnCount;
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
