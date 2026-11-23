using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 오버 시 카메라 흔들기 + 붉은 화면 플래시를 재생합니다.
/// GameManager가 소환해 Play()를 호출합니다.
/// </summary>
public class GameOverEffect : MonoBehaviour
{
    public static GameOverEffect Instance { get; private set; }

    [Header("카메라 흔들기")]
    public float shakeDuration  = 0.55f;
    public float shakeMagnitude = 0.28f;

    [Header("화면 플래시")]
    public Color  flashColor    = new Color(1f, 0.05f, 0.05f, 0.65f);
    public float  flashFadeTime = 1.0f;

    Camera mainCam;
    Image  flashImage;
    bool   playing;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        mainCam  = Camera.main;
        BuildFlashOverlay();
    }

    // ── 런타임으로 풀스크린 오버레이 생성 ──────────────────────────────
    void BuildFlashOverlay()
    {
        // 별도 Canvas (sortingOrder 높게 → 모든 UI 위)
        GameObject canvasGO = new GameObject("GameOverFlashCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 99;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>().blockingObjects =
            GraphicRaycaster.BlockingObjects.None;

        // 이미지 오브젝트
        GameObject imgGO = new GameObject("FlashOverlay");
        imgGO.transform.SetParent(canvasGO.transform, false);

        flashImage = imgGO.AddComponent<Image>();
        flashImage.color         = Color.clear;
        flashImage.raycastTarget = false;

        RectTransform rt = imgGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // ── 외부 호출 ──────────────────────────────────────────────────────
    public void Play()
    {
        if (playing) return;
        playing = true;
        StartCoroutine(ShakeRoutine());
        StartCoroutine(FlashRoutine());
    }

    // ── 카메라 흔들기 ──────────────────────────────────────────────────
    IEnumerator ShakeRoutine()
    {
        Vector3 origin  = mainCam.transform.position;
        float   elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float t        = elapsed / shakeDuration;
            float strength = shakeMagnitude * (1f - t * t);   // ease-out
            mainCam.transform.position = origin +
                (Vector3)(Random.insideUnitCircle * strength);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCam.transform.position = origin;
    }

    // ── 화면 플래시 ────────────────────────────────────────────────────
    IEnumerator FlashRoutine()
    {
        if (flashImage == null) yield break;

        // 즉시 빨간 화면
        flashImage.color = flashColor;
        yield return null;

        // 서서히 투명해짐
        float elapsed = 0f;
        while (elapsed < flashFadeTime)
        {
            float t    = elapsed / flashFadeTime;
            Color c    = flashColor;
            c.a        = Mathf.Lerp(flashColor.a, 0f, t * t);   // ease-in
            flashImage.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }
        flashImage.color = Color.clear;
        playing = false;
    }
}
