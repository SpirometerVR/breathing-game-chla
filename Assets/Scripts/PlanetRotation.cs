using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(1, 5);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the planets at a random speed.
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}
