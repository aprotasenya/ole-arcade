using UnityEngine;
using System; 

public class GameStateModel : MonoBehaviour
{
    int collectiblesCount = 0;
    bool gameWon = false;

    public Action<int> OnCollectibleCountChanged;

    public void SetCollectiblesCount(int value)
    {
        collectiblesCount = value;
        OnCollectibleCountChanged?.Invoke(collectiblesCount);
    }

    public void AddCollectible()
    {
        collectiblesCount++;
        OnCollectibleCountChanged?.Invoke(collectiblesCount);
    }

    public void RemoveCollectible()
    {
        collectiblesCount--;
        OnCollectibleCountChanged?.Invoke(collectiblesCount);
    }

    public void SetGameWon(bool isWon) {
        gameWon = isWon;
    }

}
