using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public int damage = 1;
    private int nextMove;

    private Rigidbody2D rb;
    private Animator enemyAnim;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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

        // 'Ground' 확인 (앞쪽에 Ground 없으면 방향 반전)
        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.5f, rb.position.y - 0.2f);
        Debug.DrawRay(frontVec, Vector3.down, Color.green);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));
        
        if (rayHit.collider == null)
        {
            nextMove *= -1;

            // 현재 작동 중인 모든 Invoke 함수를 취소하는 함수
            CancelInvoke();

            Invoke("Think", 5);
        }
    }
    private void Think()    // 적의 움직임을 변경하는 메서드
    {
        nextMove = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(2f, 5f);

        // 재귀 함수
        Invoke("Think", nextThinkTime);

        // SpriteAnimation
        enemyAnim.SetInteger("Run", nextMove);
        // Think()함수를 실행할 때마다, Run 을 nextMove로 바꾼다
        // Condition 에서 설정해주었기 때문에 nextMove와 값이 같은 
        // Run 의 값에 따라 애니메이션이 바뀐다.     
    }
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.velocity = Vector2.zero;
        enemyAnim.SetTrigger("Die");    
        this.enabled = false;  // 움직임 멈추기
        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 hitDirection = (collision.transform.position - transform.position).normalized;

            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.PlayerHurt(hitDirection, damage);
            }
        }
    }
}
