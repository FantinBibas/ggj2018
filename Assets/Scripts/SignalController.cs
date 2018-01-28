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
    private const double STRENGTH_STEP = 0.1;
    private const double MIN_STRENGTH = 1;
    private const double MAX_STRENGTH = 4.2;
    [Header("Frequency (width)")]
    public float ParamFrequency;
    private const double FREQUENCY_STEP = 0.05;
    private const double MIN_FREQUENCY = 0.1;
    private const double MAX_FREQUENCY = 0.6;
    [Header("Fourier (sharpness, between simple sin to sawtooth)")]
    public float ParamFourier;
    private const double FOURIER_STEP = 1;
    private const double MIN_FOURIER = 1;
    private const double MAX_FOURIER = 7;
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
        if (IsEditable && ParamStrength < MAX_STRENGTH)
            ParamStrength += (float) STRENGTH_STEP;
    }

    public void DecrementStrength()
    {
        if (IsEditable && ParamStrength > MIN_STRENGTH)
            ParamStrength -= (float) STRENGTH_STEP;
    }

    public void IncrementFrequency()
    {
        if (IsEditable && ParamFrequency < MAX_FREQUENCY)
            ParamFrequency += (float) FREQUENCY_STEP;
    }

    public void DecrementFrequency()
    {
        if (IsEditable && ParamFrequency > MIN_FREQUENCY)
            ParamFrequency -= (float) FREQUENCY_STEP;
    }

    public void IncrementFourier()
    {
        if (IsEditable && ParamFourier < MAX_FOURIER)
            ParamFourier += (float) FOURIER_STEP;
    }

    public void DecrementFourier()
    {
        if (IsEditable && ParamFourier > MIN_FOURIER)
            ParamFourier -= (float) FOURIER_STEP;
    }

    public void RandomizeSignal()
    {
        ParamStrength = (float) Random.Range(0, (int) (MAX_STRENGTH - MIN_STRENGTH) * 10) / 10 + (float) MIN_STRENGTH;
        ParamFrequency = (float) Random.Range(0, (int) (MAX_FREQUENCY - MIN_FREQUENCY) * 100) / 100 + (float) MIN_FREQUENCY;
        ParamFourier = Random.Range((int) MIN_FOURIER, (int) MAX_FOURIER);
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
