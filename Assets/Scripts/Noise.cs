using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Noise
{
    public static Dictionary<string, Noise> NOISE_TYPES = new()
    {
        { "Perlin", new PerlinNoise() },
        { "White", new WhiteNoise() }
    };
    public int seed;
    public abstract float GetNoiseMap(float x, float y, float scale = 1f);
}
