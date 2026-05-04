using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject OptionPanel;

    public void OnPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void ShowOption()
    {
        OptionPanel.SetActive(!OptionPanel.activeSelf);
    }

    public void SaveOption()
    {
        Debug.Log("Le Opzioni sono state salvate");
    }

    public void OnExit()
    {
        Debug.Log("Esci dal gioco");
        Application.Quit();
    }
}
