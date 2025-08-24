using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject menuControlSettings;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] MenuState curMenu;

    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] public GameObject activePowerUp;
    [SerializeField] public GameObject doubleJumpText;
    [SerializeField] TMP_Text doubleJumpTimer;
    [SerializeField] public GameObject speedBoostText;
    [SerializeField] TMP_Text speedBoostTimer;

    public bool isPaused;

    [SerializeField] public GameObject ActiveReticle;
    public GameObject PistolReticle;
    public GameObject ShotgunReticle;
    public GameObject ARReticle;
    public GameObject AxeReticle;
    public GameObject DefaultReticle;

    public Image RageMeter;
    public Image PlayerDash;
    public Image PlayerHealth;
    public GameObject PlayerDamageScreen;
    public GameObject PlayerHealScreen;

    public GameObject player;
    public PlayerController playerScript;

    float timeScaleOriginal;

    int gameGoalCount;

    public float doubleJumpTimerCount;
    public float speedBoostTimerCount;

    public enum MenuState
    {
        None,
        Pause,
        Win,
        Lose,
        Settings
    }

    public MenuState origMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOriginal = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();

        ActiveReticle = DefaultReticle;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if(menuActive == null)
            {
                statePaused();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if(menuActive == menuPause)
            {
                stateUnpaused();
            }
        }
    }

    public void statePaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        curMenu = MenuState.Pause;
        origMenu = MenuState.Pause;
    }
    public void stateUnpaused()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        curMenu = 0;
        origMenu = 0;
    }
    
    public void updateGameGoal(int value)
    {
        gameGoalCount += value;

        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount <= 0)
        {
            // Player wins if there are no more waves left
            Room room = WaveManager.instance.rooms[WaveManager.instance.currentRoom];
            room.waveNumber++;
            if (!room.StartWave(room.waveNumber - 1))
            {
                WaveManager.instance.currentRoom++;
                if (WaveManager.instance.currentRoom >= WaveManager.instance.rooms.Length) Win();
            }
        }
    }

    // Win screen
    void Win()
    {
        statePaused();
        menuActive = menuWin;
        menuActive.SetActive(true);
        curMenu = MenuState.Win;
        origMenu = MenuState.Win;
    }

    public void updateToLoseScreen()
    {
        statePaused();
        menuActive = menuLose;
        menuActive.SetActive(true);
        curMenu = MenuState.Lose;
        origMenu = MenuState.Lose;
    }

    public void settingsOpen()
    {
        menuActive.SetActive(false);
        menuActive = null;

        menuActive = menuSettings;
        menuActive.SetActive(true);
    }

    public void controlSettingsOpen()
    {
        menuActive.SetActive(false);
        menuActive = null;

        menuActive = menuControlSettings;
        menuActive.SetActive(true);
        curMenu = MenuState.Settings;
    }

    public void settingsClosed()
    {
        switch(curMenu)
        {
            case MenuState.Pause:
            {
                 menuActive.SetActive(false);
                 menuActive = null;

                 menuActive = menuPause;
                 menuActive.SetActive(true);
                 break;
            }
            case MenuState.Win:
            {
                 menuActive.SetActive(false);
                 menuActive = null;

                 menuActive = menuWin;
                 menuActive.SetActive(true);
                 break;
            }
            case MenuState.Lose:
            {
                 menuActive.SetActive(false);
                 menuActive = null;

                 menuActive = menuLose;
                 menuActive.SetActive(true);
                 break;
            }
            case MenuState.Settings:
            {
                    curMenu = origMenu;
                 menuActive.SetActive(false);
                 menuActive = null;

                 menuActive = menuSettings;
                 menuActive.SetActive(true);
                 break;
            }

            default:
                break;
        }
    }

    public void doubleJumpTimerUpdate()
    {

        activePowerUp = doubleJumpText;
        activePowerUp.SetActive(true);

        if(doubleJumpTimerCount != 0)
        {
            doubleJumpTimerCount -= 1 * Time.deltaTime;
            doubleJumpTimer.text = doubleJumpTimerCount.ToString("F0");
        }
        else
        {
            return;
        }
    }

    public void speedBoostTimerUpdate()
    {

        activePowerUp = speedBoostText;
        activePowerUp.SetActive(true);

        if (speedBoostTimerCount != 0)
        {
            speedBoostTimerCount -= 1 * Time.deltaTime;
            speedBoostTimer.text = speedBoostTimerCount.ToString("F0");
        }
        else
        {
            return;
        }
    }
}
