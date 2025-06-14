using TMPro;
using UnityEngine;

public class UIcontroller : MonoBehaviour
{
    public TMP_Text scoretext; // Reference to the TextMeshProUGUI component for displaying the score
    public TMP_Text scoreMaxText; // Reference to the TextMeshProUGUI component for displaying the score
    private void Update()
    {
        // Update the score text with the current score from the ball class
        scoretext.text = "Score: " + ball.score.ToString();
        scoreMaxText.text = "Highscore: " + ball.scoreMax.ToString();
    }
}
