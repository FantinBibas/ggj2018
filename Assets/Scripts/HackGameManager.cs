using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackGameManager : MonoBehaviour
{
    public Text Strength;
    public Text Frequency;
    public Text Attack;
    public SignalController PlayerSignal;

    private void Start()
    {
        SignalController[] signals = FindObjectsOfType<SignalController>();
        foreach (SignalController signal in signals)
        {
            StartCoroutine(signal.Move());
        }
    }

    private void Update()
    {
        Strength.text = Mathf.CeilToInt(PlayerSignal.ParamStrength * 10).ToString();
        Frequency.text = Mathf.CeilToInt(PlayerSignal.ParamFrequency * 10).ToString();
        Attack.text = Mathf.CeilToInt(PlayerSignal.ParamFourier * 10).ToString();
    }
}
