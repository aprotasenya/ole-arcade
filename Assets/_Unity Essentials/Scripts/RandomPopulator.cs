using UnityEngine;

public class RandomPopulator : MonoBehaviour
{
    [SerializeField] Transform[] sceneSpots;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] bool destroySpotObjects = true;

    public void Populate()
    {
        foreach (var spot in sceneSpots)
        {
            var position = spot.position;
            var rotation = spot.rotation;
            var parent = spot.parent;

            Instantiate(prefabs[Random.Range(0, prefabs.Length)], position, rotation, parent);
        }

        if (destroySpotObjects)
        {
            foreach (var spot in sceneSpots)
            {
                Destroy(spot.gameObject);
            }
        }
    }
}
