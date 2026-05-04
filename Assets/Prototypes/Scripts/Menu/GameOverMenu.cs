using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private LifeController _playerLife;
    [SerializeField] private GameObject _gameOverPanel;
    public void OnRestart()
    {
        Time.timeScale = 1f;

        // Ripristina la vita
        if (_playerLife != null)
        {
            _playerLife.ResetHp();
        }

        // Nasconde il pannello
        _gameOverPanel.SetActive(false);

        // Nasconde il cursore
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ricarica la scena
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
