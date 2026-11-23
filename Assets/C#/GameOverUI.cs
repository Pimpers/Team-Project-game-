using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// 게임 오버 패널을 관리합니다.
/// GameManager.GameOverRoutine() 에서 Show(score) 를 호출합니다.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [Header("패널 루트 (이 오브젝트 자체)")]
    // 이 스크립트가 붙은 GO가 패널 루트입니다.

    [Header("텍스트")]
    public TextMeshProUGUI finalScoreText;   // 최종 점수 숫자
    public TextMeshProUGUI bestScoreText;    // 베스트 스코어
    public TextMeshProUGUI newRecordText;    // 신기록 문구

    [Header("버튼")]
    public Button restartButton;
    public Button exitButton;

    [Header("등장 애니메이션")]
    public float fadeInDuration = 0.35f;

    CanvasGroup cg;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        // 비활성 초기화는 씬 YAML(m_IsActive: 0)에서 처리 —
        // 여기서 SetActive(false)를 호출하면 Show()의 SetActive(true) 직후
        // Awake가 실행되어 다시 꺼지는 버그가 발생하므로 제거.
    }

    void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);
    }

    // ── 외부 호출 ──────────────────────────────────────────────────────
    public void Show(int finalScore)
    {
        gameObject.SetActive(true);

        // 최종 점수
        if (finalScoreText != null)
            finalScoreText.text = finalScore.ToString("N0");

        // 베스트 스코어
        if (ScoreManager.Instance != null && bestScoreText != null)
        {
            int best = ScoreManager.Instance.GetTopScores()[0];
            bestScoreText.text = best.ToString("N0");
        }

        // 신기록 여부
        bool isNew = ScoreManager.Instance != null
            && finalScore > 0
            && ScoreManager.Instance.GetTopScores()[0] == finalScore;

        if (newRecordText != null)
            newRecordText.gameObject.SetActive(isNew);

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        cg.alpha = 0f;
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        cg.alpha = 1f;
    }

    // ── 버튼 ────────────────────────────────────────────────────────────
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
