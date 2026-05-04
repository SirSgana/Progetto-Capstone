using UnityEngine;
using TMPro;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] private int _enemiesRequired = 21;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private TextMeshProUGUI _infoText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Recupero il conteggio dei nemici uccisi dal nuovo Manager
            int currentKills = EnemyDefeatCount.Instance.GetTotalKills();

            if (currentKills >= _enemiesRequired)
            {
                // Vittoria
                _winPanel.SetActive(true);
                _infoPanel.SetActive(false);
                Time.timeScale = 0f;

                // Sblocca il cursore
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Mostra quanti ne mancano
                int missingEnemies = _enemiesRequired - currentKills;
                _infoText.text = $"Hai abbattuto {currentKills} su {_enemiesRequired} nemici";
                _infoPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _infoPanel.SetActive(false);
        }
    }
}