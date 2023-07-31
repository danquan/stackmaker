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
        SetMenu();
    }

    public void SetMenu()
    {
        levelManager.PauseGame(true);
        uiManager.SettingOff();
        uiManager.SetWinOff();
        uiManager.SetPanelOff();
        uiManager.SetVictoryOff();
        uiManager.SetMenuOn();
    }

    public void StartGame()
    {
        levelManager.ResetLevel();
        levelManager.PauseGame(false);
        uiManager.SetMenuOff();
        uiManager.SetPanelOn(); // Game Panel on
    }


    public void ContinueGame()
    {
        levelManager.PauseGame(false);
        uiManager.SettingOff();
        uiManager.SetPanelOn();
    }
    public void SettingGame()
    {
        levelManager.PauseGame(true);
        uiManager.SetPanelOff();
        uiManager.SettingOn();
    }

    public void Won()
    {
        levelManager.PauseGame(true);
        uiManager.SetWinOn();
        uiManager.SetPanelOff();
    }
    public void Victory()
    {
        levelManager.PauseGame(true);
        uiManager.SetVictoryOn();
    }

    public void NextLevel()
    {
        levelManager.PauseGame(false);
        uiManager.SetWinOff();
        uiManager.SetPanelOn();
        levelManager.UpLevel();
    }

    public void Retry()
    {
        levelManager.PauseGame(false);
        uiManager.SetWinOff();
        uiManager.SettingOff();
        uiManager.SetPanelOn();
        levelManager.Retry();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
