using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);

    [Min(0.0001f)]
    [SerializeField] float period = 2f;
       
    float movementFactor;
    
    Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {

        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        //check if two floats are equal
        //Mathf.Abs(floatA - floabB) <= Mathf.Epsilon;


        //if (period <= Mathf.Epsilon) { return; }
        //todo protect against divide by 0
        
        float cycles = Time.time / period;
        
        const float tau = Mathf.PI * 2; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
