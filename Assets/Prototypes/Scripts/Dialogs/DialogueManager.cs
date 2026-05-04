using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    private GameInput gameInput;
    
    public bool dialogueIsPlaying { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Trovato più di un Dialogue Manager nella scena");
            Destroy(gameObject); // Distruggi il duplicato per sicurezza
            return;
        }
        instance = this;

        // FONDAMENTALE: Devi creare l'istanza qui!
        gameInput = new GameInput();
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        // Se il dialogo non è attivo o per qualche motivo gameInput è ancora null, esci subito
        if (!dialogueIsPlaying || gameInput == null)
        {
            return;
        }

        // Aggiungi le parentesi () a WasPressedThisFrame
        if (gameInput.Player.Interact.WasPressedThisFrame())
        {
            ContinueStory();
        }
    }

    private void OnEnable()
    {
        // Se gameInput è null qui, forziamo la creazione prima di abilitarlo
        if (gameInput == null)
        {
            gameInput = new GameInput();
        }
        gameInput.Enable();
    }

    private void OnDisable()
    {
        gameInput.Disable();
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }


    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f); // Piccola pausa per evitare input accidentali
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }


    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            // Testo del dialogo 
            dialogueText.text = currentStory.Continue();

            //Display choices, se ce ne sono
            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        //Faccio un check per assicurarmi che il numero di scelte non superi il numero di pulsanti disponibili
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("Il numero di scelte nel dialogo supera il numero di pulsanti disponibili!");
            return;
        }
        
        int index = 0;

        // Attiva i pulsanti e imposta il testo per ogni scelta
        foreach (Choice choice in currentChoices)
        {
            choices[index].SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        // Disattiva i pulsanti rimanenti se ci sono meno scelte del numero di pulsanti
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].SetActive(false);
        }

    }


}
