using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    // 移動量
    [SerializeField]
    private Vector2 _moveSpeed = Vector2.one;
    public Vector2 MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    private Transform _cameraTrans = null;

    private Vector3 _movingAmount;
    public Vector3 MovingAmount
    {
        get { return _movingAmount; }
        set { _movingAmount = value; }
    }


    void Awake()
    {

    }

    void Start()
    {
        _cameraTrans = GameObject.Find("MainCamera").transform;
        if (_cameraTrans == null)
            Debug.Log("_cameraTrans null!");
    }

    void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGameState
            != GameState.GAMEMAIN)
            return;

        if (GetComponent<Rotater>().IsRotating)
            return;

        // 軸の傾きを獲得
        float horizotal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical") * -1.0f;
        // 経過時間を獲得
        float deltaTime = Time.deltaTime;
        // カメラからY軸の回転量を獲得
        float cameraRotateYValue = _cameraTrans.eulerAngles.y;

        // カメラが見ている方向に対して移動軸を変更しないといけない
        _movingAmount = new Vector3(
            _moveSpeed.x * horizotal * Mathf.Cos(cameraRotateYValue * Mathf.Deg2Rad),
            _moveSpeed.y * vertical,
            _moveSpeed.x * -horizotal * Mathf.Sin(cameraRotateYValue * Mathf.Deg2Rad))
            * deltaTime;

        transform.position += _movingAmount;

        // ステージないから出れないようにする処理
        // 未実装
    }
}