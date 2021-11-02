using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameMaster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject[] alienPrefab; // Несколько префабов для инстанцирования пришельцев
    [SerializeField] private GameObject heartPrefab; // Префаб для инстанцирования сердец
    [SerializeField] private int DefaultmaxAliensNumber = 3;//Максимальное количество прищельцев на сцене / Заданное начальное значение потом вырастет в течении игры
    [SerializeField] private int Defaulthealth = 5; //Здоровье
    [SerializeField] private KeyCode pauseKey = KeyCode.P; // Настройка для кнопки паузы
    [SerializeField] private float scoreSpeedParam = 10000f; //параметр для изменения скорости игры в зависимости от очков 
    [SerializeField] private int scoreAlienNumberParam = 500; //параметр для изменения максимального количества пришельцев в зависимости от очков
    [SerializeField] private double alienSpawnChanseParam = 0.9; //Параметр Шанса появления пришельца
    [SerializeField] private double heartSpawnChanseParam = 0.9995; // Параметр Шанса появления сердца
    [SerializeField] private GameObject alienSquadPrefab;

    enum GameState // Энумерация для состояний игры
    {
        Playing,
        Pause,
        GameOver,
        NotinGame
    }
    enum SpeedAffector // Энумерация необходимая для регулирования скорости игры
    {
        Score,
        Pause,
        Gameover
    }

    public event Action<int,int> gameOvered;

    private int health;
    private int maxAliensNumber;

    private GameState gameState = GameState.NotinGame; //Текущее состояние игры (в процессе , пауза, проигрышь)
    private int score = 0; // Очки
    public int totalAlienNumber = 0; // Количество пришельцев в данный момент на сцене
    private float gameSpeed = 1; // Скорость игры
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

    /*Функция клика. 
     * Вызывает рейкаст, в случае успешного попадения по объекту на который можно кликнуть вызывает на том GetShoted, далее вызывает функцию изменения очков, на то количество которое заддано в объекте  */
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

    /*Функция для появления прешельцев в заданной точке 
      При подаче вектора спавнит прешельца в данной точке иначе спавнит пришельца в случайной точке на экране
      Если подать номер пришельца то заспавнит его, иначе создаст случайного, в том числе если подать номер за пределами списка пришельцев*/
    private void SpawnAllien(Vector3? whereSpawn = null, int spawnNumber = -1)
    {
        GameObject spawnedAlien;
        if (spawnNumber < 0 || spawnNumber > alienPrefab.Length - 1)
        {
            spawnNumber = Mathf.RoundToInt(UnityEngine.Random.Range(-0.49999f, alienPrefab.Length - 1 + 0.49999f));
        }
        if (whereSpawn != null)
        {
            spawnedAlien = Instantiate(alienPrefab[spawnNumber], whereSpawn.Value, Quaternion.identity);
        }
        else
        {
            Vector3 placeToSpawn = mainCam.ScreenToWorldPoint(new Vector3(mainCam.pixelWidth * UnityEngine.Random.Range(0.1f, 0.9f), mainCam.pixelHeight * UnityEngine.Random.Range(0.1f, 0.9f), 0));
            spawnedAlien = Instantiate(alienPrefab[spawnNumber], new Vector3(placeToSpawn.x, placeToSpawn.y, 0), Quaternion.identity);
        }
        totalAlienNumber++;
        spawnedAlien.GetComponent<AlienScript>().ImBangedSelfEvent += GameMaster_ImBangedSelfEvent;
        spawnedAlien.GetComponent<AlienScript>().ImDestroidEvent += GameMaster_ImDestroidEvent;
    }

    private void SpawnAlienSquad(int aliensInSpawnedSquad = 3, Vector3? whereSpawn = null)
    {
        GameObject spawnedAlienSquad;
        alienSquadPrefab.GetComponent<AlienSquadScript>().aliensInSquad = aliensInSpawnedSquad;
        if (whereSpawn != null)
        {
            spawnedAlienSquad = Instantiate(alienSquadPrefab, whereSpawn.Value, Quaternion.identity);
        }
        else
        {
            Vector3 placeToSpawn = mainCam.ScreenToWorldPoint(new Vector3(mainCam.pixelWidth * UnityEngine.Random.Range(0.1f, 0.9f), mainCam.pixelHeight * UnityEngine.Random.Range(0.1f, 0.9f), 0));
            spawnedAlienSquad = Instantiate(alienSquadPrefab, new Vector3(placeToSpawn.x, placeToSpawn.y, 0), Quaternion.identity);
        }
        totalAlienNumber += aliensInSpawnedSquad;
        spawnedAlienSquad.GetComponent<AlienSquadScript>().SquadIsReady += GameMaster_SquadIsReady;
    }

    private void GameMaster_SquadIsReady(GameObject obj)
    {
        for (int alienInSquad = 0; alienInSquad < obj.transform.childCount; alienInSquad++)
        {
            AlienScript alienScriptOfAlienInSquad = obj.transform.GetChild(alienInSquad).GetComponent<AlienScript>();
            alienScriptOfAlienInSquad.ImBangedSelfEvent += GameMaster_ImBangedSelfEvent;
            alienScriptOfAlienInSquad.ImDestroidEvent += GameMaster_ImDestroidEvent;
        }
        obj.GetComponent<AlienSquadScript>().SquadIsReady -= GameMaster_SquadIsReady;
    }

    private void GameMaster_ImDestroidEvent(GameObject obj)
    {
        totalAlienNumber--;
        obj.GetComponent<AlienScript>().ImBangedSelfEvent -= GameMaster_ImBangedSelfEvent;
        obj.GetComponent<AlienScript>().ImDestroidEvent -= GameMaster_ImDestroidEvent;
    }

    private void GameMaster_ImBangedSelfEvent(GameObject obj)
    {
        ChangeHealth(obj.GetComponent<AlienScript>().Damage);
        obj.GetComponent<AlienScript>().ImBangedSelfEvent -= GameMaster_ImBangedSelfEvent;
    }

    /*Функция для появления сердец в заданной точке 
 При подаче вектора спавнит сердце в данной точке иначе спавнит сердце в случайной точке на экране*/
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
    /*Функция для изменения здоровья
     Другие обьекты могут вызвать ее чтобы изменить количество здоровья
     Если здоровье становится <= 0 ты происходит проигрыш*/
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
            foreach (ParticleSystem particle in FindObjectsOfType<ParticleSystem>())
            {
                particle.Stop();
            }
            FindObjectsOfType<ShootableObject>();
            foreach (ShootableObject shootable in FindObjectsOfType<ShootableObject>())
            {
                Destroy(shootable.gameObject);
            }
            foreach (ParticleSystem particle in FindObjectsOfType<ParticleSystem>())
            {
                Destroy(particle.gameObject);
            }
                gameOvered?.Invoke(score, (int)playTime);
        }
    }
    /*Функция для изменения очков
      При определенном значении очков увеличивает максимально возможное значение пришельцев, а так же скорость игры*/
    private void ChangeScore(int Amount)
    {
        score += Amount;
        ChangeGameSpeed(SpeedAffector.Score);
        maxAliensNumber = Mathf.Max(maxAliensNumber, Mathf.RoundToInt((score / scoreAlienNumberParam)*difficultyParam));
        Debug.Log(message: "Health is " + health + " Score is " + score);
    }
    
    /* Функция изменения скорости игры 
       Передаваемый параметр говорит функции о том как повлиять на скорость игры
       Очки - увеличить скорость в зависимости от очков
       Пауза - остановить игру или вернуть скорость обратно
       Проигрышь - остановить игру*/
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
        if (gameState != GameState.GameOver && gameState != GameState.NotinGame) //Если игра не проигранна
        {
            if (Input.GetKeyDown(pauseKey)) //Постановка и снятие с паузы
            {
                ChangeGameSpeed(SpeedAffector.Pause);
            }
            if (gameState == GameState.Playing) //Если игра в состоянии игры
            {
                if (Input.GetMouseButtonDown(0)) // Клик при ЛКМ
                {
                    Shoot();

                }
                //========================================================
                if (Input.GetMouseButtonDown(1)) // Клик при ЛКМ
                {
                    Instantiate(alienSquadPrefab, new Vector3(mainCam.ScreenToWorldPoint(Input.mousePosition).x, mainCam.ScreenToWorldPoint(Input.mousePosition).y, 0), Quaternion.identity);

                }
                //========================================================


                if (UnityEngine.Random.value > alienSpawnChanseParam && totalAlienNumber < maxAliensNumber) //Появление пришельца с некоторым шансом и если пришельцев еще не максимум
                {
                    int tryedAliensInSquad = Mathf.RoundToInt(UnityEngine.Random.Range(2, 5));
                    if (totalAlienNumber + tryedAliensInSquad <= maxAliensNumber && UnityEngine.Random.value < 0.5 * difficultyParam)
                    {
                        SpawnAlienSquad(tryedAliensInSquad);

                    }
                    else
                    {
                        SpawnAllien();
                    }
                }
                if (UnityEngine.Random.value > heartSpawnChanseParam) // Появление сердца с некоторым шансом
                {
                    SpawnHeart();
                }
            }
        }
    }
}
