using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("캐릭터 HP")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    [Header("캐릭터 이동속도")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float runSpeed = 8.0f;

    [Header("캐릭터 점프력")]
    [SerializeField] private float jumpPower = 10.0f;
    [SerializeField] private float lowJumpMultiplier = 2.0f;

    [Header("Ground Layer 설정")]
    [SerializeField] private LayerMask groundLayer;

    [Header("체력 프리팹")]
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
        MoveMent();      // 이동
        CheckGrounded(); // 땅 체크

        moveX = Input.GetAxisRaw("Horizontal"); 

        // 점프 입력
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            PlayerJump();
            Debug.Log("점프 시작");
        }

        // 짧게/길게 점프
        if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // 스페이스바에서 손을 떼면 빠르게 낙하
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // 던지기 입력
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
            // 애니메이션 끝나고 false로 돌리기
            Invoke(nameof(ResetThrowBools), 0.5f);
        }

        // 웃으며 걷기 (z 키)
        playerAnim.SetBool("isHappyWalk", Input.GetKey(KeyCode.Z));

        // 걷기 / 달리기 상태
        playerAnim.SetBool("isWalking", moveX != 0 && !Input.GetKey(KeyCode.LeftShift));
        playerAnim.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift) && moveX != 0);

        // 점프 상태
        playerAnim.SetBool("isJumping", !isGrounded && rb.velocity.y > 0);

        // 낙하 상태
        playerAnim.SetBool("isFalling", !isGrounded && rb.velocity.y < 0);

        // 애니메이션 처리
        if (playerAnim != null)
        {
            playerAnim.SetBool("isGrounded", isGrounded);
        }

        // 캐릭터 방향 전환
        if (moveX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveX), 1, 1); //좌우 반전
        }

        // 점프 중일 때 머리 위 블록 체크
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
        heartManager.UpdateHearts(currentHealth); // 하트 UI 갱신

        // 넉백
        isKnockback = true;
        rb.velocity = Vector2.zero; // 기존 속도 초기화
        rb.AddForce(new Vector2(hitDirection.x * 10f, 2f), ForceMode2D.Impulse); // 넉백 방향 + 세기


        Debug.Log($"대미지를 입었다! 남은 체력 : {currentHealth}");
        playerAnim.SetTrigger("isHurt");

        if (currentHealth <= 0)
        {
            Die();
        }

        // 피격 후 일정 시간 무적
        StartCoroutine(InvincibilityCo());
        // 0.3초 뒤 이동 가능
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
        Debug.Log("플레이어 사망 ");

        if (isDead) return;

        isDead = true;

        // 이동 멈추기
        rb.velocity = Vector2.zero;

        // 물리 비활성화 → 적에게 밀리지 않도록
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        // 죽는 애니메이션 실행
        playerAnim.SetTrigger("isDead");

        // 조작 비활성화
        this.enabled = false;

        // 2초 뒤 게임 오버 씬으로 전환
        Invoke("GoToGameOver", 2f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // '적'과 충돌 시
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

        // '음식'과 충돌 시 흡수 처리
        if (collision.gameObject.CompareTag("Food"))
        {
            if (currentHealth < maxHealth)
            {
                if (currentHealth < maxHealth)
                {
                    currentHealth += 1;
                    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 예외 방지
                    heartManager.UpdateHearts(currentHealth); // 하트 갱신

                    Debug.Log("체력 회복! 현재 체력 : " + currentHealth);
                }
                else
                {
                    Debug.Log("이미 최대 체력입니다.");
                }

                // 음식 오브젝트 제거
                Destroy(collision.gameObject);
            }
        }
    }

    private void CheckCeilingHit()
    {
        // 플레이어 콜라이더 기준으로 위쪽 중앙 위치 계산
        Bounds bounds = playerBC.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.max.y + 0.05f);

        // Raycast로 머리 위 블록 감지
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, 0.1f, LayerMask.GetMask("Block"));

        Debug.DrawRay(origin, Vector2.up * 0.1f, hit.collider ? Color.green : Color.red);

        if (hit.collider != null)
        {
            // 충돌한 순간 캐릭터 점프 속도 0 으로 만들기
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            // 일반 블록일 경우 금 간 블록으로 교체
            if (hit.collider.TryGetComponent(out BreakableBlock normalBlock))
            {
                normalBlock.BreakToCracked();
            }
            // 이미 금 간 블록이라면 아이템 소환 및 블록 파괴
            else if (hit.collider.TryGetComponent(out CrackedBlock crackedBlock))
            {
                crackedBlock.BreakAndSpawnItem();
            }
        }
    }
    private void GoToGameOver()
    {
        Debug.Log("게임 오버 씬으로 전환합니다");
        SceneManager.LoadScene("GameOver");
    }

}
