using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoControllerInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();

        if(OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
		{
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                print("okay");
                UnityEngine.Debug.Log("Okay");
            }
        }

        
    }
}
