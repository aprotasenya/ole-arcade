using UnityEngine;
using TMPro;
using Zenject;

public class GameStateView : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI counterTextObject;
    [SerializeField] private string counterText = "Collectibles remaining:";

    [SerializeField] string celebrationText = "All collected! Woohoo!";
    [SerializeField] Color celebrationTextColor = Color.magenta;

    [Inject] readonly CelebrationConfetti confetti;

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
