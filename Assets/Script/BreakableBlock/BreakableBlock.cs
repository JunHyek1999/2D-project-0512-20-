using UnityEngine;
using UnityEngine.UIElements;

public class BreakableBlock : MonoBehaviour
{
    [Header("금 간 블록 프리팹")]
    public GameObject crackedBlockPrefab;

    // 플레이어가 부딪히면 이 메서드가 호출
    public void BreakToCracked()
    {
        // 같은 위치에 CrackedBlock 프리팹 생성
        Instantiate(crackedBlockPrefab, transform.position, Quaternion.identity);

        // 원래 블록 파괴
        Destroy(gameObject);
    }
}
