using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalController : MonoBehaviour
{
    public TrailRenderer TrailA;
    public TrailRenderer TrailB;
    public float ParamStrength;
    public float ParamFrequency;
    public float ParamFourier;
    public float MinLimit;
    public float MaxLimit;
    public float YOffset;
    public float Speed;

    private float _currentPosition = 0;
    private TrailRenderer _tempTrail;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(MinLimit, YOffset, 0), new Vector3(MaxLimit, YOffset, 0));
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
        if (!(_currentPosition >= MaxLimit)) return false;
        _currentPosition = MinLimit;
        _tempTrail = TrailA;
        TrailA = TrailB;
        TrailB = _tempTrail;
        return true;

    }

    public IEnumerator Move()
    {
        _currentPosition = MinLimit;
        while (true)
        {
            increasePosition();
            transform.position = new Vector3(_currentPosition, GetY(), 0);
            TrailA.transform.position = transform.position;
            yield return new WaitForEndOfFrame();
        }
    }
}
