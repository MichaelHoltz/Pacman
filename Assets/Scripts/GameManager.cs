using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject LeftWarpNode;
    public GameObject RightWarpNode;

    public AudioSource siren;     

    private void Awake()
    {
        siren.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
