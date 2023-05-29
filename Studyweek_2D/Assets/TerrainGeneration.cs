using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    #region Fields
    [SerializeField] private Sprite stone;
    [SerializeField] private Sprite dirt;
    [SerializeField] private Sprite grass;
    [SerializeField] private Sprite log;
    [SerializeField] private Sprite leaf;

    [SerializeField] private bool generateCaves = true;
    [SerializeField] private int dirtLayerHeigt = 5;
    [SerializeField] private float surfaceValue = 0.25f;
    [SerializeField] private float heightMultiplier = 4f;
    [SerializeField] private int heightAddition = 25;
    [SerializeField] private int worldSize = 100;
    [SerializeField] private float caveFreq = 0.05f;
    [SerializeField] private float terrainFreq = 0.05f;
    [SerializeField] private float seed;
    [SerializeField] private Texture2D noiseTexture;
    #endregion

    private void Start()
    {
        seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture();
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        for (int x = 0; x < worldSize; x++)
        {
            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;

            for (int y = 0; y < height; y++)
            {
                Sprite tileSprite;
                if(y < height - dirtLayerHeigt)
                {
                    tileSprite = stone;                    
                }
                else if(y < height - 1)
                {
                    tileSprite = dirt;
                }
                else
                {
                    tileSprite = grass;
                }

                if (generateCaves)
                {
                    if (noiseTexture.GetPixel(x, y).r > surfaceValue)
                    {
                        PlaceTile(tileSprite, x, y);
                    }
                }
                else
                {
                    PlaceTile(tileSprite, x, y);
                }
                
            }            
        }
    }

    public void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(worldSize, worldSize);

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }

        noiseTexture.Apply();
    }

    public void PlaceTile(Sprite tileSprite, float x, float y)
    {
        GameObject newTile = new GameObject();
        newTile.transform.parent = this.transform;
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
    }
}
