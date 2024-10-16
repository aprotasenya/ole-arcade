using UnityEngine;
// Celebration

public class CelebrationConfetti : MonoBehaviour
{
    Transform player;
    [SerializeField] GameObject celebrationPrefab;
    [SerializeField] bool celebrateAtPlayerPosition = true;
    [SerializeField] bool useCustomRotation = true;
    [SerializeField] Vector3 customCelebrationRotation = new(-90, 0, 0);
    [SerializeField] Transform celebrationPoint;

    public void LaunchConfetti()
    {
        Vector3 celebratePosition;
        Quaternion celebrateRotation;

        if (celebrateAtPlayerPosition)
        {
            player = GameObject.FindObjectOfType<PlayerController>().transform;
            celebratePosition = player.position;
            celebrateRotation = player.rotation;
        }
        else
        {
            celebratePosition = celebrationPoint.position;
            celebrateRotation = celebrationPoint.rotation;
        }

        if (useCustomRotation)
        {
            celebrateRotation = Quaternion.Euler(customCelebrationRotation);

        }

        var celebration = Instantiate(celebrationPrefab, celebratePosition, celebrateRotation);
        celebration.GetComponent<ParticleSystem>().Play();
    }
}
