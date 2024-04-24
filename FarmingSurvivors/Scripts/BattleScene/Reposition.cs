using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 화면 밖으로 나가는 플레이어를 감지하여 해당 방향으로 자기자신을 이동 시키는 컴포넌트
/// TileMap 에 추가할 시 무한 맵 로딩 기능으로 작동하며
/// Enemy 에 추가할 시 플레이어가 향하는 방향에서 재등장 하도록 이동 시킨다.
/// </summary>
// public enum RepositionType
// {   Monster, TileMap    }
public class Reposition : MonoBehaviour
{
    public Vector3 startPos;
    public Transform target;
    Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        startPos = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Area"))
            return;
        
        Vector3 playerPos = other.transform.position;
        Vector3 myPos = transform.position;

        float distX = Mathf.Abs(playerPos.x - myPos.x);
        float distY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 playerDir = (Vector3)JoyStickInput.GetInputDir();
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        if(distX > distY)
            transform.position += Vector3.right * dirX * 40; // 40 : 타일맵 가로길이 * 2
        else if (distX < distY)
            transform.position += Vector3.up * dirY * 40; // 40 : 타일맵 세로길이 * 2

    }
}
