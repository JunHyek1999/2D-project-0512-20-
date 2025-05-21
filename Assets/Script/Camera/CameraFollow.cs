using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("ī�޶� ������")]
    public Vector2 offset = new Vector2(-2f, 0f);

    public Transform target;        // ���� ��� (�÷��̾�)
    public Vector2 minBounds;       // ī�޶� �̵� �ּ� x,y
    public Vector2 maxBounds;
    public float smoothTime = 0.2f;       // �ε巴�� ���󰡴� �ӵ�

    private Vector3 velocity = Vector3.zero;


    void Update()
    {
        if (target == null) return;

        // �������� ������ Ÿ���� ��ġ ���
        Vector3 targetPos = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
            );

        // �ε巴�� �̵�
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // ��� ����
        float clampX = Mathf.Clamp(smoothPos.x, minBounds.x, maxBounds.x);
        float clampY = Mathf.Clamp(smoothPos.y, minBounds.y, maxBounds.y);

        transform.position = new Vector3(clampX, clampY, smoothPos.z);
    }
}
