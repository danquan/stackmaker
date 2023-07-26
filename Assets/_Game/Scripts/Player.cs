using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private int defaultBricks = 0;

    // Number of Bricks that player has catched
    private int nBricks;

    // Game Manager
    private Vector3 startSwipe,
                    endSwipe;
    
    public enum Direction
    {
        /*
          0
         3 1
          2
        */
        NORTH = 0, 
        EAST = 1, 
        SOUTH = 2, 
        WEST = 3
    }
    private int playerDirection;
    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        OnInit();
    }

    private void OnInit()
    {
        nBricks = defaultBricks;
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Don't process anything while moving
        if (IsMoving())
        {
            return;
        }

        // Mouse Down
        if(Input.GetMouseButtonDown(0))
        {
            startSwipe = Input.mousePosition;
        }

        // Mouse Up
        if(Input.GetMouseButtonUp(0))
        {
            // Make sure that player swipe after moving
            if (Mathf.Abs(startSwipe.z - (-1)) < 0.05f)
            {
                return;
            }

            endSwipe = Input.mousePosition;

            // Horizontal
            if (Mathf.Abs(startSwipe.x - endSwipe.x) > Mathf.Abs(startSwipe.y - endSwipe.y))
            {
                if(endSwipe.x > startSwipe.x)
                {
                    TurnRight();
                }
                else
                {
                    TurnLeft();
                }
            }
            // Vertical
            else
            {
                // Don't do anything, just keep current directioin
                if(endSwipe.y > startSwipe.y)
                {
                    
                }
                else
                {
                    TurnBack();
                }
            }

            startSwipe = new Vector3(0, 0, -1);

            // Move
            levelManager.FindAndSetTarget(this.playerDirection);
            SetMove(true);
        }
    }

    /*
     Turn Right => Increase Direction
     Turn Left => Turn Right 3 times
     Turn Back => Turn Right 2 times
      0
     3 1
      2
    */
    private void TurnRight()
    {
        playerDirection = (playerDirection + 1) % 4;
        ForceTurn();
    }
    private void TurnLeft()
    {
        playerDirection = (playerDirection + 3) % 4;
        ForceTurn();
    }
    private void TurnBack()
    {
        playerDirection = (playerDirection + 2) % 4;
        ForceTurn();
    }
    // Rotate GameObject
    // By definition, we just care about position of y
    private void ForceTurn()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, playerDirection * 90, 0));
    }

    public void SetMove(bool move)
    {
        this.isMoving = move;
    }
    public bool IsMoving()
    {
        return isMoving;
    }
    public void AddBrick()
    {
        ++nBricks;
    }
    public void RemoveBrick()
    {
        --nBricks;
    }
    private void ClearBrick()
    {
        nBricks = 0;
    }
}
