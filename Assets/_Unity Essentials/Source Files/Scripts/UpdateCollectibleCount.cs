using UnityEngine;
using TMPro;
using System; // Required for Type handling

public class UpdateCollectibleCount : MonoBehaviour
{
    [SerializeField] private string counterText = "Collectables:";
    [SerializeField] private TextMeshProUGUI collectibleText; // Reference to the TextMeshProUGUI component
    int totalCollectibles = 0;
    [SerializeField] GameObject celebrationPrefab;
    Transform player;
    bool gameWon = false;


    void Start()
    {
        if (collectibleText == null)
        {
            Debug.LogError("UpdateCollectibleCount script requires a TextMeshProUGUI component.");
            return;
        }
        UpdateCollectibleDisplay(); // Initial update on start

    }

    void Update()
    {
        if (gameWon) { return; }

        UpdateCollectibleDisplay();

        if (totalCollectibles == 0)
        {
            Celebrate();
        }
    }

    private void UpdateCollectibleDisplay()
    {
        
        CountCollectableType();

        // Update the collectible count display
        collectibleText.text = $"{counterText} {totalCollectibles}";


    }

    private void CountCollectableType()
    {
        // Check and count objects of type Collectible
        Type collectibleType = Type.GetType("Collectible");
        if (collectibleType != null)
        {
            totalCollectibles = UnityEngine.Object.FindObjectsOfType(collectibleType).Length;
        }

    }


    private void Celebrate()
    {
        gameWon = true;

        player = GameObject.FindObjectOfType<PlayerController>().transform;
        var celebratePosition = player.position;
        var celebrateRotation = Quaternion.Euler(new Vector3(-90, 0, 0));

        collectibleText.text = "All collected! Woohoo!";
        var celebration = Instantiate(celebrationPrefab, celebratePosition, celebrateRotation);
        celebration.GetComponent<ParticleSystem>().Play();
        
        
    }

}
