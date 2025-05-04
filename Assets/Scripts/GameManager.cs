using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public string gameMode;

    public GameObject pacman;

    public GameObject leftWarpNode;
    public GameObject rightWarpNode;

    public AudioSource siren;

    public AudioSource munch1;
    public AudioSource munch2;
    public AudioSource powerPelletAudio;
    public AudioSource respawningAudio;
    public AudioSource ghostEatenAudio;



    public int currentMunch = 0;

    public int score;
    public Text scoreText;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public GameObject redGhost;
    public GameObject pinkGhost;
    public GameObject blueGhost;
    public GameObject orangeGhost;

    public EnemyController redGhostController;
    public EnemyController pinkGhostController;
    public EnemyController blueGhostController;
    public EnemyController orangeGhostController;

    public int totalPellets;
    public int pelletsLeft;
    public int pelletsCollectedOnThisLife;

    public bool hadDeathOnThisLevel = false;

    public bool gameIsRunning;

    public Image blackBackground;
    public Text gameOverText;
    public Text livesText;

    public bool isPowerPelletRunning = false;
    public float currenPowerPelletTime = 0;
    public float powerPelletTimer = 8f;
    public int powerPelletMultiplyer = 1;


    public List<NodeController> nodeControllers = new List<NodeController>();

    public bool newGame;
    public bool clearedLevel;

    public AudioSource startGameAudio;
    public AudioSource deathSound;

    public int lives;
    public int currentLevel;

    public int[] ghostModeTimers = new int[] {7, 20, 7, 20, 5, 20, 5};
    public int ghostModeTimerIndex;
    public float ghostModeTimer;
    public bool runningTimer;
    public bool completedTimer;

    public enum GhostMode
    {
        chase,
        scatter
    }

    public GhostMode currentGhostMode;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("GameManager Awake()");

        // black screen turned off by default
        blackBackground.enabled = false;

        newGame = true;
        clearedLevel = false;

        redGhostController = redGhost.GetComponent<EnemyController>();
        pinkGhostController = pinkGhost.GetComponent<EnemyController>();
        blueGhostController = blueGhost.GetComponent<EnemyController>();
        orangeGhostController = orangeGhost.GetComponent<EnemyController>();

        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;

        pacman = GameObject.Find("Player");

        StartCoroutine(Setup());
    }

    public IEnumerator Setup()
    {
        Debug.Log("Setup started.");

        gameMode = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        Debug.Log(gameMode);

        ghostModeTimerIndex = 0;

        ghostModeTimer = 0f;
        completedTimer = false;
        runningTimer = true;

        gameOverText.enabled = false;

        // setup munch audio to first audio source
        currentMunch = 0;

        // if pacman clears a level, a background will appear covering the level and the game will pause for 0.1 seconds
        if (clearedLevel)
        {
            Debug.Log("Cleared level. Resetting everything and pausing briefly.");

            // activate background
            blackBackground.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        // deactivate background after 0.1 seconds
        blackBackground.enabled = false;

        pelletsCollectedOnThisLife = 0;
        currentGhostMode = GhostMode.scatter;
        gameIsRunning = false;

        float waitTimer = 1f;

        if (clearedLevel || newGame)
        {
            hadDeathOnThisLevel = false;
            pelletsLeft = totalPellets;
            waitTimer = 4f;
            // pellets respawn when pacman clears a level or a new game starts
            for (int i = 0; i < nodeControllers.Count; i ++)
            {
                nodeControllers[i].RespawnPellet();
            }
        }

        if (newGame)
        {
            Debug.Log("New game setup.");

            if (gameMode == "_Hardcore_Mode")
            {
                startGameAudio.pitch = 0.5f;
                waitTimer *= 2;
                SetLives(1);
                pacman.GetComponent<MovementController>().SetSpeed(4.65f);
            }
            else if (gameMode == "_Ryan_Mode")
            {
                startGameAudio.pitch = 1.5f;
                waitTimer /= 1.5f;
                SetLives(Random.Range(1, 6));
            }
            else
                SetLives(3);

            startGameAudio.Play();
            score = 0;
            scoreText.text = "Score: " + score.ToString();

            currentLevel = 1;
        }

        // setup pacman
        pacman.GetComponent<PlayerController>().Setup();

        // ghosts get setup
        redGhostController.Setup();
        pinkGhostController.Setup();
        blueGhostController.Setup();
        orangeGhostController.Setup();

        newGame = false;
        clearedLevel = false;

        Debug.Log("Waiting " + waitTimer + " seconds before starting.");
        yield return new WaitForSeconds(waitTimer);

        Debug.Log("Calling StartGame()");
        StartGame();
    }

    void SetLives(int newLives)
    {
        lives = newLives;
        livesText.text = "Lives: " + lives;

        if (gameMode == "_Ryan_Mode")
        {
            livesText.text = "Lives: ???";
        }
    }

    void StartGame()
    {
        gameIsRunning = true;
        siren.Play();
    }

    void StopGame()
    {
        gameIsRunning = false;
        siren.Stop();
        powerPelletAudio.Stop();
        respawningAudio.Stop();
        pacman.GetComponent<PlayerController>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameIsRunning)
        {
            return;
        }

        if (redGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning
            || blueGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning
            || pinkGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning
            || orangeGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning)
        {
            if (!respawningAudio.isPlaying)
            {
                respawningAudio.Play();
            }
        }
        else
        {
            if (respawningAudio.isPlaying)
            {
                respawningAudio.Stop();
            }
        }
        if (gameMode == "_Hardcore_Mode")
        {
            completedTimer = true;
            runningTimer = false;
            currentGhostMode = GhostMode.chase;
        }
        if (!completedTimer && runningTimer)
        {
            ghostModeTimer += Time.deltaTime;
            if (ghostModeTimer >= ghostModeTimers[ghostModeTimerIndex])
            {
                ghostModeTimer = 0;
                ghostModeTimerIndex ++;
                if (currentGhostMode == GhostMode.chase)
                {
                    currentGhostMode = GhostMode.scatter;
                }
                else
                {
                    currentGhostMode = GhostMode.chase;
                }

                if (ghostModeTimerIndex == ghostModeTimers.Length)
                {
                    completedTimer = true;
                    runningTimer = false;
                    currentGhostMode = GhostMode.chase;
                }
            }
        }
        if (isPowerPelletRunning)
        {
            currenPowerPelletTime += Time.deltaTime;
            if (currenPowerPelletTime >= powerPelletTimer)
            {
                isPowerPelletRunning = false;
                currenPowerPelletTime = 0;
                powerPelletAudio.Stop();
                siren.Play();
                powerPelletMultiplyer = 1;

            }
        }
    }

    public void GotPelletFromNodeController(NodeController nodeController)
    {
        nodeControllers.Add(nodeController);
        totalPellets ++;
        pelletsLeft ++;
    }

    public void AddToScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score.ToString();
    }

    public IEnumerator CollectedPellet(NodeController nodeController)
    {
        if (currentMunch == 0)
        {
            munch1.Play();
            currentMunch = 1;
        }
        else if (currentMunch == 1)
        {
            munch2.Play();
            currentMunch = 0;
        }

        pelletsLeft --;
        pelletsCollectedOnThisLife ++;

        int requiredBluePellets = 0;
        int requiredOrangePellets = 0;

        if (hadDeathOnThisLevel)
        {
            requiredBluePellets = 12;
            requiredOrangePellets = 32;
        }
        else
        {
            requiredBluePellets = 30;
            requiredOrangePellets = 60;
        }

        if (pelletsCollectedOnThisLife >= requiredBluePellets && !blueGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            blueGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }
        if (pelletsCollectedOnThisLife >= requiredOrangePellets && !orangeGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            orangeGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }

        // add to our score
        if (gameMode == "_Hardcore_Mode")
            AddToScore(25);
        else if (gameMode == "_Ryan_Mode")
        {
            AddToScore(Random.Range(0, 101));
        }
        else
            AddToScore(10);


        // check if there are any pellets left
        if (pelletsLeft == 0)
        {
            currentLevel += 1;
            clearedLevel = true;
            StopGame();
            yield return new WaitForSeconds(1);
            StartCoroutine(Setup());
        }

        // is this a power pellet
        if (nodeController.isPowerPellet)
        {
            siren.Stop();
            powerPelletAudio.Play();
            isPowerPelletRunning = true;
            currenPowerPelletTime = 0;

            redGhostController.SetFrightened(true);
            blueGhostController.SetFrightened(true);
            pinkGhostController.SetFrightened(true);
            orangeGhostController.SetFrightened(true);
        }
    }
    public IEnumerator PauseGame(float timeToPause)
    {
        gameIsRunning = false;
        yield return new WaitForSeconds(timeToPause);
        gameIsRunning = true;
    }

    public void QuitGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("_Menu");
    }

    public void GhostEaten()
    {
        ghostEatenAudio.Play();

        if (gameMode == "_Hardcore_Mode")
            AddToScore(500 * powerPelletMultiplyer);
        else
            AddToScore(400 * powerPelletMultiplyer);
        powerPelletMultiplyer++;
        StartCoroutine(PauseGame(1));
    }

    public IEnumerator PlayerEaten()
    {
        hadDeathOnThisLevel = true;
        StopGame();
        yield return new WaitForSeconds(1);

        redGhostController.SetVisibile(false);
        pinkGhostController.SetVisibile(false);
        blueGhostController.SetVisibile(false);
        orangeGhostController.SetVisibile(false);

        pacman.GetComponent<PlayerController>().Death();
        deathSound.Play();

        yield return new WaitForSeconds(3);

        SetLives(lives -1);

        if (lives <= 0)
        {
            newGame = true;

            // display game over text
            gameOverText.enabled = true;

            int highScore = PlayerPrefs.GetInt("HighScore");
            if (score > highScore)
                PlayerPrefs.SetInt("HighScore", score);

            yield return new WaitForSeconds(3);

            UnityEngine.SceneManagement.SceneManager.LoadScene("_Menu");
        }

        StartCoroutine(Setup());
    }
}
