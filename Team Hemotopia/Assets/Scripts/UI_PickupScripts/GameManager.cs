using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEditor.Rendering;
using UnityEditor;
using System.IO;

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

    [SerializeField] public GameObject wavePlusEnemyUI;
    [SerializeField] public TMP_Text waveCurrent;
    [SerializeField] public TMP_Text waveTotal;
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] public GameObject activePowerUp;
    [SerializeField] public GameObject doubleJumpText;
    [SerializeField] TMP_Text doubleJumpTimer;
    [SerializeField] public GameObject speedBoostText;
    [SerializeField] TMP_Text speedBoostTimer;

    [SerializeField] public GameObject reloadUI;
    [SerializeField] public GameObject outOfAmmoUI;
    [SerializeField] public GameObject ammoUI;
    [SerializeField] public TMP_Text ammoCurrent;
    [SerializeField] public TMP_Text ammoTotal;

    public bool isPaused;

    [SerializeField] public GameObject reticle;
    [SerializeField] public GameObject ARReticle;
    [SerializeField] public GameObject ShotgunReticle;
    [SerializeField] public GameObject PistolReticle;
    [SerializeField] public GameObject AxeReticle;
    public GameObject DefaultReticle;

    public Image RageMeter;
    public Image PlayerDash;
    public Image PlayerHealth;
    public GameObject playerSpawnPos;
    public GameObject PlayerDamageScreen;
    public GameObject PlayerHealScreen;

    public GameObject player;
    public PlayerController playerScript;

    float timeScaleOriginal;


    int gameGoalCount;

    public float doubleJumpTimerCount;
    public float speedBoostTimerCount;

    List<EnemySave> enemiesOnMap = new List<EnemySave>();
    List<gunStats> gunSave = new List<gunStats>();
    List<GunSave> gunStatsSave = new List<GunSave>();
    List<GunPickupSave> gunPickupSave = new List<GunPickupSave>();
    List<RoomSave> roomSave = new List<RoomSave>();

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

        playerSpawnPos = GameObject.FindWithTag("PlayerSpawnPos");

        reticle = DefaultReticle;

        wavePlusEnemyUI.SetActive(false);

        SaveForRespawn();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePaused();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
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
            if(room.waveNumber != room.waves.Length + 1)
            {
            waveCurrent.text = room.waveNumber.ToString("F0");
            waveTotal.text = room.waves.Length.ToString("F0");
                
            }
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
        switch (curMenu)
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

        if (doubleJumpTimerCount != 0)
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

    public void SaveForRespawn()
    {
        enemiesOnMap.Clear();
        EnemyAI_Base[] enemies = FindObjectsByType<EnemyAI_Base>(FindObjectsSortMode.None);
        foreach (EnemyAI_Base enemy in enemies)
        {
            EnemySave save = new EnemySave();
            Transform enemyTransform = enemy.gameObject.transform;

            save.rotW = enemyTransform.rotation.w;
            save.rotX = enemyTransform.rotation.x;
            save.rotY = enemyTransform.rotation.y;
            save.rotZ = enemyTransform.rotation.z;

            save.posX = enemyTransform.position.x;
            save.posY = enemyTransform.position.y;
            save.posZ = enemyTransform.position.z;

            save.health = enemy.HP;
            save.prefab = enemy.enemyStats.prefab;

            enemiesOnMap.Add(save);
        }

        gunSave = new List<gunStats>(playerScript.gunList);
        gunStatsSave.Clear();
        foreach (gunStats stats in gunSave)
        {
            GunSave save = new GunSave();
            save.ammo = stats.ammoCur;
            save.damage = stats.damage;
            save.isAutomatic = stats.isAutomatic;
            save.rageDamage = stats.rageDamage;
            save.fireDistance = stats.fireDist;
            save.fireRate = stats.fireRate;
            save.bullets = stats.bullets;
            save.bloomMod = stats.bloomMod;
            save.rageMeterIncrement = stats.rageMeterIncrement;

            gunStatsSave.Add(save);
        }

        GameObject[] gunPickups = GameObject.FindGameObjectsWithTag("GunPickup");
        gunPickupSave.Clear();
        foreach (GameObject gun in gunPickups)
        {
            GunPickupSave save = new GunPickupSave();
            pickup pickup = gun.GetComponent<pickup>();
            save.stats = pickup.gun;
            save.rotW = gun.transform.rotation.w;
            save.rotX = gun.transform.rotation.x;
            save.rotY = gun.transform.rotation.y;
            save.rotZ = gun.transform.rotation.z;

            save.posX = gun.transform.position.x;
            save.posY = gun.transform.position.y;
            save.posZ = gun.transform.position.z;

            save.prefab = pickup.gun.prefab;

            gunPickupSave.Add(save);
        }

        roomSave.Clear();
        foreach (Room room in WaveManager.instance.rooms)
        {
            RoomSave save = new RoomSave();
            save.waveNumber = 1;
            save.started = room.started;

            roomSave.Add(save);
        }
    }

    public void LoadRespawn() // Get rid of weapon model and change the ammo ui on respawn.
    {
        foreach (EnemyAI_Base enemy in FindObjectsByType<EnemyAI_Base>(FindObjectsSortMode.None))
        {
            Destroy(enemy.gameObject);
        }

        foreach (EnemySave enemy in enemiesOnMap)
        {
            GameObject enemyObject = Instantiate(enemy.prefab, new Vector3(enemy.posX, enemy.posY, enemy.posZ), new Quaternion(enemy.rotX, enemy.rotY, enemy.rotZ, enemy.rotW));
            enemyObject.GetComponent<EnemyAI_Base>().HP = enemy.health;
        }

        playerScript.gunList.Clear();
        playerScript.gunListPos = 0;

        if (gunSave.Count > 0) playerScript.gunList = new List<gunStats>(gunSave);
        else
        {
            playerScript.gunList = new List<gunStats>();
            playerScript.ResetPlayerGunStats();
        }

        for (int gunIndex = 0; gunIndex < playerScript.gunList.Count; gunIndex++)
        {
            playerScript.gunList[gunIndex].ammoCur = gunSave[gunIndex].ammoCur;
        }

        if (playerScript.gunList.Count > 0)
        {
            playerScript.ChangeGun();
            playerScript.ChangePlayerReticle();
        }

        foreach (GameObject gun in GameObject.FindGameObjectsWithTag("GunPickup"))
        {
            Destroy(gun);
        }

        foreach (GunPickupSave gun in gunPickupSave)
        {
            GameObject gunObject = Instantiate(gun.prefab, new Vector3(gun.posX, gun.posY, gun.posZ), new Quaternion(gun.rotX, gun.rotY, gun.rotZ, gun.rotW));
            pickup pickup = gunObject.GetComponent<pickup>();
            pickup.gun = gun.stats;
        }

        for (int roomIndex = 0; roomIndex < roomSave.Count; roomIndex++)
        {
            WaveManager.instance.rooms[roomIndex].started = roomSave[roomIndex].started;
            WaveManager.instance.rooms[roomIndex].waveNumber = roomSave[roomIndex].waveNumber;
        }
    }
}
