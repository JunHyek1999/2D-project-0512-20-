using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("ĳ���� HP")]
    [SerializeField] private int maxHealth = 5;             // �ִ� ü��
    private int currentHealth;                              // ���� ü��

    [Header("ĳ���� �̵�/����")]
    [SerializeField] private float walkSpeed = 5.0f;            // �ȱ� �ӵ�
    [SerializeField] private float runSpeed = 8.0f;             // �޸��� �ӵ�
    [SerializeField] private float jumpPower = 10.0f;           // ������
    [SerializeField] private float lowJumpMultiplier = 2.0f;    // ���� ��ư�� ª�� ���� ��� ���� ���ӵ�

    [Header("���� ���� ���̾�")]
    [SerializeField] private LayerMask groundLayer;             // �÷��̾ �� ���� �ٴ� ���̾�

    [Header("��Ʈ UI")]
    public HeartManager heartManager;                           // ��Ʈ UI�� �����ϴ� ��ũ��Ʈ

    private Rigidbody2D rb;
    private BoxCollider2D playerBC;
    private Animator playerAnim;

    private float moveX;                                        // �¿� �Է� ��
    private bool isGrounded = false;                            // ���鿡 ��Ҵ��� ����
    private bool isKnockback = false;                           // �˹� ���� ����
    private bool isInvincible = false;                          // ���� ���� ����
    [SerializeField] private float invincibleTime = 1.0f;       // ���� ���� �ð�


    private bool isDead = false;                                // ��� ����


    private void Awake()
    {
        // ������Ʈ �ʱ�ȭ
        rb = GetComponent<Rigidbody2D>();
        playerBC = rb.GetComponent<BoxCollider2D>();
        playerAnim = GetComponent<Animator>();
        
        // ü�� �ʱ�ȭ
        currentHealth = maxHealth;
    }
    private void Start()
    {
        // ��Ʈ UI �ʱ�ȭ
        if (HeartManager.Instance != null)
        {
            HeartManager.Instance.UpdateHearts(currentHealth);
        }
    }
    private void Update()
    {
        // �¿� �̵� �Է�
        moveX = Input.GetAxisRaw("Horizontal");

        // �� ��ɺ� ó�� �Լ� ȣ��
        HandleMovement();
        HandleJump();
        HandleAnimations();
        HandleThrow();
        HandleDirection();

        // �Ӹ� �� ��� �浹 üũ
        CheckCeilingHit();
    }
    private void HandleMovement()   // �̵� ó��
    {
        // �˹� ������ �� �̵� �Ұ�
        if (isKnockback) return;

        // Shift Ű�� �޸���
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        // �ӵ� ����
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
    }
    private void HandleJump()   // ���� ó��
    {
        CheckGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // ����
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            Debug.Log("���� ����");
        }

        // ª�� ���� �� ���� ����
        if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    private void HandleThrow()   // ������ �ִϸ��̼� ó��
    {
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

            Invoke(nameof(ResetThrowBools), 0.5f);      // ���� �ð� �� �ִϸ��̼� ����
        }
    }

    private void ResetThrowBools()   // ������ �ִϸ��̼� ���� �ʱ�ȭ
    {
        playerAnim.SetBool("isThrowing", false);
        playerAnim.SetBool("isJumpThrowing", false);
    }
    private void HandleAnimations()     // �ִϸ��̼� ���� ����
    {
        playerAnim.SetBool("isHappyWalk", Input.GetKey(KeyCode.Z));
        playerAnim.SetBool("isWalking", moveX != 0 && !Input.GetKey(KeyCode.LeftShift));
        playerAnim.SetBool("isRunning", moveX != 0 && Input.GetKey(KeyCode.LeftShift));
        playerAnim.SetBool("isJumping", !isGrounded && rb.velocity.y > 0);
        playerAnim.SetBool("isFalling", !isGrounded && rb.velocity.y < 0);
        playerAnim.SetBool("isGrounded", isGrounded);
    }
    private void HandleDirection()      // ���� ��ȯ ó��
    {
        if (moveX != 0)
        {
            // ����/������ ��ȯ
            transform.localScale = new Vector3(Mathf.Sign(moveX), 1, 1);    
        }
    }
    private void CheckGrounded()        // �ٴڿ� ��Ҵ��� ����
    {
        Bounds bounds = playerBC.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y - 0.05f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, groundLayer);
       
        isGrounded = hit.collider != null;

        Debug.DrawRay(origin, Vector2.down * 0.1f, isGrounded ? Color.green : Color.red);
    }
    public void PlayerHurt(Vector2 hitDirection, int damage)        // �÷��̾ ������ �¾��� �� ����
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        // ü�� �ּ� 0 ����
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // ��Ʈ UI ����
        heartManager.UpdateHearts(currentHealth); 

        // �˹� ó��
        isKnockback = true;
        // ���� �ӵ� �ʱ�ȭ
        rb.velocity = Vector2.zero;
        // �˹� ���� + ����
        rb.AddForce(new Vector2(hitDirection.x * 10f, 2f), ForceMode2D.Impulse); 

        playerAnim.SetTrigger("isHurt");
        Debug.Log($"������� �Ծ���! ���� ü�� : {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }

        // ���� �ð� �� �˹� ȸ�� ó��
        StartCoroutine(InvincibilityCo());
        StartCoroutine(EndKnockbackAfterTimeCo(0.3f));
    }
    private IEnumerator InvincibilityCo()       // ���� ���� ���� �ڷ�ƾ
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
    private IEnumerator EndKnockbackAfterTimeCo(float duration)     // �˹� ���� ���� �ڷ�ƾ
    {
        yield return new WaitForSeconds(duration);
        isKnockback = false;
    }
    public void Die()       // �÷��̾� ��� ó��
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
    private void GoToGameOver()     // ���� ���� �� ��ȯ
    {
        Debug.Log("���� ���� ������ ��ȯ�մϴ�");
        SceneManager.LoadScene("GameOver");
    }
    private void OnCollisionEnter2D(Collision2D collision)      // �浹 ó��
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

        // '����'�� �浹 �� ü�� ȸ��
        if (collision.gameObject.CompareTag("Food"))
        {
            if (currentHealth < maxHealth)
            {
                currentHealth++;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                heartManager.UpdateHearts(currentHealth);

                Debug.Log("ü�� ȸ��! ���� ü�� : " + currentHealth);
            }
            else
            {
                Debug.Log("�̹� �ִ� ü���Դϴ�.");
            }

            Destroy(collision.gameObject);
        }
    }

    private void CheckCeilingHit()      // �Ӹ� �� ��� �浹 üũ
    {
        if (rb.velocity.y <= 0 || isGrounded) return;

        // �÷��̾� �ݶ��̴� �������� ���� �߾� ��ġ ���
        Bounds bounds = playerBC.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.max.y + 0.05f);

        // Raycast�� �Ӹ� �� ��� ����
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, 0.1f, LayerMask.GetMask("Block"));
        Debug.DrawRay(origin, Vector2.up * 0.1f, hit.collider ? Color.green : Color.red);

        if (hit.collider != null)
        {
            // �浹�� ���� ���� �ߴ�
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            // �Ϲ� ����� ��� �� �� ������� ��ü
            if (hit.collider.TryGetComponent(out BreakableBlock normalBlock))
            {
                normalBlock.BreakToCracked();
            }
            // �� �� ����� ��� ������ ��ȯ �� ��� �ı�
            else if (hit.collider.TryGetComponent(out CrackedBlock crackedBlock))
            {
                // ��� ��鸲 ����
                crackedBlock.ShakeBlock();
                StartCoroutine(BreakCrackedBlockDelayed(crackedBlock, 0.25f));
            }
        }
    }
    private IEnumerator BreakCrackedBlockDelayed(CrackedBlock block, float delay)       // �� �� ��� �ı� �� ������ ���� ���� ó��
    {
        yield return new WaitForSeconds(delay);
        if (block != null)
        {
            block.BreakAndSpawnItem();
        }
    }
    

}
