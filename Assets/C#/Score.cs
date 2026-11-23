using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public static Score Instance { get; private set; }
    public TextMeshProUGUI scoreText;   // Canvas UI TMP (ScoreUI 오브젝트)
    private int currentScore;

    void Awake()
    {
        // 씬 재로드 시 중복 인스턴스 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    public int GetScore() => currentScore;

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = currentScore.ToString("0000");
    }
}
