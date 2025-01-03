﻿using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameStatePresenter : MonoBehaviour {
    [Inject] readonly GameStateModel model;
    [Inject] readonly GameStateView view;
    [SerializeField] RandomPopulator populator;

    private void Start()
    {
        model.InitializeGoals();

        if (populator != null) populator.Populate();
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
        ICollectible.OnCreated += model.AddCollectibleCallback;
        ICollectible.OnCollected += model.RemoveCollectibleCallback;
        CollectibleGoal.OnComplete += model.UpdateTheGoals;
        model.OnGoalsUpdated += view.UpdateCounter;
        model.OnGoalsUpdated += CheckAllGoalsComplete;
    }

    private void UnsubscribeAll()
    {
        ICollectible.OnCreated -= model.AddCollectibleCallback;
        ICollectible.OnCollected -= model.RemoveCollectibleCallback;
        CollectibleGoal.OnComplete -= model.UpdateTheGoals;
        model.OnGoalsUpdated -= view.UpdateCounter;
        model.OnGoalsUpdated -= CheckAllGoalsComplete;

    }

    private void CheckAllGoalsComplete(List<CollectibleGoal> goals)
    {
        var victory = (goals.Count > 0) && (goals.TrueForAll(g => g.IsComplete == true));

        if (victory)
        {
            model.SetGameWon(true);
            view.Celebrate();
        }
    }

}
