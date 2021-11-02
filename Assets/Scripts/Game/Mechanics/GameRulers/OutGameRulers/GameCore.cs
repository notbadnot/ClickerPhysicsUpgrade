using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    private GameMaster gameMaster;
    private GameModel GameModel;
    private OutGameUIManager outGameUI;
    private GameModel gameModel;
    private GameModel.Difficulty difficulty;
    // Start is called before the first frame update
    void Start()
    {
        gameMaster = FindObjectOfType<GameMaster>();
        gameModel = new GameModel();
        outGameUI = FindObjectOfType<OutGameUIManager>();
        outGameUI.gameModel = gameModel;
        gameMaster.gameOvered += GameMaster_gameOvered;
        outGameUI.GameStarted += OutGameUI_GameStarted;
    }

    private void OutGameUI_GameStarted()
    {
        gameMaster.StartGame(gameModel.enableCrowding, gameModel.difficulty);
        gameMaster.TelUImanagerToSwitchInGameMenu(true);
        outGameUI.SwitchMenu(false);
    }

    private void GameMaster_gameOvered(int score, int time)
    {
        outGameUI.SwitchMenu(true);
        gameMaster.TelUImanagerToSwitchInGameMenu(false);
        outGameUI.ShowGameOverMenu(score, time);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
