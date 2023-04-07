using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public static void NewGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public static void LoadGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        OnTerrainReady(MapPersistence.LoadGame);
    }

    public static void OnTerrainReady(Action action) {
        SceneManager.sceneLoaded += (s, l) => { action(); };
    }
}
