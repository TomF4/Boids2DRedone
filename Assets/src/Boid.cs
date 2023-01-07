using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color color = new Color(1, 1, 0.5f, 1);
    
    private float speed = 6.0f;
    float rotationSpeed = 2.5f;
    float viewRadius = 10.5f;
    float separationDistance = 4.0f;
    float alignmentDistance = 5.0f;

    Vector2 Velocity;

    List<Boid> nearbyBoids;
    float separationWeight = 10.0f;
    float cohesionWeight = 1.0f;
    float alignmentWeight = 1.0f;

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
        
        transform.localScale = new Vector3(0.25f,0.25f,0.25f);
        transform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        
        Camera cam = Camera.main;
        float distanceZ = Mathf.Abs(cam.transform.position.z + transform.position.z);
        leftConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).x;
        rightConstraint = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, distanceZ)).x;
        bottomConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).y;
        topConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, distanceZ)).y;
        nearbyBoids = new List<Boid>();

        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = (segments + 1);
        line.useWorldSpace = false;
        CreatePoints();
    }

    void Update()
    {
        ScreenWrap();
        UpdateBoid();
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void UpdateBoid()
    {
        UpdateNearbyBoids();
        Vector2 separation = Vector2.zero;
        Vector2 alignment  = Vector2.zero;
        Vector2 cohesion   = Vector2.zero;
        
        Vector2 distance = Vector2.zero;

        //Counts are used to count the number of boids included in the calculations
        int separateCount  = 0;
        int alignmentCount = 0;
        int cohesionCount  = 0;

        if (nearbyBoids != null && nearbyBoids.Any())
        {
            foreach (var boid in nearbyBoids)
            {
                if (boid == this) // return on self
                    return;

                distance = transform.position - boid.transform.position;

                if(distance.magnitude > 0)
                {
                    // Cohesion
                    if(distance.magnitude < 4)
                    {
                        cohesion += (Vector2)boid.transform.position;
                        cohesionCount++;
                        //Debug.DrawLine(transform.position, boid.transform.position, Color.green);
                    }
                    // Alignment
                    if (distance.magnitude < 3)
                    {
                        alignment += (Vector2)boid.transform.up;
                        alignmentCount++;
                        //Debug.DrawLine(transform.position, boid.transform.position, Color.blue);
                    }
                    // Separation
                    if (distance.magnitude < 2)
                    {
                        separation += distance.normalized / distance.magnitude;
                        separateCount++;
                        //Debug.DrawLine(transform.position, boid.transform.position, Color.red);
                    }
                }
            }
            // separate average
            if (separateCount > 0)
            {
                separation /= separateCount;
            }
            // alignment average
            if (alignmentCount > 0)
            {
                alignment /= alignmentCount;
            }
            // cohesion average
            if (cohesionCount > 0)
            {
                cohesion /= cohesionCount;
            }
            transform.up += (((Vector3)separation * separationWeight) + ((Vector3)alignment * alignmentWeight) + ((Vector3)cohesion * cohesionWeight)) * rotationSpeed * Time.deltaTime;
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
