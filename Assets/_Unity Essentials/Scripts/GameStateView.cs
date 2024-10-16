using UnityEngine;
using TMPro;

public class GameStateView : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI counterTextObject; // Reference to the TextMeshProUGUI component
    [SerializeField] private string counterText = "Collectibles remaining:";

    [SerializeField] string celebrationText = "All collected! Woohoo!";
    [SerializeField] Color celebrationTextColor = Color.magenta;

    [SerializeField] CelebrationConfetti confetti;

    public void UpdateCounter(int count)
    {
        counterTextObject.text = $"{counterText} {count}";
    }

    public void Celebrate()
    {
        counterTextObject.text = celebrationText;
        counterTextObject.color = celebrationTextColor;

        confetti?.LaunchConfetti();

    }
}
