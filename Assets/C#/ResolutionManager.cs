using UnityEngine;

// 빈 게임오브젝트에 붙여서 사용. Main Camera가 씬에 있어야 합니다.
public class ResolutionManager : MonoBehaviour
{
    [Header("Target Aspect Ratio (세로형 9:16)")]
    public int targetWidth = 9;
    public int targetHeight = 16;

    Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        ApplyAspectRatio();
    }

    void ApplyAspectRatio()
    {
        // 카메라를 전체 화면으로 표시 — 좌우 사이드바는 Canvas Image 패널로 처리
        mainCamera.rect = new Rect(0, 0, 1, 1);
    }

}
