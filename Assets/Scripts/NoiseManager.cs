using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoiseManager : MonoBehaviour
{
    public Button regenerateNoiseButton;
    public RawImage noiseImage;
    public Slider scaleSlider;
    public int width;
    public int height;
    public float scale;
    
    private Noise noise;
    [SerializeField] NoiseType noiseType;
    private enum NoiseType
    {
        PerlinNoise
    }

    private void Start()
    {
        switch (noiseType)
        {
            case NoiseType.PerlinNoise:
                noise = new PerlinNoise();
                break;
        }
        scaleSlider.value = scale;
        scaleSlider.onValueChanged.AddListener((value) =>
        {
            scale = value;
        });

        regenerateNoiseButton.onClick.AddListener(() =>
        {
            SetNoiseTexture();
        });
    }
    private void SetNoiseTexture()
    {
        Color[] pixels = new Color[width * height];
        Texture2D texture = new Texture2D(width, height);
        for(int i = 0; i < width * height; i++)
        {
            var j = noise.GetNoiseMap(i, i % width, scale);
            pixels[i] = Color.Lerp(Color.black, Color.white, j); ;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        noiseImage.texture = texture;
    }
}
