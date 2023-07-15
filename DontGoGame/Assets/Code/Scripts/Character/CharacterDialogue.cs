using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] string[] dialogueLines;
    [SerializeField] GameSettingsSO gameSettings;

    private int dialogueIndex;
    private int currentPages;

    IEnumerator dialogueCoroutine = null;

    public bool showingText = false;

    IEnumerator TypeDialogue()
    {
        showingText = true;

        foreach (char letter in dialogueLines[dialogueIndex].ToCharArray())
        {
            // Change page of the dialogue box if the text is too long
            if(dialogueText.textInfo.pageCount > currentPages)
            {
                yield return new WaitForSeconds(0.2f);
                currentPages++;
                dialogueText.pageToDisplay = currentPages;
            }

            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        Debug.Log(dialogueIndex);
        dialogueIndex =  (dialogueIndex + 1) > (dialogueLines.Length - 1) ? dialogueIndex : dialogueIndex + 1;  

        gameSettings.currentDialogue = dialogueIndex;

        yield return new WaitForSeconds(0.2f);

        showingText = false;    
    }

    public void LoadDialogue(int index)
    {
        dialogueIndex = index;
        dialogueCoroutine = TypeDialogue();
        RestartDialogue();
        StartCoroutine(dialogueCoroutine);
    }

    void NextLine()
    {
        RestartDialogue();
        StartCoroutine(dialogueCoroutine);
    }

    private void RestartDialogue()
    {
        dialogueText.text = "";
        currentPages = 1;
        dialogueText.pageToDisplay = currentPages;
        dialogueText.textInfo.pageCount = currentPages;
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogueCoroutine = TypeDialogue();
        currentPages = 1;
        dialogueIndex = gameSettings.currentDialogue;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
