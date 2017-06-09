using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private GameObject _player = null;

    [SerializeField]
    private Vector2 _movedrange = new Vector2(7.5f, 7.5f);

    void Awake()
    {

    }

    void Start()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
            Debug.Log("_player is null");
    }

    void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGameState
            != GameState.GAMEMAIN)
            return;

        if (GetComponent<Rotater>().IsRotating)
            return;

        float y = transform.eulerAngles.y;
        Vector3 playerPos = _player.transform.position;
        Vector3 cameraPos = transform.position;
        Vector3 distance = Vector3.zero;
        //誤差が怖いので幅とった
        if (y > -5.0f && y < 5.0f)
        {
            if (playerPos.x < cameraPos.x - _movedrange.x / 2.0f)
                distance.x = playerPos.x - (cameraPos.x - _movedrange.x / 2.0f);
            if (playerPos.x > cameraPos.x + _movedrange.x / 2.0f)
                distance.x = playerPos.x - (cameraPos.x + _movedrange.x / 2.0f);
            if (playerPos.y < cameraPos.y - _movedrange.y / 2.0f)
                distance.y = playerPos.y - (cameraPos.y - _movedrange.y / 2.0f);
            if (playerPos.y > cameraPos.y + _movedrange.y / 2.0f)
                distance.y = playerPos.y - (cameraPos.y + _movedrange.y / 2.0f);

        }
        else if (y > 85.0f && y < 95.0f)
        {
            if (playerPos.z < cameraPos.z - _movedrange.x / 2.0f)
                distance.z = playerPos.z - (cameraPos.z - _movedrange.x / 2.0f);
            if (playerPos.z > cameraPos.z + _movedrange.x / 2.0f)
                distance.z = playerPos.z - (cameraPos.z + _movedrange.x / 2.0f);
            if (playerPos.y < cameraPos.y - _movedrange.y / 2.0f)
                distance.y = playerPos.y - (cameraPos.y - _movedrange.y / 2.0f);
            if (playerPos.y > cameraPos.y + _movedrange.y / 2.0f)
                distance.y = playerPos.y - (cameraPos.y + _movedrange.y / 2.0f);
        }
        else if (y > 175.0f && y < 195.0f)
        {
            if (playerPos.x < cameraPos.x - _movedrange.x / 2.0f)
                distance.x = playerPos.x - (cameraPos.x - _movedrange.x / 2.0f);
            if (playerPos.x > cameraPos.x + _movedrange.x / 2.0f)
                distance.x = playerPos.x - (cameraPos.x + _movedrange.x / 2.0f);
            if (playerPos.y < cameraPos.y - _movedrange.y / 2.0f)
                distance.y = playerPos.y - (cameraPos.y - _movedrange.y / 2.0f);
            if (playerPos.y > cameraPos.y + _movedrange.y / 2.0f)
                distance.y = playerPos.y - (cameraPos.y + _movedrange.y / 2.0f);
        }
        else if (y > 265.0f && y < 275.0f)
        {
            if (playerPos.z < cameraPos.z - _movedrange.x / 2.0f)
                distance.z = playerPos.z - (cameraPos.z - _movedrange.x / 2.0f);
            if (playerPos.z > cameraPos.z + _movedrange.x / 2.0f)
                distance.z = playerPos.z - (cameraPos.z + _movedrange.x / 2.0f);
            if (playerPos.y < cameraPos.y - _movedrange.y / 2.0f)
                distance.y = playerPos.y - (cameraPos.y - _movedrange.y / 2.0f);
            if (playerPos.y > cameraPos.y + _movedrange.y / 2.0f)
                distance.y = playerPos.y - (cameraPos.y + _movedrange.y / 2.0f);
        }

        transform.position += distance;

        //マップの端を超えた場合戻す処理が必要
        //未実装
    }
}
