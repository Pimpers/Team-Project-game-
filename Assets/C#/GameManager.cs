using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform launcher;
    public float spawnOffset = -1.3f;

    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public GameObject effectPrefab;
    public Transform effectGroup;

    public AudioClip[] sfxClip;
    public AudioSource bgmplayer;
    public AudioSource[] sfxPlayer;
    public enum SFX { LevelUp }
    int sfxCursor;

    public int maxLevel;
    public bool isGameOver;

    [Header("Launcher Bounds (물리 벽 내부면 위치 — 공 크기 마진은 코드에서 자동 계산)")]
    public float launcherMinX = -4.9f;
    public float launcherMaxX = 4.9f;

    [Header("레벨별 월드 반지름 (levelRadii × animFinalScale)")]
    public float[] ballWorldRadii = { 0.69f, 0.86f, 1.05f, 1.28f, 1.56f, 1.89f, 2.31f, 2.81f };

    [Header("Game Over Effect")]
    public GameOverEffect gameOverEffect;  // 씬의 GameOverEffect GO 연결
    public GameOverUI gameOverUI;          // 씬의 GameOverPanel GO 연결

    [Header("Next Preview")]
    public NextPreview nextPreviewObj;   // 씬의 NextPreview GO 연결
    int nextLevel;                       // 미리 뽑아둔 다음 레벨

    [Header("점수 기반 스폰 MAX레벨")]
    [Tooltip("게임 시작 시 스폰 가능한 최대 레벨 (0 = 레벨0만, 1 = 레벨0~1, ...)")]
    public int startSpawnLevel = 1;
    [Tooltip("이 점수를 넘으면 MAX레벨이 1씩 증가. 예) 100점→lv2, 300점→lv3 ...")]
    public int[] levelUpThresholds = { 100, 300, 600, 1000 };
    [Tooltip("스폰 최대 레벨 상한 (이 값을 넘지 않음)")]
    public int maxSpawnLevel = 5;
    int spawnLevelCap;                   // 런타임 스폰 최대 레벨 (Read-only 참고용)

    // 오브젝트 풀
    [Header("Pool Settings")]
    public int poolSize = 20;
    Queue<Dongle> donglePool = new Queue<Dongle>();
    Queue<ParticleSystem> effectPool = new Queue<ParticleSystem>();

    // 활성 Dongle 목록 (FindObjectsOfType 대체)
    List<Dongle> activeDongles = new List<Dongle>();

    // 캐시
    Camera mainCamera;
    WaitForSeconds wait01s  = new WaitForSeconds(0.1f);
    WaitForSeconds wait020s = new WaitForSeconds(0.20f);

    static readonly Vector3 gameOverHideTarget = Vector3.up * 100;

    void Awake()
    {
        Application.targetFrameRate = 60;
        mainCamera = Camera.main;
        InitPool();
    }

    void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            donglePool.Enqueue(CreateDongle());
            effectPool.Enqueue(CreateEffect());
        }
    }

    Dongle CreateDongle()
    {
        GameObject obj = Instantiate(donglePrefab, dongleGroup);
        Dongle d = obj.GetComponent<Dongle>();
        obj.SetActive(false);
        return d;
    }

    ParticleSystem CreateEffect()
    {
        GameObject obj = Instantiate(effectPrefab, effectGroup);
        ParticleSystem ps = obj.GetComponent<ParticleSystem>();
        obj.SetActive(false);
        return ps;
    }

    Dongle GetDongle()
    {
        Dongle d = donglePool.Count > 0 ? donglePool.Dequeue() : CreateDongle();
        ParticleSystem effect = effectPool.Count > 0 ? effectPool.Dequeue() : CreateEffect();

        effect.gameObject.SetActive(true);
        d.effect = effect;
        d.manager = this;

        activeDongles.Add(d);
        return d;
    }

    public void ReturnDongle(Dongle d)
    {
        activeDongles.Remove(d);
        if (d.effect != null)
        {
            d.effect.gameObject.SetActive(false);
            effectPool.Enqueue(d.effect);
            d.effect = null;
        }
        donglePool.Enqueue(d);
    }

    void Start()
    {
        bgmplayer.Play();
        if (maxLevel < 1) maxLevel = 1;
        spawnLevelCap = Mathf.Max(1, startSpawnLevel);
        if (launcher != null) launcher.gameObject.SetActive(true);
        // 첫 번째 다음 레벨 미리 결정
        nextLevel = Random.Range(0, spawnLevelCap);
        if (nextPreviewObj != null) nextPreviewObj.SetLevel(nextLevel);
        nextDongle();
    }

    void Update()
    {
        if (isGameOver) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 현재 공 레벨에 따라 런처 범위 동적 계산 (공 가장자리가 벽에 닿지 않게)
        const float wallMargin = 0.18f;   // 벽 겹침 방지 마진 (너무 작으면 Drop 순간 튕김 발생)
        float worldRadius = 0f;
        if (lastDongle != null && !lastDongle.isDropped
            && ballWorldRadii != null && lastDongle.level < ballWorldRadii.Length)
        {
            worldRadius = ballWorldRadii[lastDongle.level];
        }
        float dynMin = launcherMinX + worldRadius + wallMargin;
        float dynMax = launcherMaxX - worldRadius - wallMargin;
        float xPos = Mathf.Clamp(mousePos.x, dynMin, dynMax);

        if (launcher != null)
        {
            launcher.position = new Vector3(xPos, launcher.position.y, 0);

            // 클릭 전후 모두 lastDongle을 런처 아래에 고정
            if (lastDongle != null && !lastDongle.isDropped)
            {
                Vector3 donglePos = launcher.position;
                donglePos.y += spawnOffset;
                lastDongle.transform.position = donglePos;
            }
        }

        if (Input.GetMouseButtonDown(0)) TouchDown();
        if (Input.GetMouseButtonUp(0)) TouchUp();

        UpdateSpawnLevelCap();
    }

    void UpdateSpawnLevelCap()
    {
        if (Score.Instance == null || levelUpThresholds == null) return;
        int score = Score.Instance.GetScore();
        int newCap = Mathf.Max(1, startSpawnLevel);
        for (int i = 0; i < levelUpThresholds.Length; i++)
        {
            if (score >= levelUpThresholds[i]) newCap = startSpawnLevel + i + 1;
            else break;
        }
        spawnLevelCap = Mathf.Min(newCap, maxSpawnLevel);
    }

    public void PlaySFX(SFX sfx)
    {
        if (sfxClip.Length == 0 || sfxPlayer.Length == 0) return;
        sfxPlayer[sfxCursor].clip = sfxClip[(int)sfx];
        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }

    void nextDongle()
    {
        if (isGameOver) return;
        lastDongle = GetDongle();
        lastDongle.level = nextLevel;   // 미리 결정된 레벨 사용

        // 다음 레벨 미리 결정 & 미리보기 갱신
        nextLevel = Random.Range(0, spawnLevelCap);
        if (nextPreviewObj != null) nextPreviewObj.SetLevel(nextLevel);

        Vector3 spawnPos = launcher.position;
        spawnPos.y += spawnOffset;
        lastDongle.transform.position = spawnPos;
        lastDongle.gameObject.SetActive(true);
    }

    IEnumerator WaitNext(Dongle dropped)
    {
        // 최소 대기 — Drop() 직후 벽 튕김에 의한 즉시 hasLanded 오판정 방지
        yield return wait020s;

        // 공이 뭔가에 닿을 때까지 대기 (최대 2초 타임아웃)
        float elapsed = 0f;
        while (elapsed < 2f)
        {
            if (dropped == null || !dropped.gameObject.activeSelf || dropped.hasLanded)
                break;
            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return wait01s;   // 아주 짧은 여유 (0.1초)
        nextDongle();
    }

    public void TouchDown() { /* 클릭 피드백 필요 시 여기에 추가 */ }
    public void TouchUp()
    {
        if (lastDongle != null)
        {
            Dongle dropped = lastDongle;
            lastDongle.Drop();
            lastDongle = null;
            StartCoroutine(WaitNext(dropped));
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        // ① 카메라 흔들기 + 화면 플래시 즉시 실행
        if (gameOverEffect != null) gameOverEffect.Play();

        // ② 공들 물리 정지
        Dongle[] snapshot = activeDongles.ToArray();
        foreach (var d in snapshot) d.rigid.simulated = false;

        // ③ 하나씩 이펙트 터뜨린 뒤 사라짐
        //    ※ Hide() 내부의 EffectPlay()는 ReturnDongle()이 즉시 effect를 끄므로
        //      SpawnBurstEffect로 독립적인 이펙트를 먼저 소환한다.
        foreach (var d in snapshot)
        {
            SpawnBurstEffect(d.transform.position);  // 공 위치에서 이펙트 폭발
            d.Hide(gameOverHideTarget);              // 공 숨기기
            yield return wait01s;
        }

        // ④ 최종 점수 순위 등록
        int finalScore = (Score.Instance != null) ? Score.Instance.GetScore() : 0;
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SubmitScore(finalScore);
            if (RankingBoard.Instance != null)
                RankingBoard.Instance.RefreshUI(finalScore);
        }

        // ⑤ 게임 오버 UI 표시
        if (gameOverUI != null)
            gameOverUI.Show(finalScore);
    }

    // 이펙트 풀에서 하나 꺼내 지정 위치에서 재생
    void SpawnBurstEffect(Vector3 position)
    {
        ParticleSystem ps = effectPool.Count > 0 ? effectPool.Dequeue() : CreateEffect();
        ps.gameObject.SetActive(true);
        ps.transform.position = position;
        ps.Play();
        StartCoroutine(ReturnEffectDelayed(ps));
    }

    IEnumerator ReturnEffectDelayed(ParticleSystem ps)
    {
        yield return new WaitForSeconds(2.5f);
        if (ps != null && ps.gameObject.activeSelf)
        {
            ps.Stop();
            ps.gameObject.SetActive(false);
            effectPool.Enqueue(ps);
        }
    }
}
