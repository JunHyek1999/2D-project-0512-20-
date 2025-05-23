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

    // �÷��̾ ��Ż ������ ���� �ִ��� ����
    private bool isPlayerInRange = false;
   
    void Update()
    {
        // �÷��̾ ���� �ȿ� ������ W �Ǵ� �踦 �Է� ��
        if (isPlayerInRange && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()       // ���̵� �ƿ� �� ���� ��ȯ�ϴ� �ڷ�ƾ
    {
        // �ִϸ����Ͱ� �����Ǿ� ������ ���̵� �ƿ� ����
        if (fadeAnimator != null)
        {
            // �ִϸ����Ϳ��� "FadeOut" Ʈ���Ÿ� �۵�
            fadeAnimator.SetTrigger("FadeOut");

            // ���̵� �ƿ� �ִϸ��̼��� ����� �ð��� ��ٸ�
            yield return new WaitForSeconds(1f); 
        }
        // ������ ���� ������ ��ȯ
        SceneManager.LoadScene(nextSceneName);
    }
    private void OnTriggerEnter2D(Collider2D collision)     // �÷��̾ ��Ż ������ ������ ȣ��
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)      // �÷��̾ ��Ż ������ ����� ȣ��
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
