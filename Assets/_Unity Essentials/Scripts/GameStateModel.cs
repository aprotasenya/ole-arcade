using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;

public class GameStateModel : MonoBehaviour
{
    bool gameWon = false;

    [SerializeField, ReorderableList]
    List<CollectibleGoal> gameCollectGoals = new List<CollectibleGoal>();

    public event Action<List<CollectibleGoal>> OnGoalsUpdated;

    private void Awake()
    {
        foreach (var goal in gameCollectGoals)
        {
            goal.Init();
        }
    }

    public void UpdateTheGoals()
    {
        if (!gameWon) OnGoalsUpdated?.Invoke(gameCollectGoals);
    }

    public void SetCollectiblesCount(CollectibleType itemType, int value)
    {
        var goal = gameCollectGoals.Find(g => g.collectibleType == itemType);
        goal?.SetGoalQuantity(goal.collectibleType, value);

        OnGoalsUpdated?.Invoke(gameCollectGoals);
    }

    public void AddCollectibleCallback(CollectibleType itemType, int value)
    {
        var goal = gameCollectGoals.Find(g => g.collectibleType == itemType);
        if (goal.collectAllFromScene)
        {
            goal?.AddGoalQuantity(goal.collectibleType, value);
        }

        OnGoalsUpdated?.Invoke(gameCollectGoals);
    }

    public void RemoveCollectibleCallback(CollectibleType itemType, int value)
    {
        var goal = gameCollectGoals.Find(g => g.collectibleType == itemType);
        goal?.RemoveGoalQuantity(goal.collectibleType, value);

        OnGoalsUpdated?.Invoke(gameCollectGoals);
    }

    public void SetGameWon(bool isWon) {
        gameWon = isWon;
    }

}
