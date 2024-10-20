using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.CullingGroup;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject LeftWarpNode;
    public GameObject RightWarpNode;

    [SerializeField] private AudioSource _startGame;
    [SerializeField] private AudioSource _siren;
    [SerializeField] private AudioSource _munch1;
    [SerializeField] private AudioSource _munch2;
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

    [SerializeField] private Image _blackBackground;
    [SerializeField] private bool _testLevelCleared = false;

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
        //if pacman clears a level, a backgrouw will appear covering the level, and the game will pause for 0.1 seconds.
        if (clearedLevel)
        {
            _blackBackground.enabled = true;
            //Activate background
            yield return new WaitForSeconds(0.1f);
            OnClearedLevel?.Invoke(this, EventArgs.Empty);
        }
        _blackBackground.enabled = false;

        _pelletsLeft = _totalPellets;

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
        //BlinkyController.StopGame();
        //PinkyController.StopGame();
        //InkyController.StopGame();
        //ClydeController.StopGame();
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
    }
}
