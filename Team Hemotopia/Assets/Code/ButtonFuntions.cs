using UnityEngine;

public class ButtonFuntions : MonoBehaviour
{
    public void resumeButton()
    {
        GameManager.instance.stateUnpaused();
    }
}

