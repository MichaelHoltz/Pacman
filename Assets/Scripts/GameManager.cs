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
    [SerializeField] private TextMeshProUGUI _scoreText;

    private float _countdownToStartTimer = 4f;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private TextMeshProUGUI _readyText;

    private bool _gameStarted = false;
    private void Awake()
    {
        _startGame.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameStarted)
        {
            _countdownToStartTimer -= Time.deltaTime;
            if (_countdownToStartTimer <= 0)
            {

                _gameStarted = true;
                _readyText.enabled = false;
                _playerController.StartGame();
                _movementController.StartGame();
                _siren.Play();
            }
        }
    }

    public void AddToScore(int scoreToAdd)
    {
        _score += scoreToAdd;
        _scoreText.text = _score.ToString();
    }

    public void CollectedPellet(NodeController nodeController)
    {
        if (_currentMunch == 0)
        { 
            _munch1.Play();
            _currentMunch = 1;
        }
        else if(_currentMunch == 1)
        {
            _munch2.Play();
            _currentMunch = 0;
        }

        //TODO Add to score
        AddToScore(10);
        //TODO Check if all pellets are collected

        //TODO check how many pellets are left

        //TODO is this a power pellet?
    }
}
