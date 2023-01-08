using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    [Range(0, 500)]
    public int mapSize = 1;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        Camera.main.orthographicSize = mapSize;

        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector2(mapSize,mapSize);
    }


    //public void GenerateBoids(int minx, int maxx, int miny, int maxy)
    //{
    //    for (int i = 0; i < numberOfBoids; i++)
    //    {
    //        this.GetComponent<BoidController>().CreateBoid(Random.Range(minx, maxx), Random.Range(miny, maxy), 0, $"Boid-{i}");
    //    }
    //}
}