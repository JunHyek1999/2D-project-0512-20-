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

    private bool isPlayerInRange = false;
   
    void Update()
    {
        // 플레이어가 범위 안에 있으며 W 또는 ↑를 입력 시
        if (isPlayerInRange && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()
    {
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(1f); 
        }

        SceneManager.LoadScene(nextSceneName);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
