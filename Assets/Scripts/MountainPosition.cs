using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainPosition : MonoBehaviour
{
    public GameObject boat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Keep moving the surrounding mountains to match the boat position.
        transform.position = new Vector3(transform.position.x, transform.position.y, boat.transform.position.z);
        
    }
}
