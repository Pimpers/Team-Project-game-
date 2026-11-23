using UnityEngine;

// 다음에 떨어질 동글을 미리 보여주는 순수 시각 컴포넌트.
// Rigidbody2D / Collider 없이 Animator 만 사용.
[RequireComponent(typeof(Animator))]
public class NextPreview : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetLevel(int level)
    {
        anim.SetInteger("level", level);
    }
}
