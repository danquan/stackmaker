using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject Win;
    [SerializeField] private GameObject Victory;

    public void SetMenuOn()
    {
        gameMenu.SetActive(true);
    }
    public void SetMenuOff()
    {
        gameMenu.SetActive(false);
    }
    public void SetVictoryOn()
    {
        Victory.SetActive(true);
    }
    public void SetVictoryOff()
    {
        Victory.SetActive(false);
    }
    public void SetWinOn()
    {
        Win.SetActive(true);
    }
    public void SetWinOff()
    {
        Win.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
