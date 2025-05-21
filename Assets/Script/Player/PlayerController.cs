using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("ĳ���� HP")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    [Header("ĳ���� �̵��ӵ�")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float runSpeed = 8.0f;

    [Header("ĳ���� ������")]
    [SerializeField] private float jumpPower = 10.0f;
    [SerializeField] private float lowJumpMultiplier = 2.0f;

    [Header("Ground Layer ����")]
    [SerializeField] private LayerMask groundLayer;

    [Header("ü�� ������")]
    public HeartManager heartManager;

    private Rigidbody2D rb;
    private BoxCollider2D playerBC;
    private Animator playerAnim;

    private float moveX;

    private bool isInvincible = false;
    [SerializeField] private float invincibleTime = 1.0f;

    private bool isKnockback = false;
    private bool isGrounded = false;
    private bool isDead = false;


    private void Awake()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        playerBC = rb.GetComponent<BoxCollider2D>();
        playerAnim = GetComponent<Animator>();
    }
    private void Start()
    {
        if (HeartManager.Instance != null)
        {
            HeartManager.Instance.UpdateHearts(currentHealth);
        }
    }
    private void Update()
    {
        MoveMent();      // �̵�
        CheckGrounded(); // �� üũ

        moveX = Input.GetAxisRaw("Horizontal"); 

        // ���� �Է�
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            PlayerJump();
            Debug.Log("���� ����");
        }

        // ª��/��� ����
        if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // �����̽��ٿ��� ���� ���� ������ ����
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // ������ �Է�
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isGrounded)
            {
                playerAnim.SetBool("isThrowing", true);
            }
            else
            {
                playerAnim.SetBool("isJumpThrowing", true);
            }
            // �ִϸ��̼� ������ false�� ������
            Invoke(nameof(ResetThrowBools), 0.5f);
        }

        // ������ �ȱ� (z Ű)
        playerAnim.SetBool("isHappyWalk", Input.GetKey(KeyCode.Z));

        // �ȱ� / �޸��� ����
        playerAnim.SetBool("isWalking", moveX != 0 && !Input.GetKey(KeyCode.LeftShift));
        playerAnim.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift) && moveX != 0);

        // ���� ����
        playerAnim.SetBool("isJumping", !isGrounded && rb.velocity.y > 0);

        // ���� ����
        playerAnim.SetBool("isFalling", !isGrounded && rb.velocity.y < 0);

        // �ִϸ��̼� ó��
        if (playerAnim != null)
        {
            playerAnim.SetBool("isGrounded", isGrounded);
        }

        // ĳ���� ���� ��ȯ
        if (moveX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveX), 1, 1); //�¿� ����
        }

        // ���� ���� �� �Ӹ� �� ��� üũ
        if (rb.velocity.y > 0 && isGrounded == false)
        {
            CheckCeilingHit();
        }
    }
   
    private void ResetThrowBools()
    {
        playerAnim.SetBool("isThrowing", false);
        playerAnim.SetBool("isJumpThrowing", false);
    }
    private void MoveMent()
    {
        if (isKnockback) return;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.velocity = new Vector2(moveX * currentSpeed, rb.velocity.y);
    }
    private void PlayerJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
    }
    private void CheckGrounded()
    {
        Bounds bounds = playerBC.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y - 0.05f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, groundLayer);
       
        isGrounded = hit.collider != null;

        Debug.DrawRay(origin, Vector2.down * 0.1f, isGrounded ? Color.green : Color.red);
    }
    public void PlayerHurt(Vector2 hitDirection, int damage)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        heartManager.UpdateHearts(currentHealth); // ��Ʈ UI ����

        // �˹�
        isKnockback = true;
        rb.velocity = Vector2.zero; // ���� �ӵ� �ʱ�ȭ
        rb.AddForce(new Vector2(hitDirection.x * 10f, 2f), ForceMode2D.Impulse); // �˹� ���� + ����


        Debug.Log($"������� �Ծ���! ���� ü�� : {currentHealth}");
        playerAnim.SetTrigger("isHurt");

        if (currentHealth <= 0)
        {
            Die();
        }

        // �ǰ� �� ���� �ð� ����
        StartCoroutine(InvincibilityCo());
        // 0.3�� �� �̵� ����
        StartCoroutine(EndKnockbackAfterTimeCo(0.3f));
    }
    private IEnumerator InvincibilityCo()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
    private IEnumerator EndKnockbackAfterTimeCo(float duration)
    {
        yield return new WaitForSeconds(duration);
        isKnockback = false;
    }
    public void Die()
    {
        Debug.Log("�÷��̾� ��� ");

        if (isDead) return;

        isDead = true;

        // �̵� ���߱�
        rb.velocity = Vector2.zero;

        // ���� ��Ȱ��ȭ �� ������ �и��� �ʵ���
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        // �״� �ִϸ��̼� ����
        playerAnim.SetTrigger("isDead");

        // ���� ��Ȱ��ȭ
        this.enabled = false;

        // 2�� �� ���� ���� ������ ��ȯ
        Invoke("GoToGameOver", 2f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // '��'�� �浹 ��
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 hitDir = (transform.position - collision.transform.position).normalized;

            int damage = 1;
            EnemyMove enemy = collision.gameObject.GetComponent<EnemyMove>();
            if (enemy != null)
            {
                damage = enemy.damage;
            }
            PlayerHurt(hitDir, damage);
        }

        // '����'�� �浹 �� ��� ó��
        if (collision.gameObject.CompareTag("Food"))
        {
            if (currentHealth < maxHealth)
            {
                if (currentHealth < maxHealth)
                {
                    currentHealth += 1;
                    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // ���� ����
                    heartManager.UpdateHearts(currentHealth); // ��Ʈ ����

                    Debug.Log("ü�� ȸ��! ���� ü�� : " + currentHealth);
                }
                else
                {
                    Debug.Log("�̹� �ִ� ü���Դϴ�.");
                }

                // ���� ������Ʈ ����
                Destroy(collision.gameObject);
            }
        }
    }

    private void CheckCeilingHit()
    {
        // �÷��̾� �ݶ��̴� �������� ���� �߾� ��ġ ���
        Bounds bounds = playerBC.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.max.y + 0.05f);

        // Raycast�� �Ӹ� �� ��� ����
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, 0.1f, LayerMask.GetMask("Block"));

        Debug.DrawRay(origin, Vector2.up * 0.1f, hit.collider ? Color.green : Color.red);

        if (hit.collider != null)
        {
            // �浹�� ���� ĳ���� ���� �ӵ� 0 ���� �����
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            // �Ϲ� ����� ��� �� �� ������� ��ü
            if (hit.collider.TryGetComponent(out BreakableBlock normalBlock))
            {
                normalBlock.BreakToCracked();
            }
            // �̹� �� �� ����̶�� ������ ��ȯ �� ��� �ı�
            else if (hit.collider.TryGetComponent(out CrackedBlock crackedBlock))
            {
                crackedBlock.BreakAndSpawnItem();
            }
        }
    }
    private void GoToGameOver()
    {
        Debug.Log("���� ���� ������ ��ȯ�մϴ�");
        SceneManager.LoadScene("GameOver");
    }

}
