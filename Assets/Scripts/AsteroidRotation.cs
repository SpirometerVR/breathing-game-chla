using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidRotation : MonoBehaviour
{
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(30, 40); 
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the asteroid at a random speed.
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}
