using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private float brickHeight = 0.3f;
    [SerializeField] private int defaultBricks = 0;
    [SerializeField] private int speed = 350;

    // Number of Bricks that player has catched
    List<GameObject> listBrick = new List<GameObject>();

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
        for(int i = 1; i <= defaultBricks; ++i)
        {
            AddBrick();
        }

        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(levelManager.IsBlockGesture())
        {
            return;
        }

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
                    // Go North
                    SetDirection((int)Direction.NORTH);
                }
                else
                {
                    SetDirection((int)Direction.SOUTH);
                }
            }
            // Vertical
            else
            {
                // Don't do anything, just keep current directioin
                if(endSwipe.y > startSwipe.y)
                {
                    SetDirection((int)Direction.WEST);
                }
                else
                {

                    SetDirection((int)Direction.EAST);
                }
            }

            startSwipe = new Vector3(0, 0, -1);

            // Move
            SetMove(true);
            levelManager.FindAndSetTarget(this.playerDirection);
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
    private void SetDirection(int direction)
    {
        playerDirection = direction;
        //ForceTurn();
    }
    // Rotate GameObject
    // By definition, we just care about position of y
    private void ForceTurn()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, playerDirection * 90, 0));
    }

    public int GetNumBricks() { return listBrick.Count; }
    public int GetDirection() { return playerDirection; }
    public bool IsMoving() { return isMoving; }
    public int GetSpeed() { return  speed; }
    public void SetMove(bool move)
    {
        this.isMoving = move;
    }
    public void AddBrick()
    {
        listBrick.Add(Instantiate(brickPrefab, 
                                  new Vector3(0, listBrick.Count * brickHeight ,0) + transform.position, 
                                  /*Quaternion.Euler(new Vector3(-90, playerDirection * 90, 180))*/
                                  brickPrefab.transform.rotation, 
                                  transform));
        body.transform.position = (new Vector3(0, (listBrick.Count - 1) * brickHeight, 0)) + transform.position;
    }
    public void RemoveBrick()
    {
        Destroy(listBrick[listBrick.Count - 1]);
        listBrick.RemoveAt(listBrick.Count - 1);
        body.transform.position = (new Vector3(0, (listBrick.Count - 1) * brickHeight, 0)) + transform.position;
    }
    private void ClearBrick()
    {
        while(listBrick.Count > 0)
        {
            RemoveBrick();
        }
    }
}
