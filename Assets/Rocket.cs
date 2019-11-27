using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource engineAudio;
    [SerializeField] float fwdThrust = 1f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float fuelLossThrust = 0.1f;
    [SerializeField] float maxFuel = 100f;
    float currentFuel;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        engineAudio = GetComponent<AudioSource>();
        //fuelCounter = GetComponent<Text>();

        currentFuel = maxFuel;
        
    }

    // Update is called once per frame
    void Update()
    {
        Thrust();
        Rotate();        
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Fuel":
                if (currentFuel + 20 > maxFuel)
                    currentFuel = maxFuel;
                else
                    currentFuel += 20f;
                GameObject.Destroy(collision.gameObject);
                break;
            case "Finish":
                print("WIN");

                break;
            default:
                print("dead");
                break;
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space)) //Can thrust while rotating
        {
            if (currentFuel > 0)
            {
                rigidBody.AddRelativeForce(Vector3.up * fwdThrust);
                currentFuel -= fuelLossThrust;
                print(currentFuel);

                if (!engineAudio.isPlaying)
                {
                    engineAudio.Play();
                }
            }
        }
        else
        {
            engineAudio.Stop();
        }
    }
    private void Rotate()
    {
        rigidBody.freezeRotation = true; //take manual control of rotation        
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        //Can only turn one direction
        if (Input.GetKey(KeyCode.A))
        {            
            transform.Rotate(Vector3.forward * rotationThisFrame);            
        }
        else if (Input.GetKey(KeyCode.D))
        {        
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; //resume physics control of rotation
    }
}
