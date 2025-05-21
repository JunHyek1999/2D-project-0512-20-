using UnityEngine;
using UnityEngine.UIElements;

public class BreakableBlock : MonoBehaviour
{
    [Header("�� �� ��� ������")]
    public GameObject crackedBlockPrefab;

    // �÷��̾ �ε����� �� �޼��尡 ȣ��
    public void BreakToCracked()
    {
        // ���� ��ġ�� CrackedBlock ������ ����
        Instantiate(crackedBlockPrefab, transform.position, Quaternion.identity);

        // ���� ��� �ı�
        Destroy(gameObject);
    }
}
