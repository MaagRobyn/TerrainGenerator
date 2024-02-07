using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : Noise
{
    public override float GetNoiseMap(float x, float y, float scale = 1)
    {
        x = scale * (x + seed);
        y = scale * (y + seed);
        return Mathf.PerlinNoise(x, y);
    }
}
