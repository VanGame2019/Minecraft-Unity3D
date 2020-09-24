using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 获取中心点、重心点、最大最小值
/// </summary>
public class GetCenterPointFunction
{
    /// <summary>
    /// 获取不规则多边形几何中心点
    /// </summary>
    /// <param name="mPoints"></param>
    /// <returns></returns>
    public static Vector2 GetCenterPoint(List<Vector2> mPoints)
    {
        float cx = (GetMinX(mPoints) + GetMaxX(mPoints)) / 2;
        float cy = (GetMinY(mPoints) + GetMaxY(mPoints)) / 2;
        return new Vector2(cx, cy);
    }
    /// <summary>
    /// 获取最小X值
    /// </summary>
    /// <param name="mPoints"></param>
    /// <returns></returns>
    public static float GetMinX(List<Vector2> mPoints)
    {
        float minX = 0;
        if (mPoints.Count > 0)
        {
            minX = mPoints[0].x;
            foreach (Vector2 point in mPoints)
            {
                if (point.x < minX)
                    minX = point.x;
            }
        }
        return minX;
    }
    /// <summary>
    /// 获取最大X值
    /// </summary>
    /// <param name="mPoints"></param>
    /// <returns></returns>
    public static float GetMaxX(List<Vector2> mPoints)
    {
        float maxX = 0;
        if (mPoints.Count > 0)
        {
            maxX = mPoints[0].x;
            foreach (Vector2 point in mPoints)
            {
                if (point.x > maxX)
                    maxX = point.x;
            }
        }
        return maxX;
    }
    /// <summary>
    /// 获取最小Y值
    /// </summary>
    /// <param name="mPoints"></param>
    /// <returns></returns>
    public static float GetMinY(List<Vector2> mPoints)
    {
        float minY = 0;
        if (mPoints.Count > 0)
        {
            minY = mPoints[0].y;
            foreach (Vector2 point in mPoints)
            {
                if (point.y < minY)
                    minY = point.y;
            }
        }
        return minY;
    }
    /// <summary>
    /// 获取最大Y值
    /// </summary>
    /// <param name="mPoints"></param>
    /// <returns></returns>
    public static float GetMaxY(List<Vector2> mPoints)
    {
        float maxY = 0;
        if (mPoints.Count > 0)
        {
            maxY = mPoints[0].y;
            foreach (Vector2 point in mPoints)
            {
                if (point.y > maxY)
                    maxY = point.y;
            }
        }
        return maxY;
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
}

