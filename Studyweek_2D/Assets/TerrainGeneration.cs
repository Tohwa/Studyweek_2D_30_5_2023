using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    #region Fields
    [Header("Tile Sprites")]
    [SerializeField] private Sprite stone;
    [SerializeField] private Sprite dirt;
    [SerializeField] private Sprite grass;
    [SerializeField] private Sprite log;
    [SerializeField] private Sprite leaf;

    [Header("Trees")]
    [SerializeField] private int treeChance = 10;
    [SerializeField] private int minTreeHeight = 3;
    [SerializeField] private int maxTreeHeight = 6;

    [Header("Generation Settings")]
    [SerializeField] private bool generateCaves = true;
    [SerializeField] private int dirtLayerHeigt = 5;
    [SerializeField] private float surfaceValue = 0.25f;
    [SerializeField] private float heightMultiplier = 4f;
    [SerializeField] private int heightAddition = 25;
    [SerializeField] private int worldSize = 100;

    [Header("Noise Settings")]
    [SerializeField] private float caveFreq = 0.05f;
    [SerializeField] private float terrainFreq = 0.05f;
    [SerializeField] private float seed;
    [SerializeField] private Texture2D noiseTexture;

    [SerializeField] private List<Vector2> worldTiles = new List<Vector2>();
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
                    //Terrain top Layer
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

                if(y >= height - 1)
                {
                    int t = Random.Range(0, treeChance);
                    if (t == 1)
                    {
                        //generate Tree
                        if (worldTiles.Contains(new Vector2(x, y)))
                        {
                            GenerateTree(x, y + 1);
                        }
                    }
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

    public void GenerateTree(float x, float y)
    {
        //define tree

        //generate log
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        for(int i = 0; i <= treeHeight; i++)
        {
            PlaceTile(log, x, y + i);
        }

        //generate leaves
        PlaceTile(leaf, x, y + treeHeight);
        PlaceTile(leaf, x, y + treeHeight + 1);
        PlaceTile(leaf, x, y + treeHeight + 2);

        PlaceTile(leaf, x - 1, y + treeHeight);
        PlaceTile(leaf, x - 1, y + treeHeight + 1);

        PlaceTile(leaf, x + 1, y + treeHeight);
        PlaceTile(leaf, x + 1, y + treeHeight + 1);
    }

    public void PlaceTile(Sprite tileSprite, float x, float y)
    {
        GameObject newTile = new GameObject();
        newTile.transform.parent = this.transform;
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
