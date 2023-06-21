using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public string loseSceneName = "LoseScene";
    public string winSceneName = "WinScene";

    public void LoadScene(string sceneName) {
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
}
