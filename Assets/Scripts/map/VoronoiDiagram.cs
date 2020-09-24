using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace map
{
    public class VoronoiDiagram:MonoBehaviour
    {
        public Vector2Int imageDim;
        public int regionAmount;
        public bool drawByDistance = false;
        private void Start()
        {
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(drawByDistance?GetDiagramByDistance(): GetDiagram(), new Rect(0, 0, imageDim.x, imageDim.y),
                Vector2.one * 0.5f);
        }

        Texture2D GetDiagram()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            Vector2Int[] centroids = new Vector2Int[regionAmount];
            Color[] regions = new Color[regionAmount];
            for (int i = 0; i < regionAmount; i++)
            {
                centroids[i] = new Vector2Int(Random.Range(0,imageDim.x),Random.Range(0,imageDim.y));
                regions[i] = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),1f);
            }

            Color[] pixelColors = new Color[imageDim.x * imageDim.y];
            for (int x = 0; x < imageDim.x; x++)
            {
                for (int y = 0; y < imageDim.y; y++)
                {
                    int index = x * imageDim.x + y;
                    pixelColors[index] = regions[GetClosestControidIndex(new Vector2Int(x, y), centroids)];
                }
            }

            sw.Stop();
            Debug.Log($"ms:{sw.ElapsedMilliseconds}");
            
            return GetImageFromColorArray(pixelColors);
        }

        Texture2D GetDiagramByDistance()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            Vector2Int[] centroids = new Vector2Int[regionAmount];
            for (int i = 0; i < regionAmount; i++)
            {
                centroids[i] = new Vector2Int(Random.Range(0,imageDim.x),Random.Range(0,imageDim.y));
            }

            Color[] pixelColors = new Color[imageDim.x * imageDim.y];
            float[] distances = new float[imageDim.x*imageDim.y];
            for (int x = 0; x < imageDim.x; x++)
            {
                for (int y = 0; y < imageDim.y; y++)
                {
                    int index = x * imageDim.x + y;
                    distances[index] = Vector2.Distance(new Vector2Int(x, y),
                        centroids[GetClosestControidIndex(new Vector2Int(x, y), centroids)]);
                }
            }

            float maxDst = GetMaxDistance(distances);
            for (int i = 0; i < distances.Length; i++)
            {
                float colorValue = distances[i] / maxDst;
                pixelColors[i] = new Color(colorValue,colorValue,colorValue,1f);
            }

            sw.Stop();
            Debug.Log($"ms:{sw.ElapsedMilliseconds}");
            
            return GetImageFromColorArray(pixelColors);
        }

        float GetMaxDistance(float[] distances)
        {
            float maxDst = float.MinValue;
            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] > maxDst)
                {
                    maxDst = distances[i];
                }
            }

            return maxDst;
        }
        
        int GetClosestControidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
        {
            float smallestDst = float.MaxValue;
            int index = 0;
            for (int i = 0; i < centroids.Length; i++)
            {
                if (Vector2.Distance(pixelPos, centroids[i]) < smallestDst)
                {
                    smallestDst = Vector2.Distance(pixelPos, centroids[i]);
                    index = i;
                }
            }

            return index;
        }

        Texture2D GetImageFromColorArray(Color[] pixelColors)
        {
            Texture2D tex = new Texture2D(imageDim.x,imageDim.y);
            tex.filterMode = FilterMode.Point;
            tex.SetPixels(pixelColors);
            tex.Apply();
            return tex;
        }
        
    }
}