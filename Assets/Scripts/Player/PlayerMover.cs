using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{

	[SerializeField]
	private Vector2 _moveSpeed = Vector2.one;
	public Vector2 MoveSpeed {

		get { return _moveSpeed; }
		set { _moveSpeed = value; }
	}

	void Awake()
	{

	}

	void Start()
	{

	}

	void Update()
	{

		// 軸の傾きを獲得
		float horizotal = Input.GetAxis("Horizotal");
		float vertical = Input.GetAxis("Vecrtical");
		// 経過時間を獲得
		float deltaTime = Time.deltaTime;

		// カメラが見ている方向に対して移動軸を変更しないといけない
		// 未実装
		Vector3 movingAmount = new Vector3(
			_moveSpeed.x * horizotal,
			_moveSpeed.y * vertical,
			_moveSpeed.x) * deltaTime;

		transform.position += movingAmount;

		// ステージないから出れないようにする処理
		// 未実装
	}
}
