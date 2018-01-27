using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackGameManager : MonoBehaviour {
    private void Start()
    {
        SignalController[] signals = FindObjectsOfType<SignalController>();
        foreach (SignalController signal in signals)
        {
            StartCoroutine(signal.Move());
        }
    }
}
