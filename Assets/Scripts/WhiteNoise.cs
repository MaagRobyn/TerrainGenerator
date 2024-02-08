using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class WhiteNoise : Noise
    {
        public override float GetNoiseMap(float x, float y, float scale = 1)
        {
            return Random.Range(1-scale, scale);
        }
    }
}