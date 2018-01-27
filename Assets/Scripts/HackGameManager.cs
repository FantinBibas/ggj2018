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
    public SignalController OriginalSignal;

    private SignalController[] _signals;

    private void Start()
    {
        _signals = FindObjectsOfType<SignalController>();
        foreach (SignalController signal in _signals)
        {
            StartCoroutine(signal.Move());
        }
    }

    private bool areSettingsOk()
    {
        return Mathf.RoundToInt(PlayerSignal.ParamFourier) == Mathf.RoundToInt(OriginalSignal.ParamFourier)
               && Mathf.RoundToInt(PlayerSignal.ParamFrequency * 100) == Mathf.RoundToInt(OriginalSignal.ParamFrequency * 100)
               && Mathf.RoundToInt(PlayerSignal.ParamStrength * 10) == Mathf.RoundToInt(OriginalSignal.ParamStrength * 10);
    }

    private void Update()
    {
        Strength.text = Mathf.RoundToInt(PlayerSignal.ParamStrength * 10).ToString();
        Frequency.text = Mathf.RoundToInt(PlayerSignal.ParamFrequency * 100).ToString();
        Attack.text = PlayerSignal.ParamFourier.ToString();
        if (!areSettingsOk()) return;
        PlayerSignal.IsEditable = false;
        GameObject.FindGameObjectWithTag("WinMsg").GetComponent<Text>().enabled = true;
    }
}
