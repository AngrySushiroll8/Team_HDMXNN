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


}

