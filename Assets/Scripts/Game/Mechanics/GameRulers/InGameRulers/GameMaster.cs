using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameMaster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject[] alienPrefab; // ��������� �������� ��� ��������������� ����������
    [SerializeField] private GameObject heartPrefab; // ������ ��� ��������������� ������
    [SerializeField] private int DefaultmaxAliensNumber = 3;//������������ ���������� ���������� �� ����� / �������� ��������� �������� ����� �������� � ������� ����
    [SerializeField] private int Defaulthealth = 5; //��������
    [SerializeField] private KeyCode pauseKey = KeyCode.P; // ��������� ��� ������ �����
    [SerializeField] private float scoreSpeedParam = 10000f; //�������� ��� ��������� �������� ���� � ����������� �� ����� 
    [SerializeField] private int scoreAlienNumberParam = 500; //�������� ��� ��������� ������������� ���������� ���������� � ����������� �� �����
    [SerializeField] private double alienSpawnChanseParam = 0.9; //�������� ����� ��������� ���������
    [SerializeField] private double heartSpawnChanseParam = 0.9995; // �������� ����� ��������� ������

    enum GameState // ���������� ��� ��������� ����
    {
        Playing,
        Pause,
        GameOver,
        NotinGame
    }
    enum SpeedAffector // ���������� ����������� ��� ������������� �������� ����
    {
        Score,
        Pause,
        Gameover
    }

    public event Action<int,int> gameOvered;

    private int health;
    private int maxAliensNumber;

    private GameState gameState = GameState.NotinGame; //������� ��������� ���� (� �������� , �����, ���������)
    private int score = 0; // ����
    public int totalAlienNumber = 0; // ���������� ���������� � ������ ������ �� �����
    private float gameSpeed = 1; // �������� ����
    private InGameUIManager inGameUI;
    private float playTime = 0;
    private float difficultyParam = 1;
    void Start()
    {
        mainCam = Camera.main;
        Time.timeScale = 0;
        inGameUI = gameObject.GetComponent<InGameUIManager>();
    }
    public void StartGame(GameModel.Difficulty difficulty= GameModel.Difficulty.Medium)
    {
        if (difficulty == GameModel.Difficulty.Easy)
        {
            difficultyParam = 0.3f;
        }else if (difficulty == GameModel.Difficulty.Medium)
        {
            difficultyParam = 1f;
        }else if (difficulty == GameModel.Difficulty.Hard)
        {
            difficultyParam = 3f;
        }
        gameState = GameState.Playing;
        Time.timeScale = 1;
        health = Defaulthealth;
        maxAliensNumber = DefaultmaxAliensNumber;
        score = 0;
        playTime = 0;
        inGameUI.CountScore(score);
        inGameUI.CountHealth(health);
        inGameUI.HideTextLabel();
        StartCoroutine(Timer());
    }

    /*������� �����. 
     * �������� �������, � ������ ��������� ��������� �� ������� �� ������� ����� �������� �������� �� ��� GetShoted, ����� �������� ������� ��������� �����, �� �� ���������� ������� ������� � �������  */
    private void Shoot()
    {
        RaycastHit2D hit = Physics2D.Raycast(mainCam.ScreenToWorldPoint(Input.mousePosition), mainCam.transform.forward);
        if (hit.collider != null)
        {
            GameObject hitGameObj = hit.collider.gameObject;
            if (hitGameObj.GetComponent<ShootableObject>() != null)
            {
                ChangeScore(((int)(hitGameObj.GetComponent<ShootableObject>().GetShoted() * difficultyParam)));
                inGameUI.CountScore(score);
            }



        }
    }

    /*������� ��� ��������� ���������� � �������� ����� 
      ��� ������ ������� ������� ��������� � ������ ����� ����� ������� ��������� � ��������� ����� �� ������
      ���� ������ ����� ��������� �� ��������� ���, ����� ������� ����������, � ��� ����� ���� ������ ����� �� ��������� ������ ����������*/
    private void SpawnAllien(Vector3? whereSpawn = null, int spawnNumber = -1)
    {
        if (spawnNumber < 0 || spawnNumber > alienPrefab.Length - 1)
        {
            spawnNumber = Mathf.RoundToInt(UnityEngine.Random.Range(-0.49999f, alienPrefab.Length - 1 + 0.49999f));
        }
        if (whereSpawn != null)
        {
            Instantiate(alienPrefab[spawnNumber], whereSpawn.Value, Quaternion.identity);
        }
        else
        {
            Vector3 placeToSpawn = mainCam.ScreenToWorldPoint(new Vector3(mainCam.pixelWidth * UnityEngine.Random.Range(0.1f, 0.9f), mainCam.pixelHeight * UnityEngine.Random.Range(0.1f, 0.9f), 0));
            Instantiate(alienPrefab[spawnNumber], new Vector3(placeToSpawn.x, placeToSpawn.y, 0), Quaternion.identity);
        }
    }
    /*������� ��� ��������� ������ � �������� ����� 
      ��� ������ ������� ������� ������ � ������ ����� ����� ������� ������ � ��������� ����� �� ������*/
    private void SpawnHeart(Vector3? whereSpawn = null)
    {
        if (whereSpawn != null)
        {
            Instantiate(heartPrefab, whereSpawn.Value, Quaternion.identity);
        }
        else
        {
            Vector3 placeToSpawn = mainCam.ScreenToWorldPoint(new Vector3(mainCam.pixelWidth * UnityEngine.Random.Range(0.1f, 0.9f), mainCam.pixelHeight * UnityEngine.Random.Range(0.1f, 0.9f), 0));
            Instantiate(heartPrefab, new Vector3(placeToSpawn.x, placeToSpawn.y, 0), Quaternion.identity);
        }
    }
    /*������� ��� ��������� ��������
     ������ ������� ����� ������� �� ����� �������� ���������� ��������
     ���� �������� ���������� <= 0 �� ���������� ��������*/
    public void ChangeHealth(int Amount)
    {
        health += Amount;
        Debug.Log(message: "Health is " + health + " Score is " + score );
        inGameUI.CountHealth(health);
        if (health <= 0)
        {
            Debug.Log("Game over !!!");
            inGameUI.ShowTextLabel("Game Over");
            ChangeGameSpeed(SpeedAffector.Gameover);
            FindObjectsOfType<ShootableObject>();
            foreach (ShootableObject shootable in FindObjectsOfType<ShootableObject>())
            {
                Destroy(shootable.gameObject);
            }
            gameOvered?.Invoke(score, (int)playTime);
        }
    }
    /*������� ��� ��������� �����
      ��� ������������ �������� ����� ����������� ����������� ��������� �������� ����������, � ��� �� �������� ����*/
    private void ChangeScore(int Amount)
    {
        score += Amount;
        ChangeGameSpeed(SpeedAffector.Score);
        maxAliensNumber = Mathf.Max(maxAliensNumber, Mathf.RoundToInt((score / scoreAlienNumberParam)*difficultyParam));
        Debug.Log(message: "Health is " + health + " Score is " + score);
    }
    
    /* ������� ��������� �������� ���� 
       ������������ �������� ������� ������� � ��� ��� �������� �� �������� ����
       ���� - ��������� �������� � ����������� �� �����
       ����� - ���������� ���� ��� ������� �������� �������
       ��������� - ���������� ����*/
    private void ChangeGameSpeed(SpeedAffector speedAffector)
    {
        if (speedAffector == SpeedAffector.Score)
        {
            gameSpeed = Mathf.Min(100, (1f + (score / scoreSpeedParam)*difficultyParam) );
            Time.timeScale = gameSpeed;
        }
        else if (speedAffector == SpeedAffector.Pause)
        {
            if (gameState == GameState.Playing)
            {
                gameState = GameState.Pause;
                Time.timeScale = 0;
                inGameUI.ShowTextLabel("Paused");
                StopCoroutine(Timer());
            }
            else if (gameState == GameState.Pause)
            {
                gameState = GameState.Playing;
                Time.timeScale = gameSpeed;
                inGameUI.HideTextLabel();
                StartCoroutine(Timer());
            }
        }
        else if (speedAffector == SpeedAffector.Gameover)
        {
            gameState = GameState.GameOver;
            Time.timeScale = 0;
            StopCoroutine(Timer());
        }
    }
    public void TelUImanagerToSwitchInGameMenu(bool switcher)
    {
        inGameUI.SwitchInGameMenu(switcher);
    }
    public IEnumerator Timer()
    {
        while (gameState == GameState.Playing)
        {
            playTime = playTime + Time.unscaledDeltaTime;
            inGameUI.CountTime((((int)playTime).ToString()));
            yield return new WaitForEndOfFrame();
        }
    }


    void Update()
    {
        if (gameState != GameState.GameOver && gameState != GameState.NotinGame) //���� ���� �� ����������
        {
            if (Input.GetKeyDown(pauseKey)) //���������� � ������ � �����
            {
                ChangeGameSpeed(SpeedAffector.Pause);
            }
            if (gameState == GameState.Playing) //���� ���� � ��������� ����
            {
                if (Input.GetMouseButtonDown(0)) // ���� ��� ���
                {
                    Shoot();

                }

                if (UnityEngine.Random.value > alienSpawnChanseParam && totalAlienNumber < maxAliensNumber) //��������� ��������� � ��������� ������ � ���� ���������� ��� �� ��������
                {                   
                    SpawnAllien();

                }
                if (UnityEngine.Random.value > heartSpawnChanseParam) // ��������� ������ � ��������� ������
                {
                    SpawnHeart();
                }
            }
        }
    }
}
