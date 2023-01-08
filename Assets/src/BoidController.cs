﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.WSA.Input;

public class BoidController : MonoBehaviour 
{
    /// <summary>
    /// Util Class to update all the boids at once or a particular one
    /// </summary>
    public static List<Boid> boids { get; set; }
    public Boid prefab;
    [Range(0, 1000)]
    public int numBoids = 50;
    public bool drawRadius = false;

    public Color colour = Color.white;
    Color tempColour = Color.white;
    
    bool userSelected = false; //user has selected a boid

    
    public float separationWeight = 0;
    public float alignmentWeight = 0;
    public float cohesionWeight = 0;


    void Start()
    {
        boids = new List<Boid>();
        for (int i = 0; i < numBoids; i++)
        {
            prefab.transform.position = new Vector3(Random.Range(-Camera.main.orthographicSize, Camera.main.orthographicSize), Random.Range(-Camera.main.orthographicSize, Camera.main.orthographicSize), 0);
            boids.Add(Instantiate(prefab));
        }
    }

    void Update()
    {
        if(userSelected)
        {
            //get selected boid and update
        }
        else
        {
            foreach(Boid boid in boids)
            {
                UpdateRadiusCircle(boid);
                UpdateColour(boid, colour);
                UpdateBoids(boid);
            }
        }
        tempColour = colour;
    }

    public static List<Boid> GetBoids() => boids;

    public void UpdateColour(Boid boid, Color color)
    {
        if (color != tempColour)
        {
            boid.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void UpdateRadiusCircle(Boid boid)
    {
        if (drawRadius)
        {
            boid.GetComponent<LineRenderer>().startColor = colour;
            boid.GetComponent<LineRenderer>().forceRenderingOff = false;
        }
        else
        {
            boid.GetComponent<LineRenderer>().startColor = colour;
            boid.GetComponent<LineRenderer>().forceRenderingOff = true;  
        }
    }

    private void UpdateBoids(Boid boid)
    {
        boid.separationWeight = separationWeight;
        boid.alignmentWeight  = alignmentWeight;
        boid.cohesionWeight   = cohesionWeight;
    }
}