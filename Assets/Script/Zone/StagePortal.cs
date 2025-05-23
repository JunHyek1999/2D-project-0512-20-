using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StagePortal : MonoBehaviour
{
    [Header("다음 스테이지 이름")]
    public string nextSceneName;

    [Header("페이드 아웃 애니메이터")]
    public Animator fadeAnimator;

    // 플레이어가 포탈 범위에 들어와 있는지 여부
    private bool isPlayerInRange = false;
   
    void Update()
    {
        // 플레이어가 범위 안에 있으며 W 또는 ↑를 입력 시
        if (isPlayerInRange && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()       // 페이드 아웃 후 씬을 전환하는 코루틴
    {
        // 애니메이터가 설정되어 있으면 페이드 아웃 실행
        if (fadeAnimator != null)
        {
            // 애니메이터에서 "FadeOut" 트리거를 작동
            fadeAnimator.SetTrigger("FadeOut");

            // 페이드 아웃 애니메이션이 재생될 시간을 기다림
            yield return new WaitForSeconds(1f); 
        }
        // 설정한 다음 씬으로 전환
        SceneManager.LoadScene(nextSceneName);
    }
    private void OnTriggerEnter2D(Collider2D collision)     // 플레이어가 포탈 범위에 들어오면 호출
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)      // 플레이어가 포탈 범위를 벗어나면 호출
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
