using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource engineAudio;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        engineAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.Space)) //Can thrust while rotating
        {
            rigidBody.AddRelativeForce(Vector3.up);
            if (!engineAudio.isPlaying)
            {
                engineAudio.Play();
            }
        }
        else
        {
            engineAudio.Stop();
        }
        
        //Can only turn one direction
        if (Input.GetKey(KeyCode.A))
        {
            print("ROTATE LEFT");
            //rigidBody.MoveRotation(Quaternion.AngleAxis(10, Vector3.zero));
            

            
        }
        else if (Input.GetKey(KeyCode.D))
        {
            print("ROTATE RIGHT");
        }
    }
}
