using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFuntions : MonoBehaviour
{
    public void resumeButton()
    {
        GameManager.instance.stateUnpaused();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpaused();
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }

    public void settings()
    {
        GameManager.instance.settingsOpen();
    }

    public void back()
    {
        GameManager.instance.settingsClosed();
    }
}

