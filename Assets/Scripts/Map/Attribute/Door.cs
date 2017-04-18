﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : AttributeBase
{
	public enum DoorStatus
	{
		OPEN,
		CLOSE
	}

	public DoorStatus _doorStatus = DoorStatus.CLOSE;
	private Animator anim = null;
	private float _statusLockTime = 0.0f;
	public float StatusLockTime
	{
		get { return _statusLockTime; }
		set { _statusLockTime = value; }
	}

	void Awake()
	{

	}

	void Start()
	{
		CreateAttribute("Door");
		anim = _attribute.transform.GetChild(0).GetComponent<Animator>();
	}

	void Update()
	{
		_statusLockTime = Mathf.Max(0.0f, _statusLockTime - Time.deltaTime);

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("OpenIdol") &&
			anim.GetBool("IsOpen"))
			anim.SetBool("IsOpen", false);

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("CloseIdol") &&
			anim.GetBool("IsClose"))
			anim.SetBool("IsClose", false);
	}

	public bool StartOpening()
	{
		if (_statusLockTime > 0.0f)
			return false;
		if (_doorStatus != DoorStatus.CLOSE)
			return false;
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("CloseIdol"))
			return false;

		_doorStatus = DoorStatus.OPEN;
		anim.SetBool("IsOpen", true);

		return true;
	}

	public bool StartClosing()
	{
		if (_statusLockTime > 0.0f)
			return false;
		if (_doorStatus != DoorStatus.OPEN)
			return false;
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("OpenIdol"))
			return false;

		_doorStatus = DoorStatus.CLOSE;
		anim.SetBool("IsClose", true);

		return true;
	}

	public void LockDoorStatus(float statusLockTime)
	{
		if (_statusLockTime > 0.0f)
			return;

		_statusLockTime = statusLockTime;
	}
}