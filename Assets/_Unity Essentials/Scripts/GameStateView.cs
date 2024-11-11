using UnityEngine;
using TMPro;
using Zenject;
using System.Collections.Generic;
using System.Linq;

public class GameStateView : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI counterTextObject;
    [SerializeField] private string counterText = "Collectibles remaining:";

    [SerializeField] string celebrationText = "All collected! Woohoo!";
    [SerializeField] Color celebrationTextColor = Color.magenta;

    [Inject] readonly CelebrationConfetti confetti;


    public void UpdateCounter(List<CollectibleGoal> goals)
    {
        var countTotal = goals.Sum(g => g.QuantityGoal);
        counterTextObject.text = $"{counterText} {countTotal}";
    }

    public void Celebrate()
    {
        counterTextObject.text = celebrationText;
        counterTextObject.color = celebrationTextColor;

        confetti?.LaunchConfetti();

    }
}
