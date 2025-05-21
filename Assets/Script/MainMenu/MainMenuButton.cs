using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    [Header("��ư ���۷���")]
    public Button startButton;
    public Button quitButton;

    // ���õ� ��ư �ε��� (0 : ���� , 1: ����)
    private int selectedIndex = 0;

    void Start()
    {
        // ���콺 Ŭ�� ����
        startButton.onClick.AddListener(StartClicked);   // ���� ����
        quitButton.onClick.AddListener(QuitClicked);
    }

    void Update()
    {
        // Ű���� �� Ű �Է� (W or ��)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);

        }

        // Ű���� �Ʒ� Ű �Է� (S or ��)
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = Mathf.Min(1, selectedIndex + 1);
        }

        // Enter Ű �Է����� ��ư ����
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedIndex == 0)
            {
                StartClicked();
            }
            else
            {
                QuitClicked();
            }
        }
    }

    void StartClicked()
    {
        SceneManager.LoadScene("Stage1");
        Debug.Log("���� ����");
    }

    void QuitClicked()
    {
        Application.Quit();
        Debug.Log("���� ����");
    }
}
