using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void PauseEvent();

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public GameObject pauseMenuUI;       
    public static bool isPaused = false; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            
            transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            
            Destroy(gameObject);
        }
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // --- LOGICA DEI TASTI ---

    public void Resume()
    {
        pauseMenuUI.SetActive(false); 
        Time.timeScale = 1f;         
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);  
        Time.timeScale = 0f;          
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitLevel()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        isPaused = false;
        SceneManager.LoadScene("MainLevel 1");
    }

    public void QuitGame()
    {
        Debug.Log("Uscita dal gioco...");
        pauseMenuUI.SetActive(false);
        isPaused = false;
        Application.Quit();
    }
}