using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeadTrigger : MonoBehaviour
{
    private EnemyMove enemyMove;

    private void Awake()
    {
        enemyMove = GetComponentInParent<EnemyMove>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemyMove.Die();

            // 밟은 캐릭터 위로 반동
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
            }

            // 본체 콜라이더 비활성화 → 공격 못하도록
            Collider2D bodyCollider = enemyMove.GetComponent<Collider2D>();
            if (bodyCollider != null)
            {
                bodyCollider.enabled = false;
            }

            GetComponent<Collider2D>().enabled = false;
        }
    }
}
