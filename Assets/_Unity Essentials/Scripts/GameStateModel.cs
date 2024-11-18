using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;

public class GameStateModel : MonoBehaviour
{
    bool gameWon = false;

    [SerializeField, ReorderableList]
    List<CollectibleGoal> gameCollectGoals = new List<CollectibleGoal>();
    [SerializeField] bool canAutoAddGoals = true;

    public event Action<List<CollectibleGoal>> OnGoalsUpdated;

    public void InitializeGoals()
    {
        gameWon = false;

        foreach (var goal in gameCollectGoals)
        {
            goal.Init();
        }

        OnGoalsUpdated?.Invoke(gameCollectGoals);
    }

    public void UpdateTheGoals()
    {
        if (!gameWon) OnGoalsUpdated?.Invoke(gameCollectGoals);
    }

    //public void SetCollectiblesCount(CollectibleConfig itemType, int value)
    //{
    //    var goal = gameCollectGoals.Find(g => g.collectibleConfig == itemType);
    //    goal?.SetGoalQuantity(goal.collectibleConfig, value);

    //    OnGoalsUpdated?.Invoke(gameCollectGoals);
    //}

    public void AddCollectibleCallback(CollectibleConfig itemType, int value)
    {
        var goal = gameCollectGoals.Find(g => g.collectibleConfig == itemType);

        if (goal == null)
        {
            if (!canAutoAddGoals) return;

            var newGoal = new CollectibleGoal(itemType, true);
            newGoal.Init();

            gameCollectGoals.Add(newGoal);
            goal = newGoal;
        }

        if (goal.collectAllFromScene)
        {
            goal?.AddGoalQuantity(goal.collectibleConfig, value);
        }

        OnGoalsUpdated?.Invoke(gameCollectGoals);
    }

    public void RemoveCollectibleCallback(CollectibleConfig itemType, int value)
    {
        var goal = gameCollectGoals.Find(g => g.collectibleConfig == itemType);

        if (goal == null || goal.IsComplete) return;

        goal?.RemoveGoalQuantity(goal.collectibleConfig, value);
        OnGoalsUpdated?.Invoke(gameCollectGoals);
    }

    public void SetGameWon(bool isWon)
    {
        gameWon = isWon;
    }

}
