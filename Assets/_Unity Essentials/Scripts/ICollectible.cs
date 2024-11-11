using UnityEngine;

public enum CollectibleType { Star3D, Cheese2D, RedBlock_M3, GreenBlock_M3, BlueBlock_M3 }

public interface ICollectible
{
    public CollectibleType Type { get; set; }
}
