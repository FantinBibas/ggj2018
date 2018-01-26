using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicButton : MonoBehaviour {
	public bool click()
	{
		return toPress.onClick();
	}

	public BasicActivable toPress;
}