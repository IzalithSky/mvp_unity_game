using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "ScriptableObjects/SceneLoader", order = 1)]
public class SceneLoader : ScriptableObject {
    public string loseSceneName = "LoseScene";
    public string winSceneName = "WinScene";
    public string prevSceneName = "MainMenuScene";

    public void LoadScene(string sceneName) {
        prevSceneName = SceneManager.GetActiveScene().name; 
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ResetScene() {
        Scene scene = SceneManager.GetActiveScene(); 
        LoadScene(scene.name);
    }

    public void LoadLose() {
        LoadScene(loseSceneName);
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadWin() {
        LoadScene(winSceneName);
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadPrevious() {
        LoadScene(prevSceneName);
    }
}
