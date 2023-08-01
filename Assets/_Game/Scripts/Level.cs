using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.UIElements;

public class Level : MonoBehaviour
{
    enum PIVOT {
        WALL = 0,
        BRICK = 1,
        UNBRICK = 2,
        NO_PIVOT = 3,
        NEED_BRICK = 4,
        NEED_BRICK_TEMP = 5,
        FILLED_BRICK = 8,
        WIN_POS = 6,
        WIN_POS_TEMP = 7,
        MAX_PIVOT = 9
    };

    [SerializeField] private TextAsset levelMapFile;
    [SerializeField] private GameObject[] pivotPrefabs = new GameObject[(int)PIVOT.MAX_PIVOT];
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private Vector3 defaultBrickPos;

    private int[,] bricksGrid;
    private GameObject[,] grid; // use for saving Brick
    private Vector3 upperLeft;
    int nCol, nRow;

    public struct Position{
        public int x, y;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    private Position startMapPos, finishMapPos;

    void Start()
    {
        OnInit();
    }

    private string ReadLine(ref string s)
    {
        return s;
    }

    private void Resolve(string s, ref int x, ref int y)
    {
        var tmp = s.Split(' ');
        x = y = 0;
        for (int id = 0; id < tmp[0].Length; ++id)
            if (tmp[0][id] != '\n' && tmp[0][id] != ' ' && tmp[0][id] != '\r')
                x = x * 10 + Int32.Parse(tmp[0][id].ToString());
        for (int id = 0; id < tmp[1].Length; ++id)
            if (tmp[1][id] != '\n' && tmp[1][id] != ' ' && tmp[1][id] != '\r')
                y = y * 10 + Int32.Parse(tmp[1][id].ToString());
    }
    // Prepare map
    public void OnInit()
    {
        levelReset();
        //Debug.Log("D" + levelMapFile.name);
        //string tilePath = "Assets/_Game/Tile/" + levelMapFile.name+".txt";

        //StreamReader reader = new StreamReader(levelMapFile.name);
        int cnt = 0;
        var reader = levelMapFile.text.Split('\n');
        string line;

        // Read Size of Grid
        line = ReadLine(ref reader[cnt]); // reader.ReadLine();
        ++cnt;
        Resolve(line, ref nRow, ref nCol);

        // Read Start Position
        line = ReadLine(ref reader[cnt]); // reader.ReadLine();
        ++cnt;
        Resolve(line, ref startMapPos.x, ref startMapPos.y);

        // Read Finish Position
        line = ReadLine(ref reader[cnt]); // reader.ReadLine();
        ++cnt;
        Resolve(line, ref finishMapPos.x, ref finishMapPos.y);

        /*** Read Grid ***/

        // First, alloc memory for array
        bricksGrid = new int[nRow, nCol];
        // Then read matrix
        for(int idRow = 0; idRow < nRow; ++idRow)
        {
            line = ReadLine(ref reader[cnt]); // reader.ReadLine();
            ++cnt;
            for(int id = 0; id < nCol; ++id)
            {
                bricksGrid[idRow, id] = Int32.Parse(line[id].ToString());
            }
        }

        // assign position for upper - left pivot
        upperLeft = new Vector3(-startMapPos.x, 0, -startMapPos.y) + transform.position;
        // then alloc memory for grid
        grid = new GameObject[nRow, nCol];
        // then assign position for all pivot
        for (int idRow = 0; idRow < nRow; ++idRow)
            for (int idCol = 0; idCol < nCol; ++idCol)
                if (bricksGrid[idRow, idCol] == (int)PIVOT.BRICK 
                    || bricksGrid[idRow, idCol] == (int)PIVOT.UNBRICK
                    || bricksGrid[idRow, idCol] == (int)PIVOT.WALL)
                {
                    CreateObject(pivotPrefabs[bricksGrid[idRow, idCol]], idRow, idCol);
                }
                else if (bricksGrid[idRow, idCol] == (int)PIVOT.NEED_BRICK)
                {
                    if(idRow > 0 && bricksGrid[idRow - 1, idCol] == (int)PIVOT.NEED_BRICK_TEMP)
                        CreateObject(pivotPrefabs[bricksGrid[idRow, idCol]], idRow, idCol);
                    else
                        CreateObject(pivotPrefabs[bricksGrid[idRow, idCol]], idRow, idCol, Quaternion.Euler(new Vector3(0, 90, 0)));
                }
                else if (bricksGrid[idRow, idCol] == (int)PIVOT.WIN_POS)
                {
                    CreateObject(pivotPrefabs[bricksGrid[idRow, idCol]], idRow, idCol);
                }
    }
    public void levelReset()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        grid = null;
        bricksGrid = null;
    }

    public int GetNumRow() { return nRow;}
    public int GetNumCol() { return nCol;}
    public Position GetStart() { return startMapPos; }
    public Position GetFinish() { return finishMapPos; }
    public bool CanGo(int x, int y)
    {
        return x >= 0 && x < nRow && y >= 0 && y < nCol 
            && bricksGrid[x, y] != (int)PIVOT.WALL
            && bricksGrid[x, y] != (int)PIVOT.NO_PIVOT
            && bricksGrid[x, y] != (int)PIVOT.NEED_BRICK_TEMP
            && bricksGrid[x, y] != (int)PIVOT.WIN_POS_TEMP;
    }
    public bool HasBrick(int x, int y)
    {
        return x >= 0 && x < nRow && y >= 0 && y < nCol && bricksGrid[x, y] == (int)PIVOT.BRICK;
    }
    public bool NeedBrick(int x, int y)
    {
        return x >= 0 && x < nRow && y >= 0 && y < nCol && bricksGrid[x, y] == (int)PIVOT.NEED_BRICK;
    }

    // Get position of cell (posX, posY) in matrix 
    public Vector3 GetPos(int posX, int posY)
    {
        // y in this scripts is z in gameSpace
        return new Vector3(posX, 0, posY) + upperLeft;
    }
    
    // Get positioin of Starting point
    public Vector3 GetStartPos()
    {
        return GetPos(startMapPos.x, startMapPos.y);
    }
    public Vector3 GetFinishPos()
    {
        return GetPos(finishMapPos.x, finishMapPos.y);
    }

    private void CreateObject(GameObject prefabs, int posX, int posY, Quaternion rotation, Vector3 position)
    {
        grid[posX, posY] = Instantiate(prefabs, 
                                        GetPos(posX, posY) + position, 
                                        rotation * prefabs.gameObject.transform.rotation, // use prefab's rotation
                                        transform);
    }
    private void CreateObject(GameObject prefabs, int posX, int posY)
    {
        CreateObject(prefabs, posX, posY, Quaternion.identity, prefabs.transform.position);
    }
    private void CreateObject(GameObject prefabs, int posX, int posY, Quaternion rotation)
    {
        CreateObject(prefabs, posX, posY, rotation, prefabs.transform.position);
    }

    public void RemoveBrick(int posX, int posY)
    {
        //grid[posX, posY].GetComponent<Pivot>().Remove();
        Destroy(grid[posX, posY].gameObject);
        grid[posX, posY] = null;

        bricksGrid[posX, posY] = (int)PIVOT.UNBRICK;
        CreateObject(pivotPrefabs[(int)PIVOT.UNBRICK], posX, posY);
    }

    //int cnt = 0;
    public void AddBrick(int posX, int posY)
    {
        //grid[posX, posY].GetComponent<Pivot>().Remove();
        //Destroy(grid[posX, posY].gameObject);
        //grid[posX, posY] = null;

        //bricksGrid[posX, posY] = (int)PIVOT.BRICK;
        bricksGrid[posX, posY] = (int)PIVOT.FILLED_BRICK;
        //Debug.Log((cnt++) + " " + (bricksGrid[posX, posY] == (int)PIVOT.NEED_BRICK_TEMP) + " hehe");
        CreateObject(brickPrefab, posX, posY, Quaternion.identity, defaultBrickPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
