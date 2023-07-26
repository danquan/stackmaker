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

    // Update is called once per frame
    void Update()
    {
        
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
}
