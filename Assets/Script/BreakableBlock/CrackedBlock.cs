using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedBlock : MonoBehaviour
{
    [Header("파괴 시 등장할 음식아이템 프리팹")]
    public GameObject[] foodItemPrefabs;        // 여러 음식 프리팹들을 배열로 받음

    [Header("아이템 소환 위치 오프셋")]
    public Vector2 spawnOffset = new Vector2(0, 1f); // 현재 블록 위치 기준 Y축으로 1만큼 위

    [Header("음식아이템 등장 확률")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;    // 50% 확률로 음식 등장

    private bool isUsed = false;    // 중복 생성 방지

    // 플레이어와 충돌했을 때 호출
    public void BreakAndSpawnItem()
    {
        if (isUsed) return;     // 이미 사용된 블록이라면 더 이상 처리하지 않음
        isUsed = true;          // 사용 처리

        // 랜덤 아이템 소환
        SpawnRandomItem();

        // 블록 파괴
        Destroy(gameObject);
    }

    // 음식 아이템을 랜덤하게 생성
    private void SpawnRandomItem()
    {
        // 음식 아이템 프리팹이 하나도 없으면 리턴
        if (foodItemPrefabs.Length == 0) return;

        // 랜덤값 생성 (0.0f ~ 1.0f)
        float rand = Random.value;

        // 확률 실패 → 아이템 생성 안 함
        if (rand >  spawnChance)
        {
            Debug.Log("아이템 생성 실패 (확률 미달)");
            return;
        }

        // 확률 성공 → 음식 아이템 중 하나를 랜덤으로 선택
        int randomIndex = Random.Range(0, foodItemPrefabs.Length);
        GameObject selectedItem = foodItemPrefabs[randomIndex];

        // 생성 위치 계산
        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

        // 음식 아이템 프리팹을 해당 위치에 생성
        Instantiate(selectedItem, spawnPos, Quaternion.identity);
        Debug.Log("50% 확률 성공으로 음식이 등장했습니다!");
    }

    // 블록이 흔들리는 연출
    public void ShakeBlock()
    {
        StartCoroutine(ShakeAnimation());
    }

    // 블록이 약간 위아래로 흔들리는 애니메이션
    private IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.position;    // 원래 위치 저장

        float duration = 0.2f;          // 애니메이션 재생 시간
        float magnitude = 0.05f;        // 흔들림 강도
        float elapsed = 0f;             // 경과 시간


        while (elapsed < duration)
        {
            // X, Y 축으로 약간 랜덤하게 흔들리게 만듦
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // 위치 흔들기
            transform.position = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 끝나면 원래 위치로 복구
        transform.position = originalPos;
    }
}
