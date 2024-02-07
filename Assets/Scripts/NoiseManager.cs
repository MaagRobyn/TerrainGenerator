using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NoiseManager : MonoBehaviour
{
    public Button newSeedButton;
    public RawImage noiseImage;
    //public Slider resolutionSlider;
    public Slider maxHeightSlider;
    public Slider scaleSlider;
    public Terrain terrain;
    /// <summary>
    /// the noisemap is always going to be a square, so this is the width and height of the image we get
    /// </summary>
    public int noiseResolution;
    public float noiseLevel;
    public TextMeshProUGUI seedValueLabel;

    private Noise noise;
    private float[,] noiseData;
    [SerializeField] NoiseType noiseType;
    float desiredHeight;
    float currentHeight;
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
                GenerateNewSeed();
                break;
        }
        noiseLevel = scaleSlider.value;
        scaleSlider.onValueChanged.AddListener((value) =>
        {
            noiseLevel = value;
            RegenerateNoiseTerrain();
        });
        //resolutionSlider.value = noiseResolution;
        //resolutionSlider.onValueChanged.AddListener((value) =>
        //{
        //    noiseResolution = (int)value;
        //    //terrain.
        //    RegenerateNoiseTerrain();
        //});
        desiredHeight = maxHeightSlider.value;
        maxHeightSlider.onValueChanged.AddListener(value =>
        {
            desiredHeight = value;
        });
        newSeedButton.onClick.AddListener(() =>
        {
            GenerateNewSeed();
            RegenerateNoiseTerrain();
        });
        RegenerateNoiseTerrain();
    }
    private void Update()
    {
        if(currentHeight != desiredHeight)
        {
            float difference = 0;
            if(currentHeight > desiredHeight)
            {
                difference = currentHeight - desiredHeight >= Time.deltaTime? -Time.deltaTime : desiredHeight - currentHeight;

            }
            else if(currentHeight < desiredHeight)
            {
                difference = desiredHeight - currentHeight >= Time.deltaTime ? Time.deltaTime : desiredHeight - currentHeight;
            }
            currentHeight += difference;
            RegenerateNoiseTerrain();
        }
    }

    private void GenerateNewSeed()
    {
        noise.seed = Random.Range(short.MinValue, short.MaxValue);
        seedValueLabel.text = noise.seed.ToString();
        GenerateNoise();
    }

    private void RegenerateNoiseTerrain()
    {
        GenerateNoise();
        SetNoiseTexture();
        ApplyNoiseToTerrain();
    }
    private void GenerateNoise()
    {
        noiseData = new float[noiseResolution, noiseResolution];
        for (int i = 0; i < noiseResolution * noiseResolution; i++)
        {
            var j = noise.GetNoiseMap(i / noiseResolution, i % noiseResolution, noiseLevel);
            noiseData[i % noiseResolution, i / noiseResolution] = j * currentHeight; // i % noiseScale = x coordinate, i / noiseScale = yCoordinate
            //Debug.Log($"{i % noiseResolution}, {i / noiseResolution}");
        }

    }

    private void SetNoiseTexture()
    {
        Color32[] pixels = new Color32[noiseResolution * noiseResolution];
        for(int i = 0; i < noiseResolution * noiseResolution; i++)
        {
            var noiseFactor = noiseData[i / noiseResolution, i % noiseResolution];
            pixels[i] = Color.Lerp(Color.black, Color.white, noiseFactor);
        }
        Texture2D texture = new(noiseResolution, noiseResolution);
        texture.SetPixels32(pixels);
        texture.Apply();
        noiseImage.texture = texture;
    }

    private void ApplyNoiseToTerrain()
    {
        terrain.terrainData.SetHeights(0, 0, noiseData);
    }
}
