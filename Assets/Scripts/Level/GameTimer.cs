using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour {
    public float timeLeft = 120.0f;
    public SceneLoader sceneLoader;
    

    private void Update() {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0) {
            sceneLoader.LoadLose();
        }
    }
}
