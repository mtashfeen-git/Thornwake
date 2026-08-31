using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene To Load")]
    [SerializeField] private string levelSceneName = "Level1";

    public void StartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(levelSceneName);
    }

    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }

    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }

    public void Quit()
    {
        Application.Quit(); // Quits the game (only works in build)

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exits play mode in the Editor
#endif
    }
}