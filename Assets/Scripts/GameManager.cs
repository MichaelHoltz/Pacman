using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject LeftWarpNode;
    public GameObject RightWarpNode;

    [SerializeField] private AudioSource _startGame;
    [SerializeField] private AudioSource _siren;
    [SerializeField] private AudioSource _munch1;
    [SerializeField] private AudioSource _munch2;
    [SerializeField] private AudioSource _death;
    [SerializeField] private AudioSource powerPelletAudio;
    private int _currentMunch = 0;
    private int _score = 0;
    private int _totalPellets = 0;
    private int _pelletsLeft = 0;
    private int _pelletsCollectedOnThisLife = 0;
    private bool _hadDeathOnThisLevel = false;


    [SerializeField] private TextMeshProUGUI _scoreText;

    private float _countdownToStartTimer = 4f;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private TextMeshProUGUI _readyText;

    [SerializeField] private GameObject _ghostNodeStart;
    public GameObject GhostNodeCenter;
    [SerializeField] private GameObject _ghostNodeLeft;
    [SerializeField] private GameObject _ghostNodeRight;

    public GameObject Blinky;
    public EnemyController BlinkyController;
    public GameObject Pinky;
    public EnemyController PinkyController;
    public GameObject Inky;
    public EnemyController InkyController;
    public GameObject Clyde;
    public EnemyController ClydeController;

    public GameObject Pacman;
    //private bool _gameStarted = false;

    public event EventHandler OnNewGame;
    public event EventHandler OnGameStart;
    public event EventHandler OnClearedLevel;

    public List<NodeController> NodeControllers = new List<NodeController>();
    public bool newGame;
    public bool clearedLevel;
    public bool gameIsRunning;

    public int Lives;
    public int currentLevel;
    [SerializeField] private TextMeshProUGUI _gameOverText;

    [SerializeField] private Image _blackBackground;
    [SerializeField] private bool _testLevelCleared = false;
    public int[] ghostModeTimers = new int[] { 7, 20, 7, 20, 5, 20, 5 };
    public int ghostModeTimerIndex;
    public float ghostModeTimer;
    public bool runningTimer;
    public bool completedTimer;

    public bool IsPowerPelletRunning = false;
    private float _currentPowerPelletTime = 0;
    public float PowerPelletTimer = 8f;
    public int powerPelletMultiplier = 1;
    public enum GhostMode
    {
        Chase,
        Scatter
    }

    public GhostMode CurrentGhostMode;
    private void Awake()
    {
        newGame = true;
        clearedLevel = false;
        _blackBackground.enabled = false;

        BlinkyController = Blinky.GetComponent<EnemyController>();
        PinkyController = Pinky.GetComponent<EnemyController>();
        InkyController = Inky.GetComponent<EnemyController>();
        ClydeController = Clyde.GetComponent<EnemyController>();
        
        StartCoroutine(Setup());

    }
    private IEnumerator Setup()
    {
        ghostModeTimerIndex = 0;
        runningTimer = true; 
        ghostModeTimer = 0;
        completedTimer = false;
        
        _gameOverText.enabled = false;
        
        //if pacman clears a level, a backgrouw will appear covering the level, and the game will pause for 0.1 seconds.
        if (clearedLevel)
        {
            _blackBackground.enabled = true;
            //Activate background
            yield return new WaitForSeconds(0.1f);
            OnClearedLevel?.Invoke(this, EventArgs.Empty);
            _pelletsLeft = _totalPellets;
        }
        _blackBackground.enabled = false;

        

        _pelletsCollectedOnThisLife = 0;
        CurrentGhostMode = GhostMode.Scatter;
        gameIsRunning = false;

        yield return new WaitForSeconds(0.01f);
        _countdownToStartTimer = 1f;
        if (newGame)
        {
            _countdownToStartTimer = 4f;
            OnNewGame?.Invoke(this, EventArgs.Empty);
            _startGame.Play();
            _score = 0;
            _scoreText.text = _score.ToString();
            _currentMunch = 0;
            Lives = 3;
            currentLevel = 1;
            yield return new WaitForSeconds(0.01f);
        }
        if (clearedLevel || newGame)
        {
            
            //Pellet Respawn when new game or cleared level
            foreach (NodeController nodeController in NodeControllers)
            {
                nodeController.RespawnPellet();
            }
        }

        Pacman.GetComponent<PlayerController>().Setup();
        BlinkyController.Setup();
        PinkyController.Setup();
        InkyController.Setup();
        ClydeController.Setup();

        newGame = false;
        clearedLevel = false;

        yield return new WaitForSeconds(_countdownToStartTimer);
        startGame();
    }
    private void startGame()
    {
         gameIsRunning = true;   
        _readyText.enabled = false;

        _playerController.StartGame();
        _movementController.StartGame();
        _siren.Play();
        
        OnGameStart?.Invoke(this, EventArgs.Empty);
    }
    private void stopGame()
    {

        _playerController.StopGame();
        BlinkyController.StopGame();
        PinkyController.StopGame();
        InkyController.StopGame();
        ClydeController.StopGame();
        gameIsRunning = false;
        _siren.Stop();
    }
    private void Start()
    {
        _playerController.Setup();
        
    }
    // Update is called once per frame
    void Update()
    {
        if (!gameIsRunning)
        {
            return;
        }
        if (!completedTimer && runningTimer)
        {
            ghostModeTimer += Time.deltaTime;
            if (ghostModeTimer >= ghostModeTimers[ghostModeTimerIndex])
            {
                ghostModeTimer = 0;
                ghostModeTimerIndex++;
                if(CurrentGhostMode == GhostMode.Chase)
                {
                    CurrentGhostMode = GhostMode.Scatter;
                }
                else
                {
                    CurrentGhostMode = GhostMode.Chase;
                }
                if(ghostModeTimerIndex == ghostModeTimers.Length)
                {
                    completedTimer = true;
                    runningTimer = false;
                    CurrentGhostMode = GhostMode.Chase;
                }
            }
        }
        if (IsPowerPelletRunning)
        { 
            _currentPowerPelletTime += Time.deltaTime;
            if (_currentPowerPelletTime >= PowerPelletTimer)
            { 
                _currentPowerPelletTime = 0;
                powerPelletAudio.Stop();
                _siren.Play();
                powerPelletMultiplier = 1;

            }
        }
    }
    public void GotPelletFromNodeController(NodeController nodeController)
    {
        NodeControllers.Add(nodeController);    
        _totalPellets++;
        _pelletsLeft++;
    }
    public void AddToScore(int scoreToAdd)
    {
        _score += scoreToAdd;
        _scoreText.text = _score.ToString();
    }

    public IEnumerator CollectedPellet(NodeController nodeController)
    {
        if (_currentMunch == 0)
        {
            _munch1.Play();
            _currentMunch = 1;
        }
        else if (_currentMunch == 1)
        {
            _munch2.Play();
            _currentMunch = 0;
        }


        _pelletsLeft--;
        _pelletsCollectedOnThisLife++;

        int requiredInkyPellets = 0;
        int requiredClydePellets = 0;
        if (_hadDeathOnThisLevel)
        {
            requiredInkyPellets = 12;
            requiredClydePellets = 32;
        }
        else
        {
            requiredInkyPellets = 30;
            requiredClydePellets = 60;
        }

        if (_pelletsCollectedOnThisLife >= requiredInkyPellets && !Inky.GetComponent<EnemyController>().LeftHomeBefore)
        {
            Inky.GetComponent<EnemyController>().ReadyToLeaveHome = true;
        }
        if (_pelletsCollectedOnThisLife >= requiredClydePellets && !Clyde.GetComponent<EnemyController>().LeftHomeBefore)
        {
            Clyde.GetComponent<EnemyController>().ReadyToLeaveHome = true;
        }

        //Add to score
        AddToScore(10);
        //TODO Check if all pellets are collected
        if (_pelletsLeft == 0 || _testLevelCleared)
        {
            _testLevelCleared = false;
            currentLevel++;
            clearedLevel = true;
            stopGame();
            yield return new WaitForSeconds(1);
            StartCoroutine(Setup());

        }
        //TODO check how many pellets are left

        //TODO is this a power pellet?
        if (nodeController.IsPowerPellet)
        {
            _siren.Stop();
            powerPelletAudio.Play();
            IsPowerPelletRunning = true;
            _currentPowerPelletTime = 0;

            BlinkyController.SetFrightened(true);
            PinkyController.SetFrightened(true);
            InkyController.SetFrightened(true);
            ClydeController.SetFrightened(true);

        }
    }

    public IEnumerator PlayerEaten()
    {
        _hadDeathOnThisLevel = true;
        stopGame();
        yield return new WaitForSeconds(1);
        BlinkyController.SetVisible(false);
        PinkyController.SetVisible(false);
        InkyController.SetVisible(false);
        ClydeController.SetVisible(false);
        Pacman.GetComponent<PlayerController>().Death();
        _death.Play();
        yield return new WaitForSeconds(3);
        
        Lives--;
        if(Lives <= 0)
        {
            newGame = true;
            //Display Game Over Text
            _gameOverText.enabled = true;
            yield return new WaitForSeconds(3);
        }
        StartCoroutine(Setup());
    }
}
