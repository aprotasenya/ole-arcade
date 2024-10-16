using UnityEngine;
using TMPro;
using System; // Required for Type handling

public class UpdateCollectibleCount : MonoBehaviour
{
    int totalCollectibles = 0;
    bool gameWon = false;

    [SerializeField] private TextMeshProUGUI counterTextObject; // Reference to the TextMeshProUGUI component
    [SerializeField] private string counterText = "Collectables:";

    [SerializeField] string celebrationText = "All collected! Woohoo!";
    [SerializeField] Color celebrationTextColor = Color.magenta;

    Transform player;
    [SerializeField] GameObject celebrationPrefab;
    [SerializeField] bool celebrateAtPlayerPosition = true;
    [SerializeField] bool useCustomRotation = true;
    [SerializeField] Vector3 customCelebrationRotation = new(-90, 0, 0);
    [SerializeField] Transform celebrationPoint;



    void Start()
    {
        if (counterTextObject == null)
        {
            Debug.LogError("UpdateCollectibleCount script requires a TextMeshProUGUI component.");
            return;
        }
        CountCollectiblesAndUpdateCounter(); // Initial update on start

    }

    void Update()
    {
        if (gameWon) { return; }

        CountCollectiblesAndUpdateCounter();

        if (totalCollectibles == 0)
        {
            Celebrate();
        }
    }

    private void CountCollectiblesAndUpdateCounter()
    {
        totalCollectibles = 0;

        CountCollectableType();
        CountCollectable2DType();

        // Update the collectible count display
        counterTextObject.text = $"{counterText} {totalCollectibles}";


    }

    private void CountCollectableType()
    {
        Type collectibleType = Type.GetType("Collectible");
        if (collectibleType != null)
        {
            totalCollectibles += UnityEngine.Object.FindObjectsOfType(collectibleType).Length;
        }
    }

    private void CountCollectable2DType()
    {
        Type collectibleType = Type.GetType("Collectible2D");
        if (collectibleType != null)
        {
            totalCollectibles += UnityEngine.Object.FindObjectsOfType(collectibleType).Length;
        }
    }


    private void Celebrate()
    {
        gameWon = true;
        counterTextObject.text = celebrationText;
        counterTextObject.color = celebrationTextColor;

        Vector3 celebratePosition;
        Quaternion celebrateRotation;

        if (celebrateAtPlayerPosition)
        {
            player = GameObject.FindObjectOfType<PlayerController>().transform;
            celebratePosition = player.position;
            celebrateRotation = player.rotation;
        } else
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
