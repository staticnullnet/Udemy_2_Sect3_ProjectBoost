using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource engineAudio;
    [SerializeField] float fwdThrust = 1f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float fuelLossThrust = 0.05f;
    
    [SerializeField] float maxFuel = 100f;    
    float currentFuel;

    public float CurrentFuel { get => currentFuel; set => currentFuel = value; }
    public float MaxFuel { get => maxFuel; set => maxFuel = value; }


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
            case "Finish":
                print("WIN");
                SceneManager.LoadScene(1);
                break;
            case "Friendly":
                break;
            default:
                SceneManager.LoadScene(0);
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
        if (currentFuel > 0)
        {
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
        else
        {
            engineAudio.Stop();
        }
    }
}
