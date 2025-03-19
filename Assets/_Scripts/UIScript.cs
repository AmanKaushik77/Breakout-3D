using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI current;          // ScoreText
    [SerializeField] private TextMeshProUGUI toUpdate;        // UpdatedScoreText
    [SerializeField] private Transform scoreTextContainer;    // ScoreTextBackground
    [SerializeField] private float duration = 0.4f;           // Animation duration

    private float containerInitPosition;
    private float moveAmount;

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        current.SetText("0");
        toUpdate.SetText("0");
        containerInitPosition = scoreTextContainer.localPosition.y;
        // Explicitly set moveAmount to match the distance from Y=-48 to Y=2
        moveAmount = 50f; // Distance between ScoreText (Y=2) and UpdatedScoreText (Y=-48)
        // Alternatively, test with: moveAmount = current.rectTransform.rect.height + 50f;
    }

    public void UpdateScore(int score)
    {
        toUpdate.SetText($"{score}");
        // Ensure the container moves enough to show UpdatedScoreText
        scoreTextContainer.DOLocalMoveY(containerInitPosition + moveAmount, duration)
            .SetEase(Ease.OutQuad); // Added easing for smoother animation
        StartCoroutine(ResetScoreContainer(score));
    }

    private System.Collections.IEnumerator ResetScoreContainer(int score)
    {
        yield return new WaitForSeconds(duration);
        current.SetText($"{score}");
        Vector3 localPosition = scoreTextContainer.localPosition;
        scoreTextContainer.localPosition = new Vector3(localPosition.x, containerInitPosition, localPosition.z);
    }
}