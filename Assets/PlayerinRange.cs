using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerinRange : MonoBehaviour
{
    public bool playerinRange = false;

    
    void Start()
    {
        
    }

   
    void Update()
    {
    }

    public bool PlayerInRange()
    {
        var p = playerinRange;
        playerinRange=false;
        Debug.Log("Hello!");
        return p;
    }

    void OnTriggerEnter(Collider other)
    {
        try
        {
            
            playerinRange = (other.transform.parent.name == "Player");
            Debug.Log(playerinRange);
            if (other.transform.parent.name == "Player")
                playerinRange = true;
        }
        catch
        {
        }
    }

}
