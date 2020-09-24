using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using UnityEngine;
using van;
using Color = UnityEngine.Color;
using Debug = UnityEngine.Debug;
using FNfloat = System.Single;

namespace map
{
    
    public class MapCreater
    {
        private bool VisualiseDomainWarp = false;
        private bool DomainWarp = false;
        
        private bool Is3D = false;
        private bool Invert = false;
        
        private int PreviewWidth = 768;
        private int PreviewHeight = 768;
        
        public Texture2D Texture;
        private Color32[] ImageData;

        private float zPos = 0;

        private int count = 0;
        private int count_float = 0;
        private Dictionary<float,int> dic_value;
        private Dictionary<float,List<int>> dic_point;
        private Dictionary<float, Vector2> dic_center;
        private Dictionary<float, int> dic_float;
        
        public void Generate()
        {
            
        
            // Create noise generators
            var genNoise = new FastNoiseLite();
            var warpNoise = new FastNoiseLite();

            int w = (int)PreviewWidth;
            int h = (int)PreviewHeight;

            ImageData = new Color32[1];
            dic_value = new Dictionary<float, int>();
            dic_point = new Dictionary<float, List<int>>();
            dic_float = new Dictionary<float, int>();
            if (w <= 0 || h <= 0)
            {
                return;
            }

            genNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            genNoise.SetRotationType3D(FastNoiseLite.RotationType3D.None);
            genNoise.SetSeed(1337);
            genNoise.SetFrequency(0.015f);
            genNoise.SetFractalType(FastNoiseLite.FractalType.None);
            genNoise.SetFractalOctaves(4);
            genNoise.SetFractalLacunarity(2);
            genNoise.SetFractalGain(0.5f);
            genNoise.SetFractalWeightedStrength(0);
            genNoise.SetFractalPingPongStrength(2);

            genNoise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            genNoise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);//生成线条的时候 这里设置为Distance2Div
            genNoise.SetCellularJitter(1f);

            warpNoise.SetSeed(1337);
            warpNoise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            warpNoise.SetRotationType3D(FastNoiseLite.RotationType3D.None);
            warpNoise.SetDomainWarpAmp(100);
            warpNoise.SetFrequency(0.01f);
            warpNoise.SetFractalType(FastNoiseLite.FractalType.DomainWarpIndependent);
            warpNoise.SetFractalOctaves(3);
            warpNoise.SetFractalLacunarity(2);
            warpNoise.SetFractalGain(0.5f);

            if (ImageData.Length != w * h)
            {
                ImageData = new Color32[w * h];
            }

            float noise;
            float minN = float.MaxValue;
            float maxN = float.MinValue;
            float avg = 0;

            bool get3d = Is3D;
            bool invert = Invert;

            // Timer
            Stopwatch sw = new Stopwatch();

            int index = 0;
            if (VisualiseDomainWarp != true)
            {
                var noiseValues = new float[w * h];
                bool warp = true;

                sw.Start();
                for (var y = h / -2; y < h / 2; y++)
                {
                    for (var x = w / -2; x < w / 2; x++)
                    {
                        FNfloat xf = x;
                        FNfloat yf = y;
                        FNfloat zf = zPos;


                        if (get3d)
                        {
                            if (warp)
                                warpNoise.DomainWarp(ref xf, ref yf, ref zf);

                            noise = genNoise.GetNoise(xf, yf, zf);
                        }
                        else
                        {
                            if (warp)
                                warpNoise.DomainWarp(ref xf, ref yf);

                                noise = genNoise.GetNoise(xf, yf);
                        }

                        avg += noise;
                        maxN = Math.Max(maxN, noise);
                        minN = Math.Min(minN, noise);
                        noiseValues[index++] = noise;
                        if (!dic_float.ContainsKey(noise))
                        {
                            dic_float.Add(noise,1);
                            count_float++;
                        }
                    }
                }
                sw.Stop();
                Debug.Log( "Time (ms): " + sw.ElapsedMilliseconds.ToString());

                sw.Start();

                avg /= index - 1;
                float scale = 255 / (maxN - minN);

                for (var i = 0; i < noiseValues.Length; i++)
                {
                    int value = (int)Mathf.Round(Mathf.Clamp((noiseValues[i] - minN) * scale, 0, 255));
                    float vF = noiseValues[i];
                    
                    if (invert)
                        value = 255 - value;

//                    if (value < 250)//生成线条时候时候使用
//                        value = 0;

                    if (!dic_value.ContainsKey(vF))
                    {
                        dic_value.Add(vF,0);
                        dic_point.Add(vF,new List<int>());
                        count++;
                    }
                    if (isNear(dic_point[vF], i))
                    {
                        dic_value[vF]++;
                        dic_point[vF].Add(i);
                        dic_float[vF] = value;
                    }
                    
                    float vFloat = (float)value / 255f;
                    
                    Color c = new Color();
                    c.r = vFloat;
                    c.g = vFloat;
                    c.b = vFloat;
                    c.a = 1;

                    ImageData[i] = (Color32) c;
                }
                sw.Stop();
            }
            else
            {
                var noiseValues = new float[w * h * 3];

                sw.Start();
                for (var y = -h / 2; y < h / 2; y++)
                {
                    for (var x = -w / 2; x < w / 2; x++)
                    {
                        FNfloat xf = x;
                        FNfloat yf = y;
                        FNfloat zf = zPos;

                        if (get3d)
                            warpNoise.DomainWarp(ref xf, ref yf, ref zf);
                        else
                            warpNoise.DomainWarp(ref xf, ref yf);

                        xf -= x;
                        yf -= y;
                        zf -= zPos;

                        avg += (float)(xf + yf);
                        maxN = Math.Max(maxN, (float)Math.Max(xf, yf));
                        minN = Math.Min(minN, (float)Math.Min(xf, yf));

                        noiseValues[index++] = (float)xf;
                        noiseValues[index++] = (float)yf;

                        if (get3d)
                        {
                            avg += (float)zf;
                            maxN = Math.Max(maxN, (float)zf);
                            minN = Math.Min(minN, (float)zf);
                            noiseValues[index++] = (float)zf;
                        }
                    }
                }
                sw.Stop();

                if (get3d)
                    avg /= (index - 1) * 3;
                else avg /= (index - 1) * 2;

                index = 0;
                float scale = 1 / (maxN - minN);

                for (var i = 0; i < ImageData.Length; i++)
                {
                    Color color = new Color();

                    if (get3d)
                    {
                        color.r = (noiseValues[index++] - minN) * scale;
                        color.g = (noiseValues[index++] - minN) * scale;
                        color.b = (noiseValues[index++] - minN) * scale;
                    }
                    else
                    {
                        var vx = (noiseValues[index++] - minN) / (maxN - minN) - 0.5f;
                        var vy = (noiseValues[index++] - minN) / (maxN - minN) - 0.5f;

                        ColorHSB hsb = new ColorHSB();

                        hsb.H = Mathf.Atan2(vy, vx) * (180 / Mathf.PI) + 180;
                        hsb.B = Math.Min(1.0f, Mathf.Sqrt(vx * vx + vy * vy) * 2);
                        hsb.S = 0.9f;

                        color = hsb.ToColor();
                    }

                    if (Invert)
                    {
                        color = InvertColor(color);
                    }

                    ImageData[i] = color;
                }
            }

            // Set image
//            Bitmap = new Bitmap(w, h, PixelFormat.Format32bppRgb, ImageData);
//            Image.Image = Bitmap;

            Texture = new Texture2D(PreviewWidth,PreviewHeight);
            Texture.SetPixels32(ImageData);
            
            // Set info labels
            Debug.Log( "Time (ms): " + sw.ElapsedMilliseconds.ToString());
            Debug.Log("Mean: " + avg.ToString());
            Debug.Log("Min: " + minN.ToString());
            Debug.Log( "Max: " + maxN.ToString());

            Debug.Log($"共有{count}个国家");
            Debug.Log($"共有{count_float}个真国家");
            List<Country> list_country = new List<Country>();
            foreach (var i in dic_value)
            {
                Country c = new Country();
                c.color = dic_float[i.Key];
                c.size = i.Value;
                list_country.Add(c);
            }
            list_country.Sort((a,b)=>b.size.CompareTo(a.size));
            foreach (var country in list_country)
            {
//                Debug.Log($"颜色 {country.color} ,大小 {country.size}");
            }

            foreach (var Variable in dic_point)
            {
                var id = dic_float[Variable.Key];
                var list = Variable.Value;
                var size = list.Count;
                var bounds = new Bounds();
                var list_vec = new List<Vector2>();
                int cc = 0;
                foreach (var i in list)
                {
                    var point = new Vector2(i% PreviewWidth,Mathf.Floor((float)i/ PreviewHeight));
                    list_vec.Add(point);
                    if (cc == 0)
                    {
                        bounds = new Bounds(point,new Vector3(1,1));
                    }
                    else
                    {
                        bounds.Encapsulate(point);
                    }
                    
                    cc++;
                }

                if(size<10)
                    continue;
                
                var center = GetCenterOfGravityPoint(list_vec);
                Debug.Log($"颜色 {id} , 小数{Variable.Key},大小{size}, 中心{bounds.center}");
                TextManager.instance.createText(id.ToString(),size,bounds.center,bounds.size);
            }
            
        }

        private Color InvertColor(Color c)
        {
            Color cNew = new Color();
            cNew.a = c.a;
            cNew.r = 1 - c.r;
            cNew.g = 1 - c.g;
            cNew.b = 1 - c.b;
            return cNew;
        }
        
        public bool SaveRenderTextureToPNG(Texture2D png,string contents, string pngName)
        {
            byte[] bytes = png.EncodeToPNG();
            if (!Directory.Exists(contents))
                Directory.CreateDirectory(contents);
            FileStream file = File.Open(contents + "/" + pngName + ".png", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);
            file.Close();
            
            Texture2D.DestroyImmediate(png);
            
            return true;
        } 

        /// <summary>
        /// 获取不规则多边形重心点
        /// </summary>
        /// <param name="mPoints"></param>
        /// <returns></returns>
        public static Vector2 GetCenterOfGravityPoint(List<Vector2> mPoints)
        {
            float area = 0.0f;//多边形面积
            float gx = 0.0f, gy = 0.0f;// 重心的x、y
            for (int i = 1; i <= mPoints.Count; i++)
            {
                float iX = mPoints[i % mPoints.Count].x;
                float iY = mPoints[i % mPoints.Count].y;
                float nextX = mPoints[i - 1].x;
                float nextY = mPoints[i - 1].y;
                float temp = (iX * nextY - iY * nextX) / 2.0f;
                area += temp;
                gx += temp * (iX + nextX) / 3.0f;
                gy += temp * (iY + nextY) / 3.0f;
            }
            gx = gx / area;
            gy = gy / area;
            Vector2 v2 = new Vector2(gx, gy);
            return v2;
        }

        private bool isNear(List<int> list, int v)
        {
            if (list.Count == 0)
                return true;
            
            foreach (var i in list)
            {
                if(Mathf.Abs(i-v)==1||Mathf.Abs(i-v)==PreviewWidth)
                    return true;
            }

            return false;
        }
    }

    class ColorHSB
    {
        public float H;
        public float B;
        public float S;

        public Color ToColor()
        {
            return Color.HSVToRGB(H, S, B);
        }
    }

    class Country
    {
        public int color;
        public int size;
    }
}