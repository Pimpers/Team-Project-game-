using UnityEngine;

// TMP 텍스트 오브젝트에 붙여서 사용.
// boardTransform(SCore 월드 스프라이트)의 화면 위치를 매 프레임 따라다님.
[RequireComponent(typeof(RectTransform))]
public class ScoreBoardUI : MonoBehaviour
{
    public Transform boardTransform;

    [Tooltip("월드 좌표 오프셋 — boardTransform 위치에 더해진 뒤 스크린 좌표로 변환됩니다.")]
    public Vector2 worldOffset = Vector2.zero;

    RectTransform rt;
    RectTransform canvasRT;
    Camera mainCam;

    void Awake()
    {
        rt       = GetComponent<RectTransform>();
        canvasRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        mainCam  = Camera.main;

        // 앵커를 (0,0) 고정, 피벗 중앙 → anchoredPosition = 캔버스 절대 좌표
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot     = new Vector2(0.5f, 0.5f);
    }

    void LateUpdate()
    {
        if (boardTransform == null || mainCam == null) return;

        Vector3 worldPos = boardTransform.position + new Vector3(worldOffset.x, worldOffset.y, 0f);
        Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

        // Screen Space Overlay → worldCamera = null
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT, screenPos, null, out localPos);

        rt.anchoredPosition = localPos;
    }
}
