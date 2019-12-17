using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {        

        if (other.gameObject.TryGetComponent<Rocket> (out Rocket rocket))
        {            
            if (rocket.CurrentFuel + 40 > rocket.MaxFuel)
                rocket.CurrentFuel = rocket.MaxFuel;
            else
                rocket.CurrentFuel += 40f;

            GameObject.Destroy(this.gameObject);
        }
    }
}
