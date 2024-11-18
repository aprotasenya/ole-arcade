using System;

public interface ICollectible
{
    CollectibleConfig Type { get; set; }
    int collectibleValue { get; set; }

    static event Action<CollectibleConfig, int> OnCreated;
    static event Action<CollectibleConfig, int> OnCollected;

    static void RaiseOnCreated(CollectibleConfig type, int value) => OnCreated?.Invoke(type, value);
    static void RaiseOnCollected(CollectibleConfig type, int value) => OnCollected?.Invoke(type, value);
}
