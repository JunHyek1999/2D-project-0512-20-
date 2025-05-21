using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    [Header("버튼 레퍼런스")]
    public Button startButton;
    public Button quitButton;

    // 선택된 버튼 인덱스 (0 : 시작 , 1: 종료)
    private int selectedIndex = 0;

    void Start()
    {
        // 마우스 클릭 연결
        startButton.onClick.AddListener(StartClicked);   // 게임 시작
        quitButton.onClick.AddListener(QuitClicked);
    }

    void Update()
    {
        // 키보드 위 키 입력 (W or ↑)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);

        }

        // 키보드 아래 키 입력 (S or ↓)
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = Mathf.Min(1, selectedIndex + 1);
        }

        // Enter 키 입력으로 버튼 실행
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
        Debug.Log("게임 시작");
    }

    void QuitClicked()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }
}
