using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeartManager : MonoBehaviour
{
    [Header("��Ʈ ������")]
    public GameObject heartPrefab;

    [Header("��Ʈ�� ������ �θ� ������Ʈ (UI)")]
    public Transform heartsContainer;

    // ���� ȭ�鿡 �ִ� ��Ʈ ���
    private List<GameObject> hearts = new List<GameObject>();

    // �̱��� �ν��Ͻ�
    public static HeartManager Instance;

    private void Awake()
    {
        // �̱��� ����: �ϳ��� �����ϵ���
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �̹� �����ϴ� �ν��Ͻ��� �ִٸ� �̰� ����
        }
    }

    private void OnEnable()
    {
        // ���� �ε�� �� OnSceneLoaded �Լ� ���
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // ���� ��ε�� �� �̺�Ʈ ���� (�޸� ���� ����)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ���� ���� �ε�� �� �ڵ� ȣ��Ǵ� �Լ�
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �±׷� "HeartContainer" ������Ʈ�� ã�� heartsContainer�� ������
        GameObject found = GameObject.FindWithTag("HeartContainer");
        
        if (found != null)
        {
            heartsContainer = found.transform;

            // ���� ��Ʈ �� �������� �ٽ� �׸���
            UpdateHearts(hearts.Count);     
        }
        else
        {
            Debug.LogWarning("HeartContainer �±׸� ���� ������Ʈ�� ã�� ���߽��ϴ�.");
        }
    }

    /// <summary>
    /// ��Ʈ UI�� ���� ü�¿� ���� ����
    /// </summary>
    /// <param name="currentHealth" > ���� �÷��̾��� ü�� </param>
    public void UpdateHearts(int currentHealth)
    {
        Debug.Log("��Ʈ ���� ����: " + currentHealth);

        // ���� ��Ʈ ������Ʈ���� ��� ����
        foreach (var heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();     // ����Ʈ�� �ʱ�ȭ

        // ���� ü�� ����ŭ ��Ʈ�� ���� �����Ͽ� heartsContainer�� ��ġ
        for (int i = 0; i < currentHealth; i++)
        {
            if (heartPrefab != null && heartsContainer != null)
            {
                GameObject heart = Instantiate(heartPrefab, heartsContainer);
                hearts.Add(heart);  // ����Ʈ�� �߰�
            }
            else
            {
                Debug.LogWarning("��Ʈ ������ �Ǵ� ��Ʈ �����̳ʰ� ��� �ֽ��ϴ�.");
            }
        }
    }
}
