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
		float horizotal = Input.GetAxis("Horizotal");
		float vertical = Input.GetAxis("Vecrtical");
		float deltaTime = Time.deltaTime;

		Vector3 movingAmount = new Vector3(
			_moveSpeed.x * horizotal,
			_moveSpeed.y * vertical,
			0.0f) * deltaTime;

		transform.position += movingAmount;

		// ステージないから出れないようにする処理
		// 未実装
	}
}
