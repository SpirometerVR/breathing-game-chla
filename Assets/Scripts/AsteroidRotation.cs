using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidRotation : MonoBehaviour
{
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(50, 70); 
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the asteroid at a random speed.
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);

        // Rotate on second axis.
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
