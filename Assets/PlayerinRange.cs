using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerinRange : MonoBehaviour
{
    public bool playerinRange = false;

    // Start is called before the first frame update
    void Start()
    {
        // Time.timeScale = 2;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool PlayerInRange()
    {
        var p = playerinRange;
        playerinRange=false;
        Debug.Log("Hey!");
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

    // void OnTriggerExit(Collider other)
    // {
    //     try
    //     {
    //         if (other.transform.parent.name == "Player")
    //         {
    //             playerinRange = false;
    //         }
    //     }
    //     catch
    //     {
    //     }
    // }
}