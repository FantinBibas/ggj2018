using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SignalController : MonoBehaviour
{
    public TrailRenderer TrailA;
    public TrailRenderer TrailB;
    public bool IsEditable = true;
    [Header("Strength (height of the curve)")]
    public float ParamStrength;
    private const double StrengthStep = 0.1;
    private const double MinStrength = 0.1;
    private const double MaxStrength = 2;
    [Header("Frequency (width)")]
    public float ParamFrequency;
    private const double FrequencyStep = 0.05;
    private const double MinFrequency = 0.1;
    private const double MaxFrequency = 0.8;
    [Header("Fourier (sharpness, between simple sin to sawtooth)")]
    public float ParamFourier;
    private const double FourierStep = 1;
    private const double MinFourier = 1;
    private const double MaxFourier = 7;
    [Header("Other parameters")]
    public float MinXLimit;
    public float MaxYLimit;
    public float YOffset;
    public float Speed;

    private float _currentPosition = 0;
    private TrailRenderer _tempTrail;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(MinXLimit, YOffset, 0), new Vector3(MaxYLimit, YOffset, 0));
    }

    private float Fourier(float x)
    {
        float total = 0;
        for (int i = 1; i < ParamFourier + 1; i++)
        {
            float tmp = Mathf.Sin(2 * Mathf.PI * i * x) / i;
            if (i % 2 == 1)
                tmp *= -1;
            total += tmp;
        }

        return total * (2 / Mathf.PI);
    }
    
    private float GetY()
    {
        return ParamStrength * Fourier(_currentPosition * ParamFrequency) + YOffset;
    }

    private bool increasePosition()
    {
        _currentPosition += Speed;
        if (!(_currentPosition >= MaxYLimit)) return false;
        _currentPosition = MinXLimit;
        _tempTrail = TrailA;
        TrailA = TrailB;
        TrailB = _tempTrail;
        return true;

    }

    public void IncrementStrength()
    {
        if (IsEditable && ParamStrength < MaxStrength)
            ParamStrength += (float) StrengthStep;
    }

    public void DecrementStrength()
    {
        if (IsEditable && ParamStrength > MinStrength)
            ParamStrength -= (float) StrengthStep;
    }

    public void IncrementFrequency()
    {
        if (IsEditable && ParamFrequency < MaxFrequency)
            ParamFrequency += (float) FrequencyStep;
    }

    public void DecrementFrequency()
    {
        if (IsEditable && ParamFrequency > MinFrequency)
            ParamFrequency -= (float) FrequencyStep;
    }

    public void IncrementFourier()
    {
        if (IsEditable && ParamFourier < MaxFourier)
            ParamFourier += (float) FourierStep;
    }

    public void DecrementFourier()
    {
        if (IsEditable && ParamFourier > MinFourier)
            ParamFourier -= (float) FourierStep;
    }

    public void RandomizeSignal()
    {
        ParamStrength = (float) Random.Range(0, (int) (MaxStrength - MinStrength) * 10) / 10 + (float) MinStrength;
        ParamFrequency = (float) Random.Range(0, (int) (MaxFrequency - MinFrequency) * 100) / 100 + (float) MinFrequency;
        ParamFourier = Random.Range((int) MinFourier, (int) MaxFourier);
    }

    public IEnumerator Move()
    {
        _currentPosition = MinXLimit;
        while (true)
        {
            increasePosition();
            transform.position = new Vector3(_currentPosition, GetY(), 0);
            TrailA.transform.position = transform.position;
            yield return new WaitForEndOfFrame();
        }
    }
}
