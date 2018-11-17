using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class OutOfCamera : MonoBehaviour
{


    public static float objArea;
    List<Vector2> polygonPoints = null;
    // Use this for initialization
    void OnBecameInvisible()
    {
        PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();
        polygonPoints = new List<Vector2>(polygonCollider.points);
        float chArea = Mathf.Abs(Area(ref polygonPoints));
        Debug.Log("invisible");

        if (ScoreManager.count >= 1)
        {
            if (gameObject.layer != LayerMask.NameToLayer("nonslice"))
            {
                if (gameObject.layer != LayerMask.NameToLayer("trans")) { 
                ScoreManager.objArea += chArea;
            }
        }
        }
        Destroy(gameObject);
    }


    static float Area(ref List<Vector2> points)
    {
        int n = points.Count;
        float A = 0.0f;

        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = points[p];
            Vector2 qval = points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }

        return (A * 0.5f);
    }
}
