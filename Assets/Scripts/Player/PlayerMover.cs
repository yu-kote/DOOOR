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

    private GameObject _camera = null;

    private Vector3 _movingAmount;
    public Vector3 MovingAmount
    {
        get { return _movingAmount; }
        set { _movingAmount = value; }
    }

    private NodeManager _nodeManager;


    void Awake()
    {

    }

    void Start()
    {
        _camera = GameObject.Find("MainCamera");
        if (_camera == null)
            Debug.Log("_cameraTrans null!");
        _nodeManager = GameObject.Find("Field").GetComponent<NodeManager>();
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
        float cameraRotateYValue = _camera.transform.eulerAngles.y;

        // カメラが見ている方向に対して移動軸を変更しないといけない
        _movingAmount = new Vector3(
            _moveSpeed.x * horizotal * Mathf.Cos(cameraRotateYValue * Mathf.Deg2Rad),
            _moveSpeed.y * vertical,
            _moveSpeed.x * -horizotal * Mathf.Sin(cameraRotateYValue * Mathf.Deg2Rad))
            * deltaTime;

        transform.position += _movingAmount;

        var height_limit = _nodeManager.HeightLimit();
        if (transform.position.y > _nodeManager.HeightLimit())
            transform.position = new Vector3(transform.position.x, height_limit, transform.position.z);
        if (transform.position.y < 0)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        // ステージないから出れないようにする処理

        var camera_rotater = _camera.GetComponent<Rotater>();
        var rotater = GetComponent<Rotater>();
        var angle = rotater.RotateAngle;

        // 面の長さ
        var surface_length = _nodeManager.SurfaceNodeNum() * _nodeManager.Interval;

        var pos = transform.position;
        if (rotater.CurrentSide == 0)
        {
            if (pos.x < 0)
            {
                rotater.StartRotation(new Vector3(0, 0, 0), angle);
                camera_rotater.StartRotation(new Vector3(0, 0, 0), angle);
            }
            if (pos.x > surface_length)
            {
                rotater.StartRotation(new Vector3(surface_length, 0, 0), -angle);
                camera_rotater.StartRotation(new Vector3(surface_length, 0, 0), -angle);
            }
        }
        if (rotater.CurrentSide == 1)
        {
            if (pos.z < 0)
            {
                rotater.StartRotation(new Vector3(surface_length, 0, 0), angle);
                camera_rotater.StartRotation(new Vector3(surface_length, 0, 0), angle);
            }
            if (pos.z > surface_length)
            {
                rotater.StartRotation(new Vector3(surface_length, 0, surface_length), -angle);
                camera_rotater.StartRotation(new Vector3(surface_length, 0, surface_length), -angle);
            }
        }
        if (rotater.CurrentSide == 2)
        {
            if (pos.x < 0)
            {
                rotater.StartRotation(new Vector3(0, 0, surface_length), -angle);
                camera_rotater.StartRotation(new Vector3(0, 0, surface_length), -angle);
            }
            if (pos.x > surface_length)
            {
                rotater.StartRotation(new Vector3(surface_length, 0, surface_length), angle);
                camera_rotater.StartRotation(new Vector3(surface_length, 0, surface_length), angle);
            }
        }
        if (rotater.CurrentSide == 3)
        {
            if (pos.z < 0)
            {
                rotater.StartRotation(new Vector3(0, 0, 0), -angle);
                camera_rotater.StartRotation(new Vector3(0, 0, 0), -angle);
            }
            if (pos.z > surface_length)
            {
                rotater.StartRotation(new Vector3(0, 0, surface_length), angle);
                camera_rotater.StartRotation(new Vector3(0, 0, surface_length), angle);
            }
        }
    }
}