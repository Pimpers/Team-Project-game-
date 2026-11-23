using UnityEngine;
using TMPro;

/// <summary>
/// 화면에 Top-5 순위표를 표시합니다.
/// Canvas 하위 RankingPanel에 붙여서 사용하세요.
/// </summary>
public class RankingBoard : MonoBehaviour
{
    public static RankingBoard Instance { get; private set; }

    [Header("순위 텍스트 (TMP)")]
    public TextMeshProUGUI rank1Text;
    public TextMeshProUGUI rank2Text;
    public TextMeshProUGUI rank3Text;
    public TextMeshProUGUI rank4Text;
    public TextMeshProUGUI rank5Text;

    [Header("신기록 강조 색상")]
    public Color newRecordColor = new Color(1f, 0.84f, 0f);   // 금색
    public Color normalColor    = Color.white;
    public Color dimColor       = new Color(0.75f, 0.75f, 0.75f, 1f); // 4~5위 흐릿하게

    // 방금 등록된 점수 (강조 표시용)
    int lastSubmitted = -1;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => RefreshUI();

    // ───────────── 갱신 ─────────────
    /// <summary>ScoreManager 데이터를 읽어 UI를 갱신합니다.</summary>
    public void RefreshUI(int highlightScore = -1)
    {
        lastSubmitted = highlightScore;

        if (ScoreManager.Instance == null) return;
        int[] scores = ScoreManager.Instance.GetTopScores();

        TextMeshProUGUI[] texts = { rank1Text, rank2Text, rank3Text, rank4Text, rank5Text };
        for (int i = 0; i < texts.Length && i < scores.Length; i++)
            SetRankText(texts[i], i + 1, scores[i]);
    }

    // 순위별 번호 색상 (rich text — 이모지 없이 시각적 구분)
    static readonly string[] rankNumColors =
        { "#FFD700", "#D8D8D8", "#CD9F60", "#999999", "#777777" };
    //   금색       은색       동색       회색       짙은회색

    void SetRankText(TextMeshProUGUI tmp, int rank, int score)
    {
        if (tmp == null) return;

        string scoreStr = score > 0 ? score.ToString("N0") : "- - -";

        // 신기록이면 전체 금색, 아니면 순위 번호만 컬러 태그
        if (score > 0 && score == lastSubmitted)
        {
            tmp.text  = $"<b>{rank}.</b>   {scoreStr}";
            tmp.color = newRecordColor;
        }
        else
        {
            string nc = rank <= rankNumColors.Length
                ? rankNumColors[rank - 1] : "#777777";
            tmp.text  = $"<color={nc}><b>{rank}.</b></color>   {scoreStr}";
            tmp.color = rank <= 3 ? normalColor : dimColor;
        }
    }
}
