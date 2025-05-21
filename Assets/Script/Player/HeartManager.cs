using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeartManager : MonoBehaviour
{
    [Header("하트 프리팹")]
    public GameObject heartPrefab;

    [Header("하트가 생성될 부모 오브젝트 (UI)")]
    public Transform heartsContainer;

    // 현재 화면에 있는 하트 목록
    private List<GameObject> hearts = new List<GameObject>();

    // 싱글턴 인스턴스
    public static HeartManager Instance;

    private void Awake()
    {
        // 싱글턴 설정: 하나만 존재하도록
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 인스턴스가 있다면 이건 삭제
        }
    }

    private void OnEnable()
    {
        // 씬이 로드될 때 OnSceneLoaded 함수 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬이 언로드될 때 이벤트 제거 (메모리 누수 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 새로 로드될 때 자동 호출되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 태그로 "HeartContainer" 오브젝트를 찾아 heartsContainer에 재지정
        GameObject found = GameObject.FindWithTag("HeartContainer");
        
        if (found != null)
        {
            heartsContainer = found.transform;

            // 현재 하트 수 기준으로 다시 그리기
            UpdateHearts(hearts.Count);     
        }
        else
        {
            Debug.LogWarning("HeartContainer 태그를 가진 오브젝트를 찾지 못했습니다.");
        }
    }

    /// <summary>
    /// 하트 UI를 현재 체력에 맞춰 갱신
    /// </summary>
    /// <param name="currentHealth" > 현재 플레이어의 체력 </param>
    public void UpdateHearts(int currentHealth)
    {
        Debug.Log("하트 개수 갱신: " + currentHealth);

        // 기존 하트 오브젝트들을 모두 삭제
        foreach (var heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();     // 리스트도 초기화

        // 현재 체력 수만큼 하트를 새로 생성하여 heartsContainer에 배치
        for (int i = 0; i < currentHealth; i++)
        {
            if (heartPrefab != null && heartsContainer != null)
            {
                GameObject heart = Instantiate(heartPrefab, heartsContainer);
                hearts.Add(heart);  // 리스트에 추가
            }
            else
            {
                Debug.LogWarning("하트 프리팹 또는 하트 컨테이너가 비어 있습니다.");
            }
        }
    }
}
