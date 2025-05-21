using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedBlock : MonoBehaviour
{
    [Header("파괴 시 등장할 음식아이템 프리팹")]
    public GameObject[] foodItemPrefabs; // FoodItem01~07 의 프리팹 적용

    [Header("아이템 소환 위치 오프셋")]
    public Vector2 spawnOffset = new Vector2(0, 1f); // 블록 위로 소환

    [Header("음식아이템 등장 확률")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;    // 50% 확률로 음식 등장

    // 중복 생성 방지
    private bool isUsed = false;

    // 플레이어와 충돌했을 때 호출
    public void BreakAndSpawnItem()
    {
        if (isUsed) return;
        isUsed = true;

        // 랜덤 아이템 소환
        SpawnRandomItem();

        // 블록 파괴
        Destroy(gameObject);
    }
    private void SpawnRandomItem()
    {
        if (foodItemPrefabs.Length == 0) return;

        // 확률 체크
        float rand = Random.value; // 0~1 사이 랜덤값
        if (rand >  spawnChance)
        {
            Debug.Log("50% 확률 실패로 음식이 등장하지 않았습니다~");
            return;
        }

        // 음식 생성
        int randomIndex = Random.Range(0, foodItemPrefabs.Length);
        GameObject selectedItem = foodItemPrefabs[randomIndex];

        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

        Instantiate(selectedItem, spawnPos, Quaternion.identity);
        Debug.Log("50% 확률 성공으로 음식이 등장했습니다!");
    }
    public void ShakeBlock()
    {
        StartCoroutine(ShakeAnimation());
    }

    // 블록이 살짝 위로 튕겼다가 제자리로 돌아오는 코루틴
    private IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.position;    // 원래 위치 저장

        float duration = 0.2f;          // 진동 지속 시간
        float magnitude = 0.05f;        // 진동 강도
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
