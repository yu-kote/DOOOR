using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotater : MonoBehaviour
{
	// 一度に回転する角度
	private const float _rotateAngle = 360.0f / 4.0f;
	private Vector3 _interestPoint = Vector3.zero;
	private Vector3 _lookAxis = Vector3.forward;
	public Vector3 Lookxis
	{
		get { return _lookAxis; }
		set { _lookAxis = value; }
	}

	[SerializeField]
	private string _leftRotateButton = "L1";
	[SerializeField]
	private string _rightRotateButton = "R1";
	[SerializeField]
	private float _rotateTakeTime = 1.0f;
	private float _time = 0.0f;
	private float _angle = 0.0f;
	private bool _isRotating = false;
	public bool IsRotating
	{
		get { return _isRotating; }
		set { _isRotating = value; }
	}

	void Start()
	{
		// マップの中心点を獲得
		// 未実装

	}

	void Update()
	{
		Rotating();

		if (Input.GetButtonDown(_leftRotateButton))
			StartRotation(_rotateAngle);
		if (Input.GetButtonDown(_rightRotateButton))
			StartRotation(-_rotateAngle);
	}

	void StartRotation(float rotateAngle)
	{
		_isRotating = true;
		_angle = rotateAngle;
		_time = 0.0f;
	}

	void Rotating()
	{
		if (!_isRotating)
			return;

		transform.RotateAround(_interestPoint, Vector3.up, _angle * (Time.deltaTime / _rotateTakeTime));
		_time += Time.deltaTime / _rotateTakeTime;
		if (_time < 1.0f)
			return;

		if(_time > 1.0f)
		{
			float overTime = _time - 1.0f;
			transform.RotateAround(_interestPoint, Vector3.up, -_angle * overTime);
		}

		_isRotating = false;
	}
}
