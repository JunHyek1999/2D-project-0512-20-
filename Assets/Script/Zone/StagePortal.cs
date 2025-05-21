using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StagePortal : MonoBehaviour
{
    [Header("���� �������� �̸�")]
    public string nextSceneName;

    [Header("���̵� �ƿ� �ִϸ�����")]
    public Animator fadeAnimator;

    private bool isPlayerInRange = false;
   
    void Update()
    {
        // �÷��̾ ���� �ȿ� ������ W �Ǵ� �踦 �Է� ��
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
