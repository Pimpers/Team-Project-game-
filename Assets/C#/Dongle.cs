using System.Collections;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public GameManager manager;
    public ParticleSystem effect;
    public int level;
    public bool isDrag;
    public bool isMerge;
    public bool isDropped;  // Drop() 호출 후 true, 풀 반환 시 false
    public bool hasLanded;  // 떨어진 후 처음 충돌 시 true
    public int[] levelScores;
    public float[] levelRadii;

    public Rigidbody2D rigid;
    CircleCollider2D circle;
    Animator anim;
    SpriteRenderer spriteRenderer;
    float deadTime;

    // WaitForSeconds 인스턴스를 공유해서 GC 할당 최소화
    static readonly WaitForSeconds wait02s = new WaitForSeconds(0.2f);
    static readonly WaitForSeconds wait03s = new WaitForSeconds(0.3f);

    // 위험 색상 캐시
    static readonly Color dangerColor = new Color(0.9f, 0.2f, 0.2f);

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        circle = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        isDrag = false;
        isMerge = false;
        isDropped = false;
        hasLanded = false;
        deadTime = 0f;
        spriteRenderer.color = Color.white;
        anim.SetInteger("level", level);
        ApplyRadius();
    }

    void OnDisable()
    {
        // 풀로 반환될 때 물리 상태 초기화
        rigid.simulated = false;
        rigid.linearVelocity = Vector2.zero;
        rigid.angularVelocity = 0;
        transform.rotation = Quaternion.identity;
        if (manager != null)
        {
            manager.ReturnDongle(this);
            manager = null;
        }
    }

    void ApplyRadius()
    {
        if (levelRadii != null && level < levelRadii.Length)
        {
            circle.radius = levelRadii[level];
            circle.enabled = false;
            circle.enabled = true;
        }
    }

    // 위치 추적은 GameManager.Update()에서 처리

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        isDropped = true;
        rigid.simulated = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDropped) return;

        // 다른 공과의 충돌은 무조건 착지로 판정
        if (collision.gameObject.CompareTag("Dongle"))
        {
            hasLanded = true;
            return;
        }

        // 벽(수평 법선)과의 충돌은 착지로 판정하지 않음
        // 바닥(수직 법선, |normal.y| > 0.5)만 착지로 인정
        if (collision.contactCount > 0)
        {
            float normalY = Mathf.Abs(collision.GetContact(0).normal.y);
            if (normalY > 0.5f) hasLanded = true;
        }
        else
        {
            hasLanded = true; // 연락처 정보 없으면 일반 착지로 처리
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Dongle")) return;

        Dongle other = collision.gameObject.GetComponent<Dongle>();
        if (level != other.level || isMerge || other.isMerge || level >= 10) return;

        float meY = transform.position.y;
        float otherY = other.transform.position.y;
        float meX = transform.position.x;
        float otherX = other.transform.position.x;

        if (meY < otherY || (meY == otherY && meX > otherX))
        {
            other.Hide(transform.position);
            LevelUp();
        }
    }

    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        rigid.simulated = false;
        circle.enabled = false;
        if (targetPos == Vector3.up * 100) EffectPlay();
        gameObject.SetActive(false);
    }

    void LevelUp()
    {
        isMerge = true;
        rigid.simulated = false;
        rigid.linearVelocity = Vector2.zero;
        rigid.angularVelocity = 0;
        StartCoroutine(LevelUpRoutine());
    }

    IEnumerator LevelUpRoutine()
    {
        yield return wait02s;

        int addScore = (levelScores != null && level < levelScores.Length)
            ? levelScores[level]
            : (level + 1) * 10;
        if (Score.Instance != null) Score.Instance.AddScore(addScore);

        level++;
        anim.SetInteger("level", level);

        if (manager != null) manager.PlaySFX(GameManager.SFX.LevelUp);
        EffectPlay();
        ApplyRadius();

        if (manager != null) manager.maxLevel = Mathf.Max(level, manager.maxLevel);

        yield return wait03s;
        rigid.simulated = true;
        isMerge = false;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Finish")) return;

        deadTime += Time.deltaTime;
        if (deadTime > 2f) spriteRenderer.color = dangerColor;
        if (deadTime > 5f) manager.GameOver();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Finish")) return;

        // 아웃라인 밖으로 나가면 타이머와 색상 초기화
        deadTime = 0f;
        spriteRenderer.color = Color.white;
    }

    void EffectPlay()
    {
        if (effect != null)
        {
            effect.transform.position = transform.position;
            effect.Play();
        }
    }
}
