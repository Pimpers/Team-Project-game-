using UnityEngine;

/// <summary>
/// 베스트 스코어 Top 3를 PlayerPrefs에 저장·관리합니다.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    const int TOP_COUNT = 5;
    const string KEY_PREFIX = "BestScore_";   // BestScore_0 / _1 / _2

    int[] topScores = new int[TOP_COUNT];

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        LoadScores();
    }

    // ───────────── 점수 등록 ─────────────
    /// <summary>게임 종료 시 현재 점수를 제출합니다. Top 3에 들면 자동 저장됩니다.</summary>
    public bool SubmitScore(int score)
    {
        // 이미 Top 3 안에 있으면 대체 여부 확인
        for (int i = 0; i < TOP_COUNT; i++)
        {
            if (score > topScores[i])
            {
                // i 위치에 삽입, 아래로 밀기
                for (int j = TOP_COUNT - 1; j > i; j--)
                    topScores[j] = topScores[j - 1];
                topScores[i] = score;
                SaveScores();
                return true;   // 순위 등록됨
            }
        }
        return false;  // 순위권 밖
    }

    // ───────────── 조회 ─────────────
    /// <summary>Top 3 점수 배열(0=1등) 반환. 기록이 없으면 0.</summary>
    public int[] GetTopScores() => (int[])topScores.Clone();

    /// <summary>현재 제출한 점수가 몇 등인지 반환 (1~3). 순위권 밖이면 -1.</summary>
    public int GetRank(int score)
    {
        for (int i = 0; i < TOP_COUNT; i++)
            if (topScores[i] == score && score > 0) return i + 1;
        return -1;
    }

    // ───────────── 저장 / 로드 ─────────────
    void SaveScores()
    {
        for (int i = 0; i < TOP_COUNT; i++)
            PlayerPrefs.SetInt(KEY_PREFIX + i, topScores[i]);
        PlayerPrefs.Save();
    }

    void LoadScores()
    {
        for (int i = 0; i < TOP_COUNT; i++)
            topScores[i] = PlayerPrefs.GetInt(KEY_PREFIX + i, 0);
    }

    /// <summary>디버그용 — 전체 기록 초기화</summary>
    public void ResetScores()
    {
        for (int i = 0; i < TOP_COUNT; i++)
        {
            topScores[i] = 0;
            PlayerPrefs.DeleteKey(KEY_PREFIX + i);
        }
        PlayerPrefs.Save();
    }
}
