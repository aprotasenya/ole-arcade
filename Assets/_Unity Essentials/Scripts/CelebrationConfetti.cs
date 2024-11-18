using UnityEngine;
using Zenject;

public class CelebrationConfetti : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] GameObject celebrationPrefab;
    [SerializeField] bool celebrateAtPlayerPosition = true;
    [SerializeField] bool useCustomRotation = true;
    [SerializeField] Vector3 customCelebrationRotation = new(-90, 0, 0);
    [SerializeField] Transform celebrationPoint;

    public void LaunchConfetti()
    {
        Vector3 celebratePosition;
        Quaternion celebrateRotation;

        if (celebrateAtPlayerPosition && player != null)
        {
            celebratePosition = player.transform.position;
            celebrateRotation = player.transform.rotation;
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
