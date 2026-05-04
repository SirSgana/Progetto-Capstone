using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LifeController _playerLife;
    [SerializeField] private GameObject _gameOverPanel;

    private bool _isGameOver = false;

    private void Update()
    {
        if (_isGameOver) return;

        if (_playerLife != null && _playerLife.GetHp() <= 0)
        {
            TriggerGameOver();
        }
    }

    //Public perchè presa anche dal TimeManager
    public void TriggerGameOver()
    {
        _isGameOver = true;
        _gameOverPanel.SetActive(true);

        //Ferma il gioco ed attiva il mouse 
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}