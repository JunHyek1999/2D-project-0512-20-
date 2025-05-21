using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D bc;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    // ���� �� �״� �޼���
    public void OnDamaged()
    {
        // ���� ��, ���� ��ȯ�� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // ���� ��, �� �Ʒ� �ٲ�
        spriteRenderer.flipY = true;

        // ���� ��, �Ʒ��� �߶��ϰ� �ϵ���
        bc.enabled = false;

        // ���� ��, ���� �ణ �߸鼭 �߶��ϰ� �ϵ��� AddForce();
        rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // Destroy
        Invoke("DeActive", 5);
    }

    // ��� �߶��ϸ� ������� �ϵ��� SetActive(false)�� �̿��� DeActive()
    void DeActive()
    {
        gameObject.SetActive(false);
    }
  
}
