using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerInRange;
    private GameInput gameInput;

    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);
        gameInput = new GameInput();
    }

    // FONDAMENTALE: Abilita l'input system
    private void OnEnable()
    {
        gameInput.Enable();
    }

    private void OnDisable()
    {
        gameInput.Disable();
    }

    private void Update()
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            visualCue.SetActive(true);

            if (gameInput.Player.Interact.WasPressedThisFrame())
            {
                if (inkJSON != null)
                {
                    DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
                }
                else
                {
                    Debug.LogWarning("Manca il file Ink JSON nell'Inspector!");
                }
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}