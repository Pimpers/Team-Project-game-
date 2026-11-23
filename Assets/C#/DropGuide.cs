using UnityEngine;

// 동글이 위치에서 아래로 뻗는 점선 가이드라인
// 빈 게임오브젝트에 붙이고 LineRenderer 컴포넌트를 추가해주세요
[RequireComponent(typeof(LineRenderer))]
public class DropGuide : MonoBehaviour
{
    public GameManager manager;

    [Header("가이드라인 설정")]
    public float guideLength = 15f;   // 선 길이
    public int dotCount = 12;         // 점 개수
    public float dotSpacing = 0.5f;   // 점 간격 (LineRenderer 텍스처 없이 점선 표현 시)

    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = 0.04f;
        lr.endWidth = 0.04f;
        lr.useWorldSpace = true;

        // 색상: 흰색 반투명
        lr.startColor = new Color(1f, 1f, 1f, 0.5f);
        lr.endColor   = new Color(1f, 1f, 1f, 0.1f);
    }

    void Update()
    {
        if (manager == null || manager.lastDongle == null || manager.lastDongle.isDropped)
        {
            lr.enabled = false;
            return;
        }

        lr.enabled = true;

        Vector3 top = manager.lastDongle.transform.position;
        Vector3 bottom = new Vector3(top.x, top.y - guideLength, top.z);

        lr.SetPosition(0, top);
        lr.SetPosition(1, bottom);
    }
}
