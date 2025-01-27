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
    public TMP_Dropdown dropdown;

    public Color deepSea;
    public Color coast;
    public Color grass;
    public Color higherElevationGrass;
    public Color lowerSnow;
    public Color upperSnow;

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
        var options = new List<TMP_Dropdown.OptionData>();
        foreach(var key in Noise.NOISE_TYPES.Keys)
        {
            options.Add(new TMP_Dropdown.OptionData(key));
        }
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(i =>
        {
            noise = Noise.NOISE_TYPES[dropdown.options[i].text];
            GenerateNewSeed();
            RegenerateNoiseTerrain();
        });

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
            if(noise != null)
            {
                GenerateNewSeed();
                RegenerateNoiseTerrain();

            }
        });

        noise = Noise.NOISE_TYPES[dropdown.options[0].text];
        GenerateNewSeed();
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
        if(noise != null)
        {
            noiseData = new float[noiseResolution, noiseResolution];
            for (int i = 0; i < noiseResolution * noiseResolution; i++)
            {
                var j = noise.GetNoiseMap(i / noiseResolution, i % noiseResolution, noiseLevel);

                noiseData[i % noiseResolution, i / noiseResolution] = j * currentHeight + Mathf.Lerp(.25f, 0, currentHeight); // i % noiseScale = x coordinate, i / noiseScale = yCoordinate
            }
        }

    }

    private void SetNoiseTexture()
    {
        noiseData ??= new float[noiseResolution, noiseResolution];

        Color32[] pixels = new Color32[noiseResolution * noiseResolution];
        for(int i = 0; i < noiseResolution * noiseResolution; i++)
        {
            var noiseFactor = noiseData[i / noiseResolution, i % noiseResolution];
            if(noiseFactor < .2)
            {
                pixels[i] = Color32.Lerp(deepSea, coast, noiseFactor * 5);
            }
            else if(noiseFactor < .6)
            {
                pixels[i] = Color32.Lerp(grass, higherElevationGrass, noiseFactor * 5/3);
            }
            else
            {
                pixels[i] = Color32.Lerp(lowerSnow, upperSnow, noiseFactor);
            }
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
