using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public int damage = 1;          // 적 대미지 
    private int nextMove;           // 이동 방향

    private Rigidbody2D rb;
    private Animator enemyAnim;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;        // 사망 여부

    private void Awake()
    {
        // 컴포넌트 초기화
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 5초 후 적의 움직임 방향 결정
        Invoke("Think", 5);
        
    }
    void FixedUpdate()
    {
        // 이동
        rb.velocity = new Vector2(nextMove, rb.velocity.y);

        // 방향에 따라 스프라이트 반전
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == -1;
        }

        // 'Ground'레이어 확인 (앞쪽에 Ground 없으면 방향 반전)
        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.5f, rb.position.y - 0.2f);
        Debug.DrawRay(frontVec, Vector3.down, Color.green);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));

        // 바닥이 없으면 방향 반전
        if (rayHit.collider == null)
        {
            nextMove *= -1;

            // 현재 작동 중인 모든 Invoke 함수를 취소하는 함수
            CancelInvoke();
            Invoke("Think", 5);
        }
    }
    private void Think()    // 적이 일정 시간마다 방향을 랜덤하게 바꾸는 함수
    {
        // -1(왼쪽), 0(정지), 1(오른쪽) 중 무작위 선택
        nextMove = Random.Range(-1, 2);

        // 다음 Think 실행 시간도 랜덤
        float nextThinkTime = Random.Range(2f, 5f);

        // 재귀 함수
        Invoke("Think", nextThinkTime);

        // 애니메이션에 현재 방향 값 전달
        enemyAnim.SetInteger("Run", nextMove);
     
    }
    public void Die()       // 적이 죽었을 때 호출되는 함수
    {
        if (isDead) return;
        isDead = true;

        // 이동 멈춤
        rb.velocity = Vector2.zero;

        // 죽는 애니메이션 실행
        enemyAnim.SetTrigger("Die");

        // 스크립트 비활성화 → 더 이상 움직이지 않도록
        this.enabled = false;

        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)      // 플레이어와 충돌했을 때 실행
    {
        // 충돌한 오브젝트가 '플레이어'라면
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어가 적 기준 어느 방향에 있는지 계산
            Vector2 hitDirection = (collision.transform.position - transform.position).normalized;

            // 플레이어의 PlayerController 가져오기
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // 플레이어에게 데미지 전달
                player.PlayerHurt(hitDirection, damage);
            }
        }
    }
}
