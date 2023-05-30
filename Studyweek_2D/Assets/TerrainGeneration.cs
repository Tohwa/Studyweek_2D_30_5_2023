using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    #region Fields
    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;

    [Header("Trees")]
    [SerializeField] private int treeChance = 10;
    [SerializeField] private int minTreeHeight = 3;
    [SerializeField] private int maxTreeHeight = 6;

    [Header("Generation Settings")]
    [SerializeField] private int chunkSize = 20;
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

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();
    #endregion

    private void Start()
    {
        seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture();
        CreateChunks();
        GenerateTerrain();
    }

    public void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];

        for(int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject(name = i.ToString());
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
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
                    tileSprite = tileAtlas.stone.tileSprite;
                }
                else if(y < height - 1)
                {
                    tileSprite = tileAtlas.dirt.tileSprite;
                }
                else
                {
                    //Terrain top Layer
                    tileSprite = tileAtlas.grass.tileSprite;                    
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

    public void GenerateTree(int x, int y)
    {
        //define tree

        //generate log
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        for(int i = 0; i <= treeHeight; i++)
        {
            PlaceTile(tileAtlas.log.tileSprite, x, y + i);
        }

        //generate leaves
        PlaceTile(tileAtlas.leaf.tileSprite, x, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprite, x, y + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprite, x, y + treeHeight + 2);

        PlaceTile(tileAtlas.leaf.tileSprite, x - 1, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprite, x - 1, y + treeHeight + 1);

        PlaceTile(tileAtlas.leaf.tileSprite, x + 1, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprite, x + 1, y + treeHeight + 1);
    }

    public void PlaceTile(Sprite tileSprite, int x, int y)
    {
        GameObject newTile = new GameObject();

        float chunkCoord = Mathf.RoundToInt(x / chunkSize) * chunkSize;
        chunkCoord /= chunkSize;
        newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
