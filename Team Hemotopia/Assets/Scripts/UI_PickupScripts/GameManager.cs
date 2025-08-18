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
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] MenuState menu;

    [SerializeField] TMP_Text gameGoalCountText;

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

    enum MenuState
    {
        None,
        Pause,
        Win,
        Lose
    }


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
        menu = MenuState.Pause;
    }
    public void stateUnpaused()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }
    
    public void updateGameGoal(int value)
    {
        gameGoalCount += value;

        gameGoalCountText.text = gameGoalCount.ToString("F0");

        // Win screen
        if(gameGoalCount <= 0)
        {
            statePaused();
            menuActive = menuWin;
            menuActive.SetActive(true);
            menu = MenuState.Win;
        }
    }

    public void updateToLoseScreen()
    {
        statePaused();
        menuActive = menuLose;
        menuActive.SetActive(true);
        menu = MenuState.Lose;
    }

    public void settingsOpen()
    {
        menuActive.SetActive(false);
        menuActive = null;

        menuActive = menuSettings;
        menuActive.SetActive(true);
    }

    public void settingsClosed()
    {
        switch(menu)
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

            default:
                break;
        }
    }
}
