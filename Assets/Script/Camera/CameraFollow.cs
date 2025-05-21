using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("카메라 오프셋")]
    public Vector2 offset = new Vector2(-2f, 0f);

    public Transform target;        // 따라갈 대상 (플레이어)
    public Vector2 minBounds;       // 카메라 이동 최소 x,y
    public Vector2 maxBounds;
    public float smoothTime = 0.2f;       // 부드럽게 따라가는 속도

    private Vector3 velocity = Vector3.zero;


    void Update()
    {
        if (target == null) return;

        // 오프셋을 포함한 타겟의 위치 계산
        Vector3 targetPos = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
            );

        // 부드럽게 이동
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // 경계 제한
        float clampX = Mathf.Clamp(smoothPos.x, minBounds.x, maxBounds.x);
        float clampY = Mathf.Clamp(smoothPos.y, minBounds.y, maxBounds.y);

        transform.position = new Vector3(clampX, clampY, smoothPos.z);
    }
}
