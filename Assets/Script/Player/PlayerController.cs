using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("캐릭터 HP")]
    [SerializeField] private int maxHealth = 5;             // 최대 체력
    private int currentHealth;                              // 현재 체력

    [Header("캐릭터 이동/점프")]
    [SerializeField] private float walkSpeed = 5.0f;            // 걷기 속도
    [SerializeField] private float runSpeed = 8.0f;             // 달리기 속도
    [SerializeField] private float jumpPower = 10.0f;           // 점프력
    [SerializeField] private float lowJumpMultiplier = 2.0f;    // 점프 버튼을 짧게 누를 경우 낙하 가속도

    [Header("지면 감지 레이어")]
    [SerializeField] private LayerMask groundLayer;             // 플레이어가 서 있을 바닥 레이어

    [Header("하트 UI")]
    public HeartManager heartManager;                           // 하트 UI를 관리하는 스크립트

    private Rigidbody2D rb;
    private BoxCollider2D playerBC;
    private Animator playerAnim;

    private float moveX;                                        // 좌우 입력 값
    private bool isGrounded = false;                            // 지면에 닿았는지 여부
    private bool isKnockback = false;                           // 넉백 상태 여부
    private bool isInvincible = false;                          // 무적 상태 여부
    [SerializeField] private float invincibleTime = 1.0f;       // 무적 지속 시간


    private bool isDead = false;                                // 사망 여부


    private void Awake()
    {
        // 컴포넌트 초기화
        rb = GetComponent<Rigidbody2D>();
        playerBC = rb.GetComponent<BoxCollider2D>();
        playerAnim = GetComponent<Animator>();
        
        // 체력 초기화
        currentHealth = maxHealth;
    }
    private void Start()
    {
        // 하트 UI 초기화
        if (HeartManager.Instance != null)
        {
            HeartManager.Instance.UpdateHearts(currentHealth);
        }
    }
    private void Update()
    {
        // 좌우 이동 입력
        moveX = Input.GetAxisRaw("Horizontal");

        // 각 기능별 처리 함수 호출
        HandleMovement();
        HandleJump();
        HandleAnimations();
        HandleThrow();
        HandleDirection();

        // 머리 위 블록 충돌 체크
        CheckCeilingHit();
    }
    private void HandleMovement()   // 이동 처리
    {
        // 넉백 상태일 땐 이동 불가
        if (isKnockback) return;

        // Shift 키로 달리기
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        // 속도 적용
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
    }
    private void HandleJump()   // 점프 처리
    {
        CheckGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // 점프
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            Debug.Log("점프 시작");
        }

        // 짧게 점프 시 빠른 낙하
        if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    private void HandleThrow()   // 던지기 애니메이션 처리
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

            Invoke(nameof(ResetThrowBools), 0.5f);      // 일정 시간 후 애니메이션 종료
        }
    }

    private void ResetThrowBools()   // 던지기 애니메이션 상태 초기화
    {
        playerAnim.SetBool("isThrowing", false);
        playerAnim.SetBool("isJumpThrowing", false);
    }
    private void HandleAnimations()     // 애니메이션 상태 설정
    {
        playerAnim.SetBool("isHappyWalk", Input.GetKey(KeyCode.Z));
        playerAnim.SetBool("isWalking", moveX != 0 && !Input.GetKey(KeyCode.LeftShift));
        playerAnim.SetBool("isRunning", moveX != 0 && Input.GetKey(KeyCode.LeftShift));
        playerAnim.SetBool("isJumping", !isGrounded && rb.velocity.y > 0);
        playerAnim.SetBool("isFalling", !isGrounded && rb.velocity.y < 0);
        playerAnim.SetBool("isGrounded", isGrounded);
    }
    private void HandleDirection()      // 방향 전환 처리
    {
        if (moveX != 0)
        {
            // 왼쪽/오른쪽 전환
            transform.localScale = new Vector3(Mathf.Sign(moveX), 1, 1);    
        }
    }
    private void CheckGrounded()        // 바닥에 닿았는지 감지
    {
        Bounds bounds = playerBC.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y - 0.05f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, groundLayer);
       
        isGrounded = hit.collider != null;

        Debug.DrawRay(origin, Vector2.down * 0.1f, isGrounded ? Color.green : Color.red);
    }
    public void PlayerHurt(Vector2 hitDirection, int damage)        // 플레이어가 적에게 맞았을 때 실행
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        // 체력 최소 0 보정
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // 하트 UI 갱신
        heartManager.UpdateHearts(currentHealth); 

        // 넉백 처리
        isKnockback = true;
        // 기존 속도 초기화
        rb.velocity = Vector2.zero;
        // 넉백 방향 + 세기
        rb.AddForce(new Vector2(hitDirection.x * 10f, 2f), ForceMode2D.Impulse); 

        playerAnim.SetTrigger("isHurt");
        Debug.Log($"대미지를 입었다! 남은 체력 : {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }

        // 무적 시간 및 넉백 회복 처리
        StartCoroutine(InvincibilityCo());
        StartCoroutine(EndKnockbackAfterTimeCo(0.3f));
    }
    private IEnumerator InvincibilityCo()       // 무적 상태 유지 코루틴
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
    private IEnumerator EndKnockbackAfterTimeCo(float duration)     // 넉백 상태 해제 코루틴
    {
        yield return new WaitForSeconds(duration);
        isKnockback = false;
    }
    public void Die()       // 플레이어 사망 처리
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
    private void GoToGameOver()     // 게임 오버 씬 전환
    {
        Debug.Log("게임 오버 씬으로 전환합니다");
        SceneManager.LoadScene("GameOver");
    }
    private void OnCollisionEnter2D(Collision2D collision)      // 충돌 처리
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

        // '음식'과 충돌 시 체력 회복
        if (collision.gameObject.CompareTag("Food"))
        {
            if (currentHealth < maxHealth)
            {
                currentHealth++;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                heartManager.UpdateHearts(currentHealth);

                Debug.Log("체력 회복! 현재 체력 : " + currentHealth);
            }
            else
            {
                Debug.Log("이미 최대 체력입니다.");
            }

            Destroy(collision.gameObject);
        }
    }

    private void CheckCeilingHit()      // 머리 위 블록 충돌 체크
    {
        if (rb.velocity.y <= 0 || isGrounded) return;

        // 플레이어 콜라이더 기준으로 위쪽 중앙 위치 계산
        Bounds bounds = playerBC.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.max.y + 0.05f);

        // Raycast로 머리 위 블록 감지
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, 0.1f, LayerMask.GetMask("Block"));
        Debug.DrawRay(origin, Vector2.up * 0.1f, hit.collider ? Color.green : Color.red);

        if (hit.collider != null)
        {
            // 충돌한 순간 점프 중단
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            // 일반 블록일 경우 금 간 블록으로 교체
            if (hit.collider.TryGetComponent(out BreakableBlock normalBlock))
            {
                normalBlock.BreakToCracked();
            }
            // 금 간 블록일 경우 아이템 소환 및 블록 파괴
            else if (hit.collider.TryGetComponent(out CrackedBlock crackedBlock))
            {
                // 블록 흔들림 연출
                crackedBlock.ShakeBlock();
                StartCoroutine(BreakCrackedBlockDelayed(crackedBlock, 0.25f));
            }
        }
    }
    private IEnumerator BreakCrackedBlockDelayed(CrackedBlock block, float delay)       // 금 간 블록 파괴 및 아이템 생성 지연 처리
    {
        yield return new WaitForSeconds(delay);
        if (block != null)
        {
            block.BreakAndSpawnItem();
        }
    }
    

}
