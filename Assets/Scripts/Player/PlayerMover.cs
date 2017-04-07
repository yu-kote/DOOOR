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
	[SerializeField]
	private Transform _cameraTrans = null;

	void Awake()
	{

	}

	void Start()
	{
		if (_cameraTrans == null)
			Debug.Log("_cameraTrans null!");
	}

	void Update()
	{
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
		// 未実装
		Vector3 movingAmount = new Vector3(
			_moveSpeed.x * horizotal * Mathf.Cos(cameraRotateYValue * Mathf.Deg2Rad),
			_moveSpeed.y * vertical,
			_moveSpeed.x * -horizotal * Mathf.Sin(cameraRotateYValue * Mathf.Deg2Rad))
			* deltaTime;

		Debug.Log(cameraRotateYValue);

		transform.position += movingAmount;

		// ステージないから出れないようにする処理
		// 未実装
	}
}