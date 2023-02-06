using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Camera mainCamera;
    public float parallax_value;
    Vector2 length;

    Vector3 startposition;

    // Start is called before the first frame update
    void Start()
    {
        startposition = transform.position;
        length = GetComponentInChildren<SpriteRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relative_pos = mainCamera.transform.position * parallax_value;
        Vector3 dist = mainCamera.transform.position - relative_pos;
        if (dist.x > startposition.x + length.x)
        {
            startposition.x += length.x;
        }
        if (dist.x < startposition.x - length.x)
        {
            startposition.x -= length.x;
        }
        relative_pos.z = startposition.z;
        transform.position = startposition + relative_pos;
    }
}
