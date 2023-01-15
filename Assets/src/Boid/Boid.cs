using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color color = new Color(1, 1, 0.5f, 1);
    
    public float speed = 10.0f;
    public float rotationSpeed = 0.5f;

    List<Boid> nearbyBoids;
    public float viewRadius = 15.0f;
    public float separationWeight { get; set; }
    public float cohesionWeight { get; set; }
    public float alignmentWeight { get; set; }
    float separationDistance = 4.0f;
    float alignmentDistance = 5.0f;

    //ScreenWrapping
    float leftConstraint = Screen.width;
    float rightConstraint = Screen.width;
    float topConstraint = Screen.height;
    float bottomConstraint = Screen.height;
    float buffer = 0.5f;
    LineRenderer line;

    [Range(0, 50)]
    public int segments = 50;

    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/BoidSprite");
        spriteRenderer.color = color;
        
        transform.localScale = new Vector3(0.33f,0.33f,0.33f);
        transform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        
        Camera cam = Camera.main;
        float distanceZ = Mathf.Abs(cam.transform.position.z + transform.position.z);
        leftConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).x;
        rightConstraint = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, distanceZ)).x;
        bottomConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).y;
        topConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, distanceZ)).y;

        //CreateLineToShowRadius
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = (segments + 1);
        line.useWorldSpace = false;
        CreatePoints();
    }

    void Update()
    {
        ScreenWrap();
        UpdateBoid();
        transform.position += transform.up * speed * Time.deltaTime;
        //transform.position += Mathf.Sin(transform.rotation.z) * transform.up * Time.deltaTime;
        //transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void UpdateBoid()
    {
        UpdateNearbyBoids();
        if (nearbyBoids == null || !nearbyBoids.Any())
            return;

        Vector2 separation = Vector2.zero;
        Vector2 alignment  = Vector2.zero;
        Vector2 cohesion   = Vector2.zero;
        
        Vector2 distance = Vector2.zero;

        //Counts are used to count the number of boids included in the calculations
        int separateCount  = 0;
        int alignmentCount = 0;
        int cohesionCount  = 0;

        foreach (var boid in nearbyBoids)
        {
            if (boid == this) // return on self
                return;

            distance = transform.position - boid.transform.position;

            if(distance.magnitude > 0)
            {                
                // Alignment
                if (distance.magnitude < viewRadius)
                {
                    alignment += (Vector2)boid.transform.up;
                    alignmentCount++;
                }
                // Cohesion
                if(distance.magnitude < viewRadius)
                {
                    cohesion += (Vector2)boid.transform.position;
                    cohesion = (cohesion - (Vector2)transform.position); // Calculate the desired velocity to move towards the center of mass
                    cohesionCount++;
                }
                // Separation
                if (distance.magnitude < 1.0f)
                {
                    separation += distance.normalized / distance.magnitude;
                    separateCount++;
                }
            }
            // averages
            if (separateCount > 0)
            {
                separation /= separateCount;
                separation = separation.normalized * separationWeight * rotationSpeed * Time.deltaTime;
            }
            if (alignmentCount > 0)
            {
                alignment /= alignmentCount;
                alignment = alignment.normalized * alignmentWeight * rotationSpeed * Time.deltaTime;

            }
            if (cohesionCount > 0)
            {
                cohesion /= cohesionCount;
                cohesion = cohesion.normalized * cohesionWeight * rotationSpeed * Time.deltaTime;
            }
            transform.up += (((Vector3)separation) + ((Vector3)alignment) + ((Vector3)cohesion));
        }
    }

    void UpdateNearbyBoids()
    {
        List<Boid> boids = BoidController.boids;
        nearbyBoids = new List<Boid>();
        for (int i = 0; i < boids.Count; i++)
        {
            float temp = Vector3.Distance(boids[i].transform.position, transform.position);
            if (temp < viewRadius && temp > 0)
            {
                nearbyBoids.Add(boids[i]);
            }
        }
    }

    void ScreenWrap()
    {
        if (transform.position.x < leftConstraint - buffer)
            transform.position = new Vector3(rightConstraint - 0.10f, transform.position.y, transform.position.z);
        if (transform.position.x > rightConstraint)
            transform.position = new Vector3(leftConstraint, transform.position.y, transform.position.z);
        if (transform.position.y < bottomConstraint - buffer)
            transform.position = new Vector3(transform.position.x, topConstraint + buffer, transform.position.z);
        if (transform.position.y > topConstraint + buffer)
            transform.position = new Vector3(transform.position.x, bottomConstraint - buffer, transform.position.z);
    }


    void CreatePoints()
    {
        float x;
        float y;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * viewRadius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * viewRadius;

            line.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }
    }
}
