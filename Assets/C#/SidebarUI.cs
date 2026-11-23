using UnityEngine;
using UnityEngine.UI;

// 좌우 사이드바를 자동으로 채우고 반투명 배경을 추가하는 패널
// CanvasScaler Match Height(1) 모드 전제 — canvas 높이 = 960 고정
[RequireComponent(typeof(RectTransform))]
public class SidebarUI : MonoBehaviour
{
    public enum Side { Left, Right }
    public Side side = Side.Right;

    [Range(0f, 1f)]
    public float backgroundAlpha = 0.55f;

    RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();

        // 반투명 검은 배경 이미지 런타임 추가
        Image bg = GetComponent<Image>();
        if (bg == null) bg = gameObject.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, backgroundAlpha);
        bg.raycastTarget = false;
    }

    void LateUpdate() => Apply();

    void Apply()
    {
        // canvas 높이 960 고정, 너비는 화면 비율에 따라 결정
        float canvasWidth = 960f * Screen.width / Screen.height;
        float gameWidth   = 960f * 9f / 16f; // 540
        float sidebarWidth = (canvasWidth - gameWidth) * 0.5f;

        if (sidebarWidth < 5f)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        if (side == Side.Left)
        {
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot     = new Vector2(0, 0.5f);
        }
        else
        {
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot     = new Vector2(1, 0.5f);
        }
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(sidebarWidth, 0);
    }
}
