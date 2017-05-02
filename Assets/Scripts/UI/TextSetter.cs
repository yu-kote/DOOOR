using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSetter : MonoBehaviour
{
	private Text _text = null;

	void Awake()
	{
		_text = GetComponent<Text>();
	}

	public void SetText(string text)
	{
		_text.text = text;
	}
}
