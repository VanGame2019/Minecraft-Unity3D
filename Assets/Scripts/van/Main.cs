using System;
using System.Collections.Generic;
using map;
using UnityEngine;

namespace van
{
    public class Main:MonoBehaviour
    {
        private int width = 100;
        private int height = 100;
        
        FastNoise noise = new FastNoise();

        public List<Sprite> list;

        public GameObject container;
        
        private void Start()
        {
//            generateMap(); 
            createTexture();
        }

        private void createTexture()
        {
            MapCreater mapCreater = new MapCreater();
            mapCreater.Generate();

            var texture = mapCreater.Texture;
            var path = Application.dataPath;
            mapCreater.SaveRenderTextureToPNG(texture, path, "noiseTest4");
            Debug.Log(path);

        }
        
        private void generateMap()
        {
            var max = 0f;
            var min = 0f;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var v = getTile(i,j);

                    Sprite t;
                    switch (v)
                    {
                        case eTile.Grass:
                            t = list[1];
                            break;
                        case eTile.Sand:
                            t = list[4];
                            break;
                        case eTile.Mud:
                            t = list[3];
                            break;
                        case eTile.Water:
                            t = list[2];
                            break;
                        case eTile.Forest:
                            t = list[0];
                            break;
                        case eTile.Montain:
                            t = list[6];
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    {
                        var go = new GameObject();
                        go.AddComponent<SpriteRenderer>().sprite = t;

                        var x = (i - width / 2) * 1.6f;
                        if (j % 2 == 0)
                        {
                            x += 0.8f;
                        }

                        var y = (j - height / 2) * 1.2f;

                        go.transform.SetParent(this.container.transform, false);
                        go.transform.localPosition = new Vector3(x, y);
                    }
                    if (v == eTile.Montain)
                    {
                        var go = new GameObject();
                        var sr = go.AddComponent<SpriteRenderer>();
                        sr.sprite = list[5];
                        sr.sortingOrder = 1;
                        
                        var x = (i-width/2) * 1.6f;
                        if (j % 2 == 0)
                        {
                            x += 0.8f;
                        }
                        var y = (j-height/2) * 1.2f;
                    
                        go.transform.SetParent(this.container.transform,false);
                        go.transform.localPosition = new Vector3(x,y);
                    }
                    
                }
            }   
            
        }

        
        
        eTile getTile(int x, int y)
        {
            float simplex1 = noise.GetPerlin(x, y);

//            float simplex1 = Mathf.PerlinNoise(x*0.3f, y*0.2f);            

            Debug.Log($"{x},{y},{simplex1}");
            
            float result = simplex1 * 10;
            if (result < 3f)
            {
                return eTile.Water;
            }else if (result < 3.5f)
            {
                return eTile.Sand;
            }else if (result < 4.5f)
            {
                return eTile.Mud;
            }else if (result < 5.5f)
            {
                return eTile.Grass;
            }else if (result < 7f)
            {
                return eTile.Forest;
            }
            else
            {
                return eTile.Montain;
            }
            
            
            return eTile.Mud;
        }
    }
}