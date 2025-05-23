using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedBlock : MonoBehaviour
{
    [Header("�ı� �� ������ ���ľ����� ������")]
    public GameObject[] foodItemPrefabs;        // ���� ���� �����յ��� �迭�� ����

    [Header("������ ��ȯ ��ġ ������")]
    public Vector2 spawnOffset = new Vector2(0, 1f); // ���� ��� ��ġ ���� Y������ 1��ŭ ��

    [Header("���ľ����� ���� Ȯ��")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;    // 50% Ȯ���� ���� ����

    private bool isUsed = false;    // �ߺ� ���� ����

    // �÷��̾�� �浹���� �� ȣ��
    public void BreakAndSpawnItem()
    {
        if (isUsed) return;     // �̹� ���� ����̶�� �� �̻� ó������ ����
        isUsed = true;          // ��� ó��

        // ���� ������ ��ȯ
        SpawnRandomItem();

        // ��� �ı�
        Destroy(gameObject);
    }

    // ���� �������� �����ϰ� ����
    private void SpawnRandomItem()
    {
        // ���� ������ �������� �ϳ��� ������ ����
        if (foodItemPrefabs.Length == 0) return;

        // ������ ���� (0.0f ~ 1.0f)
        float rand = Random.value;

        // Ȯ�� ���� �� ������ ���� �� ��
        if (rand >  spawnChance)
        {
            Debug.Log("������ ���� ���� (Ȯ�� �̴�)");
            return;
        }

        // Ȯ�� ���� �� ���� ������ �� �ϳ��� �������� ����
        int randomIndex = Random.Range(0, foodItemPrefabs.Length);
        GameObject selectedItem = foodItemPrefabs[randomIndex];

        // ���� ��ġ ���
        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

        // ���� ������ �������� �ش� ��ġ�� ����
        Instantiate(selectedItem, spawnPos, Quaternion.identity);
        Debug.Log("50% Ȯ�� �������� ������ �����߽��ϴ�!");
    }

    // ����� ��鸮�� ����
    public void ShakeBlock()
    {
        StartCoroutine(ShakeAnimation());
    }

    // ����� �ణ ���Ʒ��� ��鸮�� �ִϸ��̼�
    private IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.position;    // ���� ��ġ ����

        float duration = 0.2f;          // �ִϸ��̼� ��� �ð�
        float magnitude = 0.05f;        // ��鸲 ����
        float elapsed = 0f;             // ��� �ð�


        while (elapsed < duration)
        {
            // X, Y ������ �ణ �����ϰ� ��鸮�� ����
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // ��ġ ����
            transform.position = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ������ ���� ��ġ�� ����
        transform.position = originalPos;
    }
}
