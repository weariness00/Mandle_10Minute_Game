using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
public class ChargerHead : MonoBehaviour
{

    public bool inPort = false;
    // Start is called before the first frame update
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "ChargingPort")
            inPort = true;
    }
    public void OnTriggerExit2D(Collider2D other)
    {

        if (other.gameObject.name == "ChargingPort")
            inPort = false;
    }

}
