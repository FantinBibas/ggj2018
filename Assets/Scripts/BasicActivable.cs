using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicActivable : MonoBehaviour {
	public bool onClick()
	{
		status = !status;
		return true;
	}

	public bool status;
}
