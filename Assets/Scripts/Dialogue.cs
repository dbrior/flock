using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private List<string> dialogueLines;
    [SerializeField] private UnityEvent onDialogueEnd;

    // public static Dialogue Instance {get; private set;}

    [Tooltip("Text component where the dialogue will be displayed")]
    public TextMeshProUGUI dialogueText;

    [Tooltip("Audio clip to play for each letter")]
    public AudioClip letterSound;

    [Tooltip("Audio clip to play for spaces (optional)")]
    public AudioClip spaceSound;

    [Tooltip("Audio source to play sounds")]
    public AudioSource audioSource;

    [Tooltip("Default time between letter reveals")]
    public float defaultLetterRevealDelay = 0.05f;

    [Tooltip("Volume for letter sounds")]
    [Range(0f, 1f)]
    public float letterSoundVolume = 0.5f;

    // Regex for detecting pause and formatting commands
    private static readonly Regex SpecialCommandRegex = new Regex(@"\[(pause:[^]]+)\]|<[^>]+>");

    private List<string> upcomingDialogue;
    private bool isDialogueWriting;
    private string currentDialogue;

    // void Awake() {
    //     if (Instance == null) {Instance = this;}
    //     else {Destroy(gameObject);}
    // }

    void Start() {
        Time.timeScale = 0;
        for (int i=0; i<dialogueLines.Count; i++) {
            dialogueLines[i] = dialogueLines[i].Replace(" ", "    ").ToUpper();
        }
        // MusicManager.Instance.PauseMusic();
        SetUpcomingDialogue(dialogueLines);
        // SetUpcomingDialogue(new List<string>(){

        //     "Dead again I see[pause:0.5].[pause:0.5].[pause:0.5].[pause:0.5]\n Worry not, I'll send you back once more.",
        //     // "Make sheep and hire minions to survive.".Replace(" ", "    ").ToUpper(),
        //     // "Beware the calamity on the <color=#0043b0>fifth night</color>.[pause:1.0]\nProtect the bearer of the <color=#c9aa02>golden wool</color>.".Replace(" ", "    ").ToUpper()
        // });
        NextDialogue();
    }

    public void SetUpcomingDialogue(List<string> newDialoguge) {
        upcomingDialogue = newDialoguge;
    }

    public void OnInteract() {
        if (isDialogueWriting) SkipToFullText(currentDialogue);
        else NextDialogue();
    }

    public void NextDialogue() {
        if (upcomingDialogue.Count == 0) {
            EndDialogue();
            return;
        }

        DisplayDialogue(upcomingDialogue[0]);
        upcomingDialogue.RemoveAt(0);
    }

    private void EndDialogue() {
        Time.timeScale = 1f;
        onDialogueEnd?.Invoke();
        transform.parent.gameObject.SetActive(false);
        // MusicManager.Instance.ResumeMusic();
    }

    /// <summary>
    /// Displays the dialogue text with letter-by-letter reveal, supporting pauses and rich text
    /// </summary>
    /// <param name="dialogue">The full dialogue string to display</param>
    public void DisplayDialogue(string dialogue)
    {
        // Stop any ongoing dialogue reveal
        StopAllCoroutines();
        
        // Clear the text
        dialogueText.text = "";
        
        // Start the letter-by-letter reveal
        StartCoroutine(RevealLetters(dialogue));
    }

    private IEnumerator RevealLetters(string dialogue)
    {
        currentDialogue = dialogue;
        isDialogueWriting = true;
        // Prepare for letter-by-letter reveal
        dialogueText.text = "";

        // Keep track of current position and revealed text
        int currentIndex = 0;
        string revealedText = "";

        while (currentIndex < dialogue.Length)
        {
            // Check for pause command
            var pauseMatch = Regex.Match(dialogue.Substring(currentIndex), @"^\[pause:(\d+\.\d+)\]", RegexOptions.IgnoreCase);

            if (pauseMatch.Success)
            {
                // Parse and wait for the specified pause duration
                float pauseDuration = float.Parse(pauseMatch.Groups[1].Value);
                yield return new WaitForSecondsRealtime(pauseDuration);

                // Move past the pause marker
                currentIndex += pauseMatch.Length;
                continue;
            }

            // Check for opening tag
            var openTagMatch = Regex.Match(dialogue.Substring(currentIndex), @"^<[^>]+>");
            if (openTagMatch.Success)
            {
                // Append the entire tag to revealed text
                revealedText += openTagMatch.Value;
                dialogueText.text = revealedText;
                currentIndex += openTagMatch.Length;
                continue;
            }

            // Check for closing tag
            var closeTagMatch = Regex.Match(dialogue.Substring(currentIndex), @"^</[^>]+>");
            if (closeTagMatch.Success)
            {
                // Append the entire tag to revealed text
                revealedText += closeTagMatch.Value;
                dialogueText.text = revealedText;
                currentIndex += closeTagMatch.Length;
                continue;
            }

            // Reveal a single character
            char currentChar = dialogue[currentIndex];
            revealedText += currentChar;
            dialogueText.text = revealedText;

            // Play sound for the letter
            PlayLetterSound(currentChar);

            // Wait for the specified delay before next letter
            yield return new WaitForSecondsRealtime(defaultLetterRevealDelay);

            currentIndex++;
        }
        isDialogueWriting = false;
    }

    private void PlayLetterSound(char letter)
    {
        // Ensure audio source and sounds are set
        if (audioSource == null || (letterSound == null && spaceSound == null))
            return;

        // Determine which sound to play
        AudioClip soundToPlay = char.IsWhiteSpace(letter) ? spaceSound : letterSound;

        // Play the appropriate sound if available
        if (soundToPlay != null)
        {
            audioSource.PlayOneShot(soundToPlay, letterSoundVolume);
        }
    }

    /// <summary>
    /// Quickly display the full text without letter-by-letter reveal, removing pause markers but keeping formatting
    /// </summary>
    public void SkipToFullText(string dialogue)
    {
        StopAllCoroutines();
        // Remove only pause markers while preserving formatting tags
        string cleanText = Regex.Replace(dialogue, @"\[pause:[^]]+\]", "", RegexOptions.IgnoreCase);
        dialogueText.text = cleanText;
        isDialogueWriting = false;
    }

}