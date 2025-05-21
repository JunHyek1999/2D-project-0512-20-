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

    // 밟힘 심 죽는 메서드
    public void OnDamaged()
    {
        // 밟힘 시, 색깔 변환을 위해
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 밟힘 시, 위 아래 바꿈
        spriteRenderer.flipY = true;

        // 밟힘 시, 아래로 추락하게 하도록
        bc.enabled = false;

        // 밟힘 시, 위로 약간 뜨면서 추락하게 하도록 AddForce();
        rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // Destroy
        Invoke("DeActive", 5);
    }

    // 밟고 추락하며 사라지게 하도록 SetActive(false)를 이용한 DeActive()
    void DeActive()
    {
        gameObject.SetActive(false);
    }
  
}
