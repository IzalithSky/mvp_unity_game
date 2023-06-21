using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
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
}
