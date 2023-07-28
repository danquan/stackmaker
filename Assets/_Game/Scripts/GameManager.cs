using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private UIManager uiManager;
    // Start is called before the first frame update
    void Start()
    {
        OnInit();
    }

    void OnInit()
    {
        levelManager.SetBlockGesture(true);
        uiManager.SetMenuOn();
        uiManager.SetWinOff();
        uiManager.SetVictoryOff();
    }

    public void StartGame()
    {
        levelManager.SetBlockGesture(false);
        uiManager.SetMenuOff();
    }

    public void Won()
    {
        levelManager.SetBlockGesture(true);
        uiManager.SetWinOn();
    }
    public void Victory()
    {
        levelManager.SetBlockGesture(true);
        uiManager.SetVictoryOn();
    }


    public void NextLevel()
    {
        levelManager.SetBlockGesture(false);
        uiManager.SetWinOff();
        levelManager.UpLevel();
    }

    public void Retry()
    {
        levelManager.SetBlockGesture(false);
        uiManager.SetWinOff();
        levelManager.Retry();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
