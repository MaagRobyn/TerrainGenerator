using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class RobynNoise : Noise
    {
        private float LastValue = -1.0f;
        public override float GetNoiseMap(float x, float y, float scale = 1)
        {
            var output = LastValue;
            if(output < 0)
            {
                output = Random.Range(0.0f, 1.0f);
            }
            else
            {
                var coinFlip = Random.Range(0, 2);
                var variance = Random.Range(0, scale);
                if(coinFlip == 0)
                {
                    output += variance;
                    if(output > 1)
                    {
                        output = 1;
                    }
                }
                else
                {
                    output -= variance;
                    if(output < 0)
                    {
                        output = 0;
                    }
                }
            }
            LastValue = output;
            return output;
        }
    }
}