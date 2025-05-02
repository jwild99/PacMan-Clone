using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject pacman;

    public GameObject leftWarpNode;
    public GameObject rightWarpNode;

    public AudioSource siren;

    public AudioSource munch1;
    public AudioSource munch2;
    public int currentMunch;

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

    public List<NodeController> nodeControllers = new List<NodeController>();

    public bool newGame;
    public bool clearedLevel;

    public AudioSource startGameAudio;
    public AudioSource deathSound;

    public int lives;
    public int currentLevel;

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

            startGameAudio.Play();
            score = 0;
            scoreText.text = "Score: " + score.ToString();
            lives = 3;
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

    void StartGame()
    {
        gameIsRunning = true;
        siren.Play();
    }

    void StopGame()
    {
        gameIsRunning = false;
        siren.Stop();
        pacman.GetComponent<PlayerController>().Stop();
    }

    // Update is called once per frame
    void Update()
    {

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

        lives --;

        if (lives <= 0)
        {
            newGame = true;

            // display game over text
            gameOverText.enabled = true;

            yield return new WaitForSeconds(3);
        }

        StartCoroutine(Setup());
    }
}
