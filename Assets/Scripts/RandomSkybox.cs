using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkybox : MonoBehaviour
{
    public Material[] skyBox;

    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = skyBox[Random.Range(0, skyBox.Length)];
    }

    void Update() {}

}