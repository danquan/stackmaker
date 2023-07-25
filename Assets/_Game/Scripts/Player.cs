using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int defaultBricks = 0;

    private int nBricks;
    // Start is called before the first frame update
    void Start()
    {
        OnInit();
    }

    private void OnInit()
    {
        nBricks = defaultBricks;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Move(Vector3 direction)
    {

    }

    void AddBrick()
    {

    }

    void RemoveBrick()
    {

    }

    void ClearBrick()
    {

    }
}
