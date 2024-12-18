using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
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

    void Start() {
        Time.timeScale = 0;
        MusicManager.Instance.PauseMusic();
        DisplayDialogue("Breath again young one[pause:0.5].[pause:0.5].[pause:0.5].[pause:0.5]\n Your time has not yet come.\n[pause:1.0]Protect the bearer of the <color=#FFD700>golden wool</color>.".Replace(" ", "    ").ToUpper());
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
    /// Quickly display the full text without letter-by-letter reveal
    /// </summary>
    public void SkipToFullText(string dialogue)
    {
        StopAllCoroutines();
        // Remove pause markers for final display
        dialogueText.text = Regex.Replace(dialogue, @"\[pause:[^]]+\]", "");
    }
}