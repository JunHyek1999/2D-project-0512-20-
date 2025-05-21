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

            // ���� ĳ���� ���� �ݵ�
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
            }

            // ��ü �ݶ��̴� ��Ȱ��ȭ �� ���� ���ϵ���
            Collider2D bodyCollider = enemyMove.GetComponent<Collider2D>();
            if (bodyCollider != null)
            {
                bodyCollider.enabled = false;
            }

            GetComponent<Collider2D>().enabled = false;
        }
    }
}
