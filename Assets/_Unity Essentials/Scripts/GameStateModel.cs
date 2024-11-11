using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;

public class GameStateModel : MonoBehaviour
{
    int collectiblesCount = 0;
    bool gameWon = false;

    [SerializeField, ReorderableList]
    List<CollectibleGoal> collectibleGoals = new List<CollectibleGoal>();

    Dictionary<ICollectible, int> _collectibleGoals = new Dictionary<ICollectible, int>();

    private void Awake()
    {
        foreach (var goal in collectibleGoals)
        {
            goal.Init();
        }
    }

    public Action<int> OnCollectibleCountChanged;

    public void SetCollectiblesCount(ICollectible.CollectibleType itemType, int value)
    {
        var goal = collectibleGoals.Find(g => g.collectible.type == itemType);
        goal?.SetGoalQuantity(goal.collectibleType, value);

        collectiblesCount = value;

        OnCollectibleCountChanged?.Invoke(goal.QuantityGoal);
    }

    public void AddCollectibleCallback(ICollectible.CollectibleType itemType, int value)
    {
        var goal = collectibleGoals.Find(g => g.collectible.type == itemType);
        if (goal.collectAllFromScene)
        {
            goal?.AddGoalQuantity(goal.collectibleType, value);
        }
        collectiblesCount += value;
        OnCollectibleCountChanged?.Invoke(goal.QuantityGoal);
    }

    public void RemoveCollectibleCallback(ICollectible.CollectibleType itemType, int value)
    {
        var goal = collectibleGoals.Find(g => g.collectible.type == itemType);
        goal?.RemoveGoalQuantity(goal.collectibleType, value);

        collectiblesCount -= value;
        OnCollectibleCountChanged?.Invoke(goal.QuantityGoal);
    }

    public void SetGameWon(bool isWon) {
        gameWon = isWon;
    }

}
