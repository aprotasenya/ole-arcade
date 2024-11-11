using UnityEngine;

public interface ICollectible
{
    public enum CollectibleType { Star3D, Cheese2D, RedBlock_M3, GreenBlock_M3, BlueBlock_M3 }
    public abstract CollectibleType type { get; set; }
}
