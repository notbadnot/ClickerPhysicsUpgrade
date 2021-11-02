using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OutGameUIManager : MonoBehaviour
{
    [SerializeField] GameObject leaderPrefab;
    private GameMaster gameMaster;
    private MMenuStartScript startMenu;
    private MainMenuScript menu;
    private MMenuSettingsScript settingsMenu;
    public GameModel gameModel;
    private GameModel.Difficulty tempDifficulty;
    private MMenuCredits creditsMenu;
    private MMenuLeaderBoard leaderMenu;
    private GameOverMenuScript gameOverMenu;
    private bool tempCrowding;

    public int tempScore;
    public int tempTime;

    public event Action GameStarted;


    // Start is called before the first frame update
    void Start()
    {
        gameMaster = FindObjectOfType<GameMaster>();
        startMenu = FindObjectOfType<MMenuStartScript>();
        settingsMenu = FindObjectOfType<MMenuSettingsScript>();
        menu = FindObjectOfType<MainMenuScript>();
        creditsMenu = FindObjectOfType<MMenuCredits>();
        leaderMenu = FindObjectOfType<MMenuLeaderBoard>();
        gameOverMenu = FindObjectOfType<GameOverMenuScript>();
        SubscribeToStartEvents();
        settingsMenu.gameObject.SetActive(false);
        creditsMenu.gameObject.SetActive(false);
        leaderMenu.gameObject.SetActive(false);
        gameOverMenu.gameObject.SetActive(false);
        SubscribeToGameOverMenuEvents(); //remove

    }
    private void SubscribeToGameOverMenuEvents()
    {
        gameOverMenu.PressedOkEvent += GameOverMenu_PressedOkEvent;
    }
    private void UnsubscribeToGameOverMenuEvents()
    {
        gameOverMenu.PressedOkEvent -= GameOverMenu_PressedOkEvent;
    }

    private void GameOverMenu_PressedOkEvent(string gettedName)
    {
        AddNewLeader(gettedName, tempScore, tempTime); //remove
        leaderMenu.gameObject.SetActive(true);
        SubscribeToLeaderBoardEvents();
        UnsubscribeToGameOverMenuEvents();
        gameOverMenu.gameObject.SetActive(false);
        BuildLeaderBoard();
    }

    private void SubscribeToLeaderBoardEvents()
    {
        leaderMenu.OkClickedEvent += LeaderMenu_OkClickedEvent;
    }
    private void UnsubscribeToLeaderBoardEvents()
    {
        leaderMenu.OkClickedEvent -= LeaderMenu_OkClickedEvent;
    }


    private void LeaderMenu_OkClickedEvent()
    {
        startMenu.gameObject.SetActive(true);
        SubscribeToStartEvents();
        UnsubscribeToLeaderBoardEvents();
        leaderMenu.gameObject.SetActive(false);
    }

    private void SubscribeToCreditsEvents()
    {
        creditsMenu.OkPressEvent += CreditsMenu_OkPressEvent;
    }
    private void UnsubscribeToCreditsEvents()
    {
        creditsMenu.OkPressEvent -= CreditsMenu_OkPressEvent;
    }

    private void CreditsMenu_OkPressEvent()
    {
        startMenu.gameObject.SetActive(true);
        SubscribeToStartEvents();
        UnsubscribeToCreditsEvents();
        creditsMenu.gameObject.SetActive(false);
    }

    private void SubscribeToStartEvents()
    {
        startMenu.StartEvent += MainMenu_StartEvent;
        startMenu.SettingsEvent += MainMenu_SettingsEvent;
        startMenu.QuitEvent += MainMenu_QuitEvent;
        startMenu.CreditsEvent += StartMenu_CreditsEvent;
        startMenu.LeaderBoardEvent += StartMenu_LeaderBoardEvent;
    }

    private void StartMenu_LeaderBoardEvent()
    {
        leaderMenu.gameObject.SetActive(true);
        SubscribeToLeaderBoardEvents();
        UnsubscribeToStartEvents();
        startMenu.gameObject.SetActive(false);
        BuildLeaderBoard();
    }

    private void StartMenu_CreditsEvent()
    {
        creditsMenu.gameObject.SetActive(true);
        SubscribeToCreditsEvents();
        UnsubscribeToStartEvents();
        startMenu.gameObject.SetActive(false);
    }

    private void MainMenu_QuitEvent()
    {
        QuitHelper.Exit();
    }

    private void UnsubscribeToStartEvents()
    {
        startMenu.StartEvent -= MainMenu_StartEvent;
        startMenu.SettingsEvent -= MainMenu_SettingsEvent;
        startMenu.QuitEvent -= MainMenu_QuitEvent;
        startMenu.CreditsEvent -= StartMenu_CreditsEvent;
        startMenu.LeaderBoardEvent -= StartMenu_LeaderBoardEvent;
    }
    private void SubscribeToSettingsEvents()
    {
        settingsMenu.PressBabyEvent += SettingsMenu_PressBabyEvent;
        settingsMenu.PressHumanEvent += SettingsMenu_PressHumanEvent;
        settingsMenu.PressAlienEvent += SettingsMenu_PressAlienEvent;
        settingsMenu.PressAcceptEvent += SettingsMenu_PressAcceptEvent;
        settingsMenu.PressCancelEvent += SettingsMenu_PressCancelEvent;
        settingsMenu.CrowdingChangeEvent += SettingsMenu_CrowdingChangeEvent;
    }



    private void UnsubscribeToSettingsEvents()
    {
        settingsMenu.PressBabyEvent -= SettingsMenu_PressBabyEvent;
        settingsMenu.PressHumanEvent -= SettingsMenu_PressHumanEvent;
        settingsMenu.PressAlienEvent -= SettingsMenu_PressAlienEvent;
        settingsMenu.PressAcceptEvent -= SettingsMenu_PressAcceptEvent;
        settingsMenu.PressCancelEvent -= SettingsMenu_PressCancelEvent;
        settingsMenu.CrowdingChangeEvent -= SettingsMenu_CrowdingChangeEvent;
    }
    private void SettingsMenu_CrowdingChangeEvent(bool obj)
    {
        tempCrowding = obj;
        HighlitedDifficultyButton().Select();
    }

    private void SettingsMenu_PressCancelEvent()
    {
        startMenu.gameObject.SetActive(true);
        SubscribeToStartEvents();
        UnsubscribeToSettingsEvents();
        settingsMenu.gameObject.SetActive(false);
    }

    private void SettingsMenu_PressAcceptEvent()
    {
        gameModel.difficulty = tempDifficulty;
        gameModel.enableCrowding = tempCrowding;
        startMenu.gameObject.SetActive(true);
        SubscribeToStartEvents();
        UnsubscribeToSettingsEvents();
        settingsMenu.gameObject.SetActive(false);
    }

    private void SettingsMenu_PressAlienEvent()
    {
        tempDifficulty = GameModel.Difficulty.Hard;
    }

    private void SettingsMenu_PressHumanEvent()
    {
        tempDifficulty = GameModel.Difficulty.Medium;
    }

    private void SettingsMenu_PressBabyEvent()
    {
        tempDifficulty = GameModel.Difficulty.Easy;
    }

    private void MainMenu_StartEvent()
    {
        //gameMaster.StartGame();
        UnsubscribeToStartEvents();
        startMenu.gameObject.SetActive(false);
        GameStarted?.Invoke();
    }
    private Button HighlitedDifficultyButton()
    {
        string buttonName = null;
        if (gameModel.difficulty == GameModel.Difficulty.Easy) { buttonName = "BabyButton"; }
        else if (gameModel.difficulty == GameModel.Difficulty.Medium) { buttonName = "HumanButton"; }
        else if (gameModel.difficulty == GameModel.Difficulty.Hard) { buttonName = "AlienButton"; }
        return settingsMenu.gameObject.transform.Find("Panel").Find("ChooseButtons").Find(buttonName).GetComponent<Button>();
    }

    private void MainMenu_SettingsEvent()
    {
        settingsMenu.gameObject.SetActive(true);
        SubscribeToSettingsEvents();
        UnsubscribeToStartEvents();
        startMenu.gameObject.SetActive(false);
        
        HighlitedDifficultyButton().Select();
        tempDifficulty = gameModel.difficulty;

        tempCrowding = gameModel.enableCrowding;
        settingsMenu.gameObject.transform.Find("Panel").Find("Toggle").GetComponent<Toggle>().isOn = tempCrowding;
    }
    private void AddNewLeader(string leaderName, int score, int time)
    {
        string difficultyName;
        if (gameModel.difficulty == GameModel.Difficulty.Easy)
        {
            difficultyName = "Baby";
        }
        else if (gameModel.difficulty == GameModel.Difficulty.Medium)
        {
            difficultyName = "Human";
        }
        else if (gameModel.difficulty == GameModel.Difficulty.Hard)
        {
            difficultyName = "Alien";
        }
        else { difficultyName = "Cheater"; }
        GameModel.Leader newLeader = new GameModel.Leader(leaderName, score, time, difficultyName);
        gameModel.Leaders.Add(newLeader);
        Debug.Log("Leaders count is" + gameModel.Leaders.Count);
        Debug.Log("Between two counts");
        if (gameModel.Leaders.Count > 1)
        {
            gameModel.Leaders.Sort();
        }
        if (gameModel.Leaders.Count > 5)
        {
            gameModel.Leaders.RemoveAt(4);
        }
        Debug.Log("Leaders count is" + gameModel.Leaders.Count);
    }
    private void BuildLeaderBoard()
    {
        var leaderPlace = leaderMenu.transform.Find("Panel").Find("LeaderPanel");

        int leadersToDestroy = leaderPlace.transform.childCount;
        while (leadersToDestroy > 0)
        {
            Destroy( leaderPlace.transform.GetChild(leadersToDestroy - 1).gameObject);
            leadersToDestroy--;

        }

        
        
        foreach (GameModel.Leader item in gameModel.Leaders)
        {
            GameObject newBornLeader = Instantiate(leaderPrefab, leaderPlace);
            newBornLeader.transform.GetChild(0).GetComponent<Text>().text = (gameModel.Leaders.IndexOf(item) + 1).ToString();
            newBornLeader.transform.GetChild(1).GetComponent<Text>().text = item.Name;
            newBornLeader.transform.GetChild(2).GetComponent<Text>().text = item.Score.ToString();
            newBornLeader.transform.GetChild(3).GetComponent<Text>().text = item.Time.ToString();
            newBornLeader.transform.GetChild(4).GetComponent<Text>().text = item.Difficulty.ToString();

        }
        {

        }
    }
    public void SwitchMenu(bool switcher)
    {
        menu.gameObject.SetActive(switcher);
    }
    public void ShowGameOverMenu(int score, int time)
    {
        gameOverMenu.gameObject.SetActive(true);
        string difficultyName;
        if (gameModel.difficulty == GameModel.Difficulty.Easy)
        {
            difficultyName = "Baby";
        }
        else if (gameModel.difficulty == GameModel.Difficulty.Medium)
        {
            difficultyName = "Human";
        }
        else if (gameModel.difficulty == GameModel.Difficulty.Hard)
        {
            difficultyName = "Alien";
        }
        else { difficultyName = "Cheater"; }
        gameOverMenu.transform.Find("Panel").Find("GameoverCanvas").Find("ResultPlace").Find("ResultLable").GetComponent<Text>().text = ("Your Score: " + score + "   Your Time: " +  time + "   Your Difficulty : " + difficultyName);
        SubscribeToGameOverMenuEvents();
        tempScore = score;
        tempTime = time;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
