using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Level level;
    // Start is called before the first frame update

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
        OnInit();
    }

    void OnInit()
    {
        /*** Make sure that player stand at starting box in new game ***/
        // keep position y of player
        Vector3 startPos = level.GetStartPos();
        player.transform.position = new Vector3(startPos.x, player.transform.position.y, startPos.z);
        
        /*** set player's position on map ***/
        playerMapPosition = new Position(level.GetStart().x, level.GetStart().y);
    }

    private int cnt = 0;
    // Update is called once per frame
    void Update()
    {
        // Move Player
        if(player.IsMoving())
        {
            // Move in Map
            Position velocity;
            velocity.x = targetMapPos.x > playerMapPosition.x ? 1 : (targetMapPos.x < playerMapPosition.x ? -1 : 0);
            velocity.y = targetMapPos.y > playerMapPosition.y ? 1 : (targetMapPos.y < playerMapPosition.y ? -1 : 0);

            // Change playerMapPos
            if(OnMapDistance(player.transform.position, level.GetPos(playerMapPosition.x + velocity.x, playerMapPosition.y + velocity.y)) < 0.1f)
            {
                playerMapPosition.x += velocity.x;
                playerMapPosition.y += velocity.y;

                Debug.Log((++cnt) + " - " + playerMapPosition.x + " " + playerMapPosition.y + ": " + player.GetNumBricks());
                
                if(level.HasBrick(playerMapPosition.x, playerMapPosition.y))
                {
                    level.RemoveBrick(playerMapPosition.x, playerMapPosition.y);
                    player.AddBrick();
                }
                else
                {
                    // Stop Moving if run out of brick
                    if(player.GetNumBricks() == 0)
                    {
                        playerMapPosition.x -= velocity.x;
                        playerMapPosition.y -= velocity.y;
                        player.SetMove(false);
                        return;
                    }

                    level.AddBrick(playerMapPosition.x, playerMapPosition.y);
                    player.RemoveBrick();
                }

                if(playerMapPosition.x == targetMapPos.x &&
                   playerMapPosition.y == targetMapPos.y )
                {
                    player.SetMove(false);
                }
            }
            
            // Move in Plane
            Vector3 targetPos = level.GetPos(targetMapPos.x, targetMapPos.y);
            player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(targetPos.x, player.transform.position.y, targetPos.z), player.GetSpeed() * Time.deltaTime);

        }
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
    }

    // Calculate Distance of 2 position on map
    private double OnMapDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));
    }
}
