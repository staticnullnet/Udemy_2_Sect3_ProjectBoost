using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource engineAudio;
    [SerializeField] float fwdThrust = 1f; //Forward Thrust coefficient
    [SerializeField] float rcsThrust = 100f; //Rotation Thrust coefficient
    [SerializeField] float fuelLossThrust = 0.05f; //fuel loss coefficient    
    [SerializeField] float maxFuel = 100f; //maximum fuel coefficient
    State state = State.Alive;
    enum State { Alive, Transcending, Dieing };
    public float CurrentFuel { get; set; }
    public float MaxFuel { get => maxFuel; set => maxFuel = value; }
        
    // Start is called before the first frame update
    void Start()
    {        
        rigidBody = GetComponent<Rigidbody>();
        engineAudio = GetComponent<AudioSource>();
        CurrentFuel = MaxFuel;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {            
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextLevel", 1f); //Better to use coroutines. StartCoroutine("LoadNextScene(1)"); ??WIP 
                break;
            case "Friendly":
                break;
            default:
                state = State.Transcending;
                Invoke("LoadFirstLevel", 1f); //Better to use coroutines. StartCoroutine("LoadNextScene(1)"); ??WIP 
                break;
        }
    }

    private void LoadNextLevel()
    {        
        SceneManager.LoadScene(1);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space) && CurrentFuel > 0) //Can thrust while rotating
        {
            
            rigidBody.AddRelativeForce(Vector3.up * fwdThrust);
            CurrentFuel -= fuelLossThrust;

            if (!engineAudio.isPlaying)
            {
                engineAudio.Play();
            }
        }
        else
        {
            engineAudio..Stop();
        }
    }
    private void Rotate()
    {
        if (CurrentFuel <= 0)
        {
            engineAudio.Stop();
            return;
        }

        rigidBody.freezeRotation = true; //take manual control of rotation        
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        //Can only turn one direction
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.left * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.right * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; //resume physics control of rotation
    }
}
