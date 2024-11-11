using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameStatePresenter : MonoBehaviour {
    [Inject] readonly GameStateModel model;
    [Inject] readonly GameStateView view;

    private void Start()
    {
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
        CollectibleGoal.OnComplete += model.UpdateTheGoals;
        model.OnGoalsUpdated += view.UpdateCounter;
        model.OnGoalsUpdated += CheckGameWinOnGoals;
    }

    private void UnsubscribeAll()
    {
        Collectible.OnCreated -= model.AddCollectibleCallback;
        Collectible.OnCollected -= model.RemoveCollectibleCallback;
        CollectibleGoal.OnComplete -= model.UpdateTheGoals;
        model.OnGoalsUpdated -= view.UpdateCounter;
        model.OnGoalsUpdated -= CheckGameWinOnGoals;

    }

    private void CheckGameWinOnGoals(List<CollectibleGoal> goals)
    {
        var victory = goals.TrueForAll(g => g.GoalComplete == true);

        if (victory)
        {
            model.SetGameWon(true);
            view.Celebrate();
        }
    }

}
