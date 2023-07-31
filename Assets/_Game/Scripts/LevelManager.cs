using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject[] listLevel;
    [SerializeField] private Player player;

    private bool isPaused = true;
    private int currentLevel = 0;
    private Level level;
    
    public struct Position
    {
        public int x, y;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    private Position playerMapPosition;
    private Position targetMapPos;

    void Start()
    {
        currentLevel = 0;
        for(int i = 0; i < listLevel.Length; i++)
        {
            if(i != currentLevel)
            {
                listLevel[i].SetActive(false);
            }
        }
        OnInit();
    }

    public void OnInit()
    {
        // Clear Player
        player.OnInit();

        listLevel[currentLevel].GetComponent<Level>().OnInit();
        level = listLevel[currentLevel].GetComponent<Level>();

        /*** Make sure that player stand at starting box in new game ***/
        // keep position y of player
        Vector3 startPos = level.GetStartPos();
        player.transform.position = new Vector3(startPos.x, player.transform.position.y, startPos.z);
        
        /*** set player's position on map ***/
        playerMapPosition = new Position(level.GetStart().x, level.GetStart().y);
    }

    public void UpLevel()
    {
        listLevel[currentLevel].SetActive(false);
        ++currentLevel;
        OnInit();
        listLevel[currentLevel].SetActive(true);
    }
    public void Retry()
    {
        listLevel[currentLevel].SetActive(false);
        OnInit();
        listLevel[currentLevel].SetActive(true);
    }
    public void ResetLevel()
    {
        listLevel[currentLevel].SetActive(false);
        currentLevel = 0;
        OnInit();
        listLevel[currentLevel].SetActive(true);
    }

    // private int cnt = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        // Pause Game
        if(IsPaused())
        {
            return;
        }


        if(level.HasBrick(playerMapPosition.x, playerMapPosition.y))
        {
            level.RemoveBrick(playerMapPosition.x, playerMapPosition.y);
            player.AddBrick();
        }   
        else if(level.NeedBrick(playerMapPosition.x, playerMapPosition.y))
        {
            level.AddBrick(playerMapPosition.x, playerMapPosition.y);
            //Debug.Log((cnt++) + " - Current pos: " + playerMapPosition.x + " " + playerMapPosition.y + " " + level.NeedBrick(playerMapPosition.x, playerMapPosition.y));
            player.RemoveBrick();
        }
        // Debug.Log(cnt + " - Current Bricks: " + player.GetNumBricks());

        // Move Player
        if(player.IsMoving())
        {
            // Move in Map
            Position velocity;
            velocity.x = targetMapPos.x > playerMapPosition.x ? 1 : (targetMapPos.x < playerMapPosition.x ? -1 : 0);
            velocity.y = targetMapPos.y > playerMapPosition.y ? 1 : (targetMapPos.y < playerMapPosition.y ? -1 : 0);

            // Change playerMapPos
            if(OnMapDistance(player.transform.position, level.GetPos(playerMapPosition.x + velocity.x, playerMapPosition.y + velocity.y)) < 0.25f)
            {
                playerMapPosition.x += velocity.x;
                playerMapPosition.y += velocity.y;
            }
            
            // Move in Plane
            Vector3 targetPos = level.GetPos(targetMapPos.x, targetMapPos.y);
            targetPos = new Vector3(targetPos.x, player.transform.position.y, targetPos.z);
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, player.GetSpeed() * Time.fixedDeltaTime);

            if(Vector3.Distance(targetPos, player.transform.position) < 0.1f)
            {
                player.SetMove(false);
            }
        }

        // Victory
        if(OnMapDistance(player.transform.position, level.GetFinishPos()) < 0.1f)
        {
            //player.ClearBrick();
            if(currentLevel == listLevel.Length - 1) 
            { 
                gameManager.Victory();
            }
            else
            {
                gameManager.Won();
            }
        }
    }

    // block gesture on UI screen
    public bool IsPaused()
    {
        return isPaused;
    }
    public void PauseGame(bool isPaused)
    {
        this.isPaused = isPaused;
    }
    // This function is to find Target position for MoveToward method
    public void FindAndSetTarget(int direction)
    {
        Position velocity;

        if(direction == (int)Player.Direction.NORTH)
        {
            velocity = new Position(0, 1);
        }
        else if(direction == (int)Player.Direction.SOUTH)
        {
            velocity = new Position(0, -1);
        }
        else if(direction== (int)Player.Direction.EAST)
        {
            velocity = new Position(1, 0);
        }
        else // WEST
        {
            velocity = new Position(-1, 0);
        }

        targetMapPos = playerMapPosition;
        while(level.CanGo(targetMapPos.x + velocity.x, targetMapPos.y + velocity.y))
        {
            targetMapPos.x += velocity.x;
            targetMapPos.y += velocity.y;
        }
        
        if(targetMapPos.x == playerMapPosition.x && targetMapPos.y == playerMapPosition.y)
        {
            player.SetMove(false);
        }
    }

    // Calculate Distance of 2 position on map
    private double OnMapDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));
    }
}
