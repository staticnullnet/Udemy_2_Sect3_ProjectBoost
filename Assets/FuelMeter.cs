using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelMeter : MonoBehaviour
{
    Rocket rocket;
    Text fuelCounter;

    // Start is called before the first frame update
    void Start()
    {
        fuelCounter = GetComponent<Text>();
        rocket = FindObjectOfType<Rocket>();
    }

    // Update is called once per frame
    void Update()
    {
        float fuel = rocket.CurrentFuel;        
        fuelCounter.text = ((int)fuel).ToString();
    }
}
