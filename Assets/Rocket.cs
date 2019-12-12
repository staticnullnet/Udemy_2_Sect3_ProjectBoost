using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip succeed;

    [SerializeField] ParticleSystem mainEngine1Particles;
    [SerializeField] ParticleSystem mainEngine2Particles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem succeedParticles;

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
        audioSource = GetComponent<AudioSource>();
        CurrentFuel = MaxFuel;
        mainEngine1Particles.Stop();
        mainEngine2Particles.Stop();

    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
            case "Finish":
                StartSuccessSequence();
                break;
            case "Friendly":
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dieing;
        deathParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        Invoke("LoadFirstLevel", 1f); //Better to use coroutines. StartCoroutine("LoadNextScene(1)"); ??WIP 
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        succeedParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(succeed);
        Invoke("LoadNextLevel", 1f); //Better to use coroutines. StartCoroutine("LoadNextScene(1)"); ??WIP 
    }

    private void LoadNextLevel()
    {        
        SceneManager.LoadScene(1);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space) && CurrentFuel > 0) //Can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngine1Particles.Stop();
            mainEngine2Particles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * fwdThrust);
        CurrentFuel -= fuelLossThrust;

        if (!audioSource.isPlaying)
        {
            //Used to play audio but can create skipping due to start/stopping 
            //Also plays audio source's directly attached clip.
            //engineAudio.Play(); old code. 

            audioSource.PlayOneShot(mainEngine);
        }
        mainEngine1Particles.Play();
        mainEngine2Particles.Play();
    }


    private void RespondToRotateInput()
    {
        if (CurrentFuel <= 0)
        {
            audioSource.Stop();
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
