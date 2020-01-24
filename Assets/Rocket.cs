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

    [Range (0,100)]
    [SerializeField] float maxFuel = 100f; //maximum fuel coefficient

    [SerializeField] float levelLoadDelay;

    bool collisionsDisabled = false;
    bool infiniteFuel = false;
    bool isTransitioning = false;    

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
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        // only if debug on
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
            //toggle collision
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            infiniteFuel = !infiniteFuel;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionsDisabled) { return; }

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
        deathParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        Invoke("LoadFirstLevel", levelLoadDelay); //Better to use coroutines. StartCoroutine("LoadNextScene(1)"); ??WIP 
    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        succeedParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(succeed);
        Invoke("LoadNextLevel", levelLoadDelay); //Better to use coroutines. StartCoroutine("LoadNextScene(1)"); ??WIP 
    }

    private void LoadNextLevel()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        int nextBuildIndex = currentBuildIndex + 1;

        //if next build index is past the level count, go back to level 1
        if (currentBuildIndex == SceneManager.sceneCountInBuildSettings - 1)
            nextBuildIndex = 0;
        else
            nextBuildIndex = currentBuildIndex + 1;


        SceneManager.LoadScene(nextBuildIndex);
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
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngine1Particles.Stop();
        mainEngine2Particles.Stop();
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * fwdThrust * Time.deltaTime);
        
        if (!infiniteFuel)
        {
            CurrentFuel -= fuelLossThrust;
            if (CurrentFuel <= 0)
            {
                StartDeathSequence();
            }
        }
              

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
        if (CurrentFuel > 0)
        {
            //Can only turn one direction
            if (Input.GetKey(KeyCode.A))
            {
                RorateManually(rcsThrust * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                RorateManually(-rcsThrust * Time.deltaTime);
            }
        }
    }

    private void RorateManually(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true; //take manual control of rotation        
        transform.Rotate(Vector3.left * rotationThisFrame);
        rigidBody.freezeRotation = false; //resume physics control of rotation
    }
}
