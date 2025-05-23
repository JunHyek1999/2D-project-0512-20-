using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public int damage = 1;          // �� ����� 
    private int nextMove;           // �̵� ����

    private Rigidbody2D rb;
    private Animator enemyAnim;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;        // ��� ����

    private void Awake()
    {
        // ������Ʈ �ʱ�ȭ
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 5�� �� ���� ������ ���� ����
        Invoke("Think", 5);
        
    }
    void FixedUpdate()
    {
        // �̵�
        rb.velocity = new Vector2(nextMove, rb.velocity.y);

        // ���⿡ ���� ��������Ʈ ����
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == -1;
        }

        // 'Ground'���̾� Ȯ�� (���ʿ� Ground ������ ���� ����)
        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.5f, rb.position.y - 0.2f);
        Debug.DrawRay(frontVec, Vector3.down, Color.green);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));

        // �ٴ��� ������ ���� ����
        if (rayHit.collider == null)
        {
            nextMove *= -1;

            // ���� �۵� ���� ��� Invoke �Լ��� ����ϴ� �Լ�
            CancelInvoke();
            Invoke("Think", 5);
        }
    }
    private void Think()    // ���� ���� �ð����� ������ �����ϰ� �ٲٴ� �Լ�
    {
        // -1(����), 0(����), 1(������) �� ������ ����
        nextMove = Random.Range(-1, 2);

        // ���� Think ���� �ð��� ����
        float nextThinkTime = Random.Range(2f, 5f);

        // ��� �Լ�
        Invoke("Think", nextThinkTime);

        // �ִϸ��̼ǿ� ���� ���� �� ����
        enemyAnim.SetInteger("Run", nextMove);
     
    }
    public void Die()       // ���� �׾��� �� ȣ��Ǵ� �Լ�
    {
        if (isDead) return;
        isDead = true;

        // �̵� ����
        rb.velocity = Vector2.zero;

        // �״� �ִϸ��̼� ����
        enemyAnim.SetTrigger("Die");

        // ��ũ��Ʈ ��Ȱ��ȭ �� �� �̻� �������� �ʵ���
        this.enabled = false;

        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)      // �÷��̾�� �浹���� �� ����
    {
        // �浹�� ������Ʈ�� '�÷��̾�'���
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾ �� ���� ��� ���⿡ �ִ��� ���
            Vector2 hitDirection = (collision.transform.position - transform.position).normalized;

            // �÷��̾��� PlayerController ��������
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // �÷��̾�� ������ ����
                player.PlayerHurt(hitDirection, damage);
            }
        }
    }
}
