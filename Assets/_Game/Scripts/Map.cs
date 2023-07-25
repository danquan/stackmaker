using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class Map : MonoBehaviour
{
    enum PIVOT{
        WALL = 0,
        BRICK = 1,
        UNBRICK = 2,
        MAX_PIVOT = 3
    };

    [SerializeField] private GameObject[] pivotPrefabs = new GameObject[(int)PIVOT.MAX_PIVOT];

    private int[,] bricksGrid;
    private GameObject[,] grid; // use for saving Brick
    private Vector3 upperLeft;
    int nCol, nRow;

    void Start()
    {
        OnInit();
    }

    // Prepare map
    void OnInit()
    {
        string tilePath = @"Assets/_Game/Tile/level-1.txt";
        StreamReader reader = new StreamReader(tilePath);

        string line;
        line = reader.ReadLine();

        nRow = nCol = 0;

        for(int id = 0; id < line.Length; ++id)
            if (line[id] == ' ' && nCol != 0)
            {
                nRow = nCol;
                nCol = 0;
            }
            else if (line[id] != ' ')
            {
                nCol = nCol * 10 + Int32.Parse(line[id].ToString());
            }

        //Debug.Log(nRow + " " + nCol);
        bricksGrid = new int[nRow, nCol];

        for(int idRow = 0; idRow < nRow; ++idRow)
        {
            line = reader.ReadLine();
            for(int id = 0; id < nCol; ++id)
            {
                bricksGrid[idRow, id] = Int32.Parse(line[id].ToString());
            }
        }

        upperLeft = new Vector3(0, 0, - ((nCol + 1) / 2 - 1)) + transform.position;
        grid = new GameObject[nRow, nCol];

        //Debug.Log("Run here");

        for (int idRow = 0; idRow < nRow; ++idRow)
            for (int idCol = 0; idCol < nCol; ++idCol)
                CreatePivot(pivotPrefabs[bricksGrid[idRow, idCol]], idRow, idCol);
    }

    public int GetNumRow()
    {
        return nRow;
    }

    public int GetNumCol()
    {
        return nCol;
    }

    private Vector3 GetPos(int posX, int posY)
    {
        // y in this scripts is z in gameSpace
        return new Vector3(posX, 0, posY) + upperLeft;
    }

    private void CreatePivot(GameObject prefabs, int posX, int posY)
    {
        grid[posX, posY] = Instantiate(prefabs, 
                                        GetPos(posX, posY), 
                                        prefabs.gameObject.transform.rotation, // use prefab's rotation
                                        transform);
    }

    public void RemoveBrick(int posX, int posY)
    {
        grid[posX, posY].GetComponent<Pivot>().Remove();
        Destroy(grid[posX, posY]);
        grid[posX, posY] = null;

        CreatePivot(pivotPrefabs[(int)PIVOT.UNBRICK], posX, posY);
    }

    public void AddBrick(int posX, int posY)
    {
        grid[posX, posY].GetComponent<Pivot>().Remove();
        Destroy(grid[posX, posY]);
        grid[posX, posY] = null;

        CreatePivot(pivotPrefabs[(int)PIVOT.BRICK], posX, posY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
