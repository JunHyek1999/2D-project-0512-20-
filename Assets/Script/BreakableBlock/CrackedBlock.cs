using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedBlock : MonoBehaviour
{
    [Header("�ı� �� ������ ���ľ����� ������")]
    public GameObject[] foodItemPrefabs; // FoodItem01~07 �� ������ ����

    [Header("������ ��ȯ ��ġ ������")]
    public Vector2 spawnOffset = new Vector2(0, 1f); // ��� ���� ��ȯ

    [Header("���ľ����� ���� Ȯ��")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;    // 50% Ȯ���� ���� ����

    // �ߺ� ���� ����
    private bool isUsed = false;

    // �÷��̾�� �浹���� �� ȣ��
    public void BreakAndSpawnItem()
    {
        if (isUsed) return;
        isUsed = true;

        // ���� ������ ��ȯ
        SpawnRandomItem();

        // ��� �ı�
        Destroy(gameObject);
    }
    private void SpawnRandomItem()
    {
        if (foodItemPrefabs.Length == 0) return;

        // Ȯ�� üũ
        float rand = Random.value; // 0~1 ���� ������
        if (rand >  spawnChance)
        {
            Debug.Log("50% Ȯ�� ���з� ������ �������� �ʾҽ��ϴ�~");
            return;
        }

        // ���� ����
        int randomIndex = Random.Range(0, foodItemPrefabs.Length);
        GameObject selectedItem = foodItemPrefabs[randomIndex];

        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

        Instantiate(selectedItem, spawnPos, Quaternion.identity);
        Debug.Log("50% Ȯ�� �������� ������ �����߽��ϴ�!");
    }
    public void ShakeBlock()
    {
        StartCoroutine(ShakeAnimation());
    }

    // ����� ��¦ ���� ƨ��ٰ� ���ڸ��� ���ƿ��� �ڷ�ƾ
    private IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.position;    // ���� ��ġ ����

        float duration = 0.2f;          // ���� ���� �ð�
        float magnitude = 0.05f;        // ���� ����
        float elapsed = 0f;


        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            
            transform.position = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}
