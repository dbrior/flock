using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void StartGame() {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void SelectCharacter(int character) {
        PlayerPrefs.SetInt("SelectedCharacter", character);
    }
}
