using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;


public class HeightmapToSDF
{
   public Texture2D GenerateSDF(Texture2D input, bool calculateInside)
    {
        //Stopwatch to calculate time taken for the algorithm
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        int width = input.width;
        int height = input.height;

        // Create the SDF texture
        Texture2D sdfTexture = new Texture2D(width, height, TextureFormat.RFloat, false);

        // Initialize seed points
        Vector2Int[,] closestSeed = new Vector2Int[width, height];
        float[,] minDistances = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightValue = input.GetPixel(x, y).r;

                if (calculateInside)
                {
                    if (Mathf.Approximately(heightValue, 0f)) 
                    {
                        closestSeed[x, y] = new Vector2Int(x, y);
                        minDistances[x, y] = 0f; 
                    }
                    else 
                    {
                        closestSeed[x, y] = new Vector2Int(-1, -1); 
                        minDistances[x, y] = float.MaxValue; 
                    }
                    
                }
                else
                {
                    if (Mathf.Approximately(heightValue, 0f)) 
                    {
                        closestSeed[x, y] = new Vector2Int(-1, -1); 
                        minDistances[x, y] = float.MaxValue; 
                    }
                    else 
                    {
                        closestSeed[x, y] = new Vector2Int(x, y);
                        minDistances[x, y] = 0f; 
                    }
                }
            }
        }

        // Jump Flood Algorithm
        int maxJump = Mathf.CeilToInt(Mathf.Log(Mathf.Max(width, height), 2));
        int jump = (int)Mathf.Pow(2, maxJump - 1);

        while (jump > 0)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int currentSeed = closestSeed[x, y];
                    float currentDistance = minDistances[x, y];

                    // Check neighbors
                    for (int dx = -jump; dx <= jump; dx += jump)
                    {
                        for (int dy = -jump; dy <= jump; dy += jump)
                        {
                            int nx = x + dx;
                            int ny = y + dy;

                            if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                            {
                                Vector2Int neighborSeed = closestSeed[nx, ny];
                                if (neighborSeed.x != -1 && neighborSeed.y != -1) // Valid seed
                                {
                                    float dxToSeed = x - neighborSeed.x;
                                    float dyToSeed = y - neighborSeed.y;
                                    float distanceToSeed = Mathf.Sqrt(dxToSeed * dxToSeed + dyToSeed * dyToSeed);

                                    if (distanceToSeed < currentDistance)
                                    {
                                        currentDistance = distanceToSeed;
                                        currentSeed = neighborSeed;
                                    }
                                }
                            }
                        }
                    }

                    closestSeed[x, y] = currentSeed;
                    minDistances[x, y] = currentDistance;
                }
            }

            jump /= 2; // Halve the jump size
        }
        
        // Find min and max distance values
        
        float minDistance = float.MaxValue;
        float maxDistance = float.MinValue;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float distance = minDistances[x, y];
                minDistance = Mathf.Min(minDistance, distance);
                maxDistance = Mathf.Max(maxDistance, distance);
            }
        }

        // Normalize distances to [0, 1] range
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float distance = minDistances[x, y];
                float normalizedValue = Mathf.InverseLerp(minDistance, maxDistance, distance); // Map to [0, 1]

                sdfTexture.SetPixel(x, y, new Color(normalizedValue, normalizedValue, normalizedValue, 1f));
            }
        }

        sdfTexture.Apply();

        stopwatch.Stop();
        UnityEngine.Debug.Log("Seconds taken to generate SDF: " + stopwatch.ElapsedMilliseconds / 1000f);
        
        //Use this to save the SDF as a PNG
        byte[] bytes = sdfTexture.EncodeToPNG();
        var dirPath = Application.dataPath + "/../GeneratedSDF/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        string inside = calculateInside ? "inside" : "outside";
        File.WriteAllBytes(dirPath + (input.name + "_" + inside) +  ".png", bytes);
        
        return sdfTexture;
    }
}
