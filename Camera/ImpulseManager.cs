using System.Collections.Generic;
using UnityEngine;

public struct CameraImpulse
{
    public Vector3 PositionImpulse;
    public Vector3 RotationImpulse;
    public float ExpireTime;

    public CameraImpulse(Vector3 pos, Vector3 rot, float duration)
    {
        PositionImpulse = pos;
        RotationImpulse = rot;
        ExpireTime = Time.time + duration;
    }
}

public static class ImpulseManager
{
    private static readonly List<CameraImpulse> impulses = new();

    public static void GenerateImpulse(Vector3 positionShake, Vector3 rotationShake, float duration = 0.5f)
    {
        impulses.Add(new CameraImpulse(positionShake, rotationShake, duration));
    }

    public static Vector3 GetPositionShake()
    {
        Vector3 total = Vector3.zero;
        for (int i = impulses.Count - 1; i >= 0; i--)
        {
            if (Time.time > impulses[i].ExpireTime)
                impulses.RemoveAt(i);
            else
                total += impulses[i].PositionImpulse * Mathf.Sin((Time.time - (impulses[i].ExpireTime - 0.5f)) * 50f);
        }
        return total;
    }

    public static Vector3 GetRotationShake()
    {
        Vector3 total = Vector3.zero;
        for (int i = impulses.Count - 1; i >= 0; i--)
        {
            if (Time.time > impulses[i].ExpireTime)
                impulses.RemoveAt(i);
            else
                total += impulses[i].RotationImpulse * Mathf.Sin((Time.time - (impulses[i].ExpireTime - 0.5f)) * 20f);
        }
        return total;
    }
}