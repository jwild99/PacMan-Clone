using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum GhostNodeStatesEnum
    {
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingInNodes
    }

    public GhostNodeStatesEnum ghostNodeState;
    public GhostNodeStatesEnum startGhostNodeState;

    public enum GhostType
    {
        red,
        blue,
        pink,
        orange
    }

    public GhostType ghostType;
    public GhostNodeStatesEnum respawnState;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public MovementController movementController;

    public GameObject startingNode;

    public bool readyToLeaveHome = false;

    public GameManager gameManager;

    public bool testRespawn = false;
    public bool isFrightened = false;

    public GameObject[] scatterNodes;
    public int scatterNodeIndex;

    public bool leftHomeBefore = false;

    public bool isVisible = true;

    public SpriteRenderer ghostSprite;
    public SpriteRenderer eyeSprite;

    public Animator animator;

    public Color color;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        ghostSprite = GetComponent<SpriteRenderer>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();

        if (ghostType == GhostType.red)
        {
            startGhostNodeState = GhostNodeStatesEnum.startNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeStart;
        }
        else if (ghostType == GhostType.pink)
        {
            startGhostNodeState = GhostNodeStatesEnum.centerNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
        }
        else if (ghostType == GhostType.blue)
        {
            startGhostNodeState = GhostNodeStatesEnum.leftNode;
            respawnState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
        }
        else if (ghostType == GhostType.orange)
        {
            startGhostNodeState = GhostNodeStatesEnum.rightNode;
            respawnState = GhostNodeStatesEnum.rightNode;
            startingNode = ghostNodeRight;
        }
    }

    public void Setup()
    {
        animator.SetBool("moving", false);

        if (startingNode == null)
        {
            Debug.LogError(name + " has no startingNode assigned.");
        }
        if (movementController == null)
        {
            Debug.LogError(name + " has no MovementController.");
        }

        if (gameManager.gameMode == "_Realism_Mode")
        {
            color.a = Mathf.Clamp01(0.25f);
        }

        // reset ghost node state back to start state
        ghostNodeState = startGhostNodeState;

        readyToLeaveHome = false;

        // reset ghosts back to their home positions
        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;

        movementController.direction = "";
        movementController.lastMovingDirection = "";

        // set their scatter node indices to 0
        scatterNodeIndex = 0;

        // set isFrightened to false
        isFrightened = false;

        leftHomeBefore = false;

        // set readyToLeaveHome back to false if blue or pink ghost
        if (ghostType == GhostType.red)
        {
            readyToLeaveHome = true;
            leftHomeBefore = true;
        }
        else if (ghostType == GhostType.pink)
        {
            readyToLeaveHome = true;
        }

        SetVisibile(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (ghostNodeState != GhostNodeStatesEnum.movingInNodes || !gameManager.isPowerPelletRunning)
        {
            isFrightened = false;
        }
        // show ghost sprites
        if (isVisible)
        {
            if (ghostNodeState != GhostNodeStatesEnum.respawning)
            {
                ghostSprite.enabled = true;
            }
            else
            {
                ghostSprite.enabled = false;
            }

            eyeSprite.enabled = true;
        }
        // hide ghost sprites
        else
        {
            ghostSprite.enabled = false;
            eyeSprite.enabled = false;
        }
         if (isFrightened)
        {
            animator.SetBool("frightened",true);
            eyeSprite.enabled = false;
            ghostSprite.color = new Color(255,255,255,255);
        }
        else
        {
            animator.SetBool("frightened",false);
            animator.SetBool("frightenedBlinking",false);

            ghostSprite.color = color;
        }

        if (!gameManager.gameIsRunning)
        {
            return;
        }

        if (gameManager.powerPelletTimer - gameManager.currenPowerPelletTime <= 3)
        {
            animator.SetBool("frightenedBlinking", true);
        }
        else
        {
            animator.SetBool("frightenedBlinking",false);
        }


        animator.SetBool("moving",true);

        if (testRespawn)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodeStatesEnum.respawning;
            testRespawn = false;
        }

        if (gameManager.gameMode == "_Hardcore_Mode")
        {
            if (movementController.currentNode.GetComponent<NodeController>().isSideNode)
            {
                movementController.SetSpeed(2.5f);
            }
            else
            {
                if (isFrightened)
                {
                    movementController.SetSpeed(2.5f);
                }
                else if (ghostNodeState == GhostNodeStatesEnum.respawning)
                {
                    movementController.SetSpeed(7);
                }
                else
                {
                    movementController.SetSpeed(3.5f);
                }
            }
        }
        else if (gameManager.gameMode == "_Normal_Mode")
        {
            if (movementController.currentNode.GetComponent<NodeController>().isSideNode)
            {
                movementController.SetSpeed(1);
            }
            else
            {
                if (isFrightened)
                {
                    movementController.SetSpeed(1);
                }
                else if (ghostNodeState == GhostNodeStatesEnum.respawning)
                {
                    movementController.SetSpeed(7);
                }
                else
                {
                    movementController.SetSpeed(2);
                }
            }
        }
        else if (gameManager.gameMode == "_Realism_Mode")
        {
            if (movementController.currentNode.GetComponent<NodeController>().isSideNode)
            {
                movementController.SetSpeed(0.5f);
            }
            else
            {
                if (isFrightened)
                {
                    movementController.SetSpeed(0.5f);
                }
                else if (ghostNodeState == GhostNodeStatesEnum.respawning)
                {
                    movementController.SetSpeed(7);
                }
                else
                {
                    movementController.SetSpeed(1);
                }
            }
        }


    }

    public void SetFrightened(bool newIsFrightened)
    {
        isFrightened = newIsFrightened;
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodeStatesEnum.movingInNodes)
        {
            leftHomeBefore = true;

            // scatter mode
            if (gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {
                DetermineGhostScatterModeDirection();
            }
            // firghtened mode
            else if (isFrightened)
            {
                string direction = GetRandomDirection();
                movementController.SetDirection(direction);
            }
            // chase mode
            else
            {
                // determine next game node to go to
                if (ghostType == GhostType.red)
                {
                    DetermineRedGhostDirection();
                }
                else if (ghostType == GhostType.pink)
                {
                    DeterminePinkGhostDirection();
                }
                else if (ghostType == GhostType.blue)
                {
                    DetermineBlueGhostDirection();
                }
                else if (ghostType == GhostType.orange)
                {
                    DetermineOrangeGhostDirection();
                }
            }

        }
        else if (ghostNodeState == GhostNodeStatesEnum.respawning)
        {
            string direction = "";

            // ghost has reached its start node, move to the center node.
            if (transform.position.x == ghostNodeStart.transform.position.x
            && transform.position.y == ghostNodeStart.transform.position.y)
            {
                direction = "down";
            }
            // ghost has reached the center node, either finish respawn or move to the left/right node
            else if (transform.position.x == ghostNodeCenter.transform.position.x
            && transform.position.y == ghostNodeCenter.transform.position.y)
            {
                if (respawnState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodeStatesEnum.leftNode)
                {
                    direction = "left";
                }
                else if (respawnState == GhostNodeStatesEnum.rightNode)
                {
                    direction = "right";
                }
            }
            // if ghost's respawn state is left/right node and ghost has reached that node, respawn and leave home
            else if ((transform.position.x == ghostNodeLeft.transform.position.x
            && transform.position.y == ghostNodeLeft.transform.position.y)
            || (transform.position.x == ghostNodeRight.transform.position.x
            && transform.position.y == ghostNodeRight.transform.position.y))
            {
                ghostNodeState = respawnState;
            }
            // ghost is in the gameboard still, locate its start node
            else
            {
                // determine quickest direction to home
                direction = GetClosestDirection(ghostNodeStart.transform.position);
            }

            movementController.SetDirection(direction);
        }
        else
        {
            // if ghost is ready to leave home
            if (readyToLeaveHome)
            {
                // if ghost is in the left home node, move to the center home node
                if (ghostNodeState == GhostNodeStatesEnum.leftNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.SetDirection("right");
                }
                // if ghost is in the right home node, move to the center home node
                else if (ghostNodeState == GhostNodeStatesEnum.rightNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.SetDirection("left");
                }
                // if ghost is in the center home node, move to the start node
                else if (ghostNodeState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.startNode;
                    movementController.SetDirection("up");
                }
                // if ghost is in the start node, start moving around in the game
                else if (ghostNodeState == GhostNodeStatesEnum.startNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.movingInNodes;
                    movementController.SetDirection("left");
                }
            }
        }
    }

    string GetRandomDirection()
    {
        List<string> possibleDirections = new List<string>();
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if (gameManager.gameMode == "_Realism_Mode")
        {
            if (movementController.lastMovingDirection != "up")
            {
                possibleDirections.Add("down");
            }
            if (movementController.lastMovingDirection != "down")
            {
                possibleDirections.Add("up");
            }
            if (movementController.lastMovingDirection != "left")
            {
                possibleDirections.Add("right");
            }
            if (movementController.lastMovingDirection != "right")
            {
                possibleDirections.Add("left");
            }
        }
        else
        {
            if (nodeController.canMoveDown && movementController.lastMovingDirection != "up")
            {
                possibleDirections.Add("down");
            }
            if (nodeController.canMoveUp && movementController.lastMovingDirection != "down")
            {
                possibleDirections.Add("up");
            }
            if (nodeController.canMoveRight && movementController.lastMovingDirection != "left")
            {
                possibleDirections.Add("right");
            }
            if (nodeController.canMoveLeft && movementController.lastMovingDirection != "right")
            {
                possibleDirections.Add("left");
            }
        }


        string direction = "";
        int randDirectionIndex = UnityEngine.Random.Range(0, possibleDirections.Count - 1);
        direction = possibleDirections[randDirectionIndex];

        return direction;
    }

    void DetermineGhostScatterModeDirection()
    {
        // if reached scatter node, add one to scatter node index
        if (transform.position.x == scatterNodes[scatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[scatterNodeIndex].transform.position.y)
            scatterNodeIndex ++;

        if (scatterNodeIndex == scatterNodes.Length - 1)
        {
            scatterNodeIndex = 0;
        }

        string direction = GetClosestDirection(scatterNodes[scatterNodeIndex].transform.position);
        movementController.SetDirection(direction);
    }

    void DetermineRedGhostDirection()
    {
        String direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.SetDirection(direction);
    }

    void DeterminePinkGhostDirection()
    {
        string pacmansDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = 0.35f;

        Vector2 target = gameManager.pacman.transform.position;

        if (pacmansDirection == "left")
        {
            target.x -= distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "right")
        {
            target.x += distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "up")
        {
            target.y += distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "down")
        {
            target.y -= distanceBetweenNodes * 2;
        }

        string direction = GetClosestDirection(target);
        movementController.SetDirection(direction);
    }

    void DetermineBlueGhostDirection()
    {
        string pacmansDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = 0.35f;

        Vector2 target = gameManager.pacman.transform.position;

        if (pacmansDirection == "left")
        {
            target.x -= distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "right")
        {
            target.x += distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "up")
        {
            target.y += distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "down")
        {
            target.y -= distanceBetweenNodes * 2;
        }

        GameObject redGhost = gameManager.redGhost;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;

        Vector2 blueTarget = new Vector2(target.x + xDistance, target.y + yDistance);

        String direction = GetClosestDirection(blueTarget);
        movementController.SetDirection(direction);
    }

    void DetermineOrangeGhostDirection()
    {
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);
        float distanceBetweenNodes = 0.35f;

        if (distance < 0)
        {
            distance *= -1;
        }

        // if within 8 nodes of pacman, chase with red ghost's logic
        if (distance <= distanceBetweenNodes * 8)
        {
            DetermineRedGhostDirection();
        }
        // otherwise use scatter mode logic
        else
        {
            DetermineGhostScatterModeDirection();
        }
    }

    string GetClosestDirection(Vector2 target)
    {
        float shortestDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";

        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        // if ghost can move up and won't be reversing direction, get the node above ghost
        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            GameObject nodeUp = nodeController.nodeUp;
            // gets the distance between top node above ghost and pacman
            float distance = Vector2.Distance(nodeUp.transform.position, target);

            // if this is the shortest distance so far, set new direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "up";
            }
        }

        // if ghost can move down and won't be reversing direction, get the node below ghost
        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            GameObject nodeDown = nodeController.nodeDown;
            // gets the distance between bottom node below ghost and pacman
            float distance = Vector2.Distance(nodeDown.transform.position, target);

            // if this is the shortest distance so far, set new direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "down";
            }
        }

        // if ghost can move left and won't be reversing direction, get the node left of ghost
        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            GameObject nodeLeft = nodeController.nodeLeft;
            // gets the distance between node to left of ghost and pacman
            float distance = Vector2.Distance(nodeLeft.transform.position, target);

            // if this is the shortest distance so far, set new direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "left";
            }
        }

         // if ghost can move right and won't be reversing direction, get the node right of ghost
        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            GameObject nodeRight = nodeController.nodeRight;
            // gets the distance between node to right of ghost and pacman
            float distance = Vector2.Distance(nodeRight.transform.position, target);

            // if this is the shortest distance so far, set new direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "right";
            }
        }

        return newDirection;
    }

    public void SetVisibile(bool newIsVisible)
    {
        isVisible = newIsVisible;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Ghost collided with something");

        if (collision.tag == "Player" && ghostNodeState != GhostNodeStatesEnum.respawning)
        {
            Debug.Log("Ghost collided with player");

            // Get eaten by player
            if (isFrightened)
            {
                gameManager.GhostEaten();
                ghostNodeState = GhostNodeStatesEnum.respawning;
            }
            // eat player
            else
            {
                StartCoroutine(gameManager.PlayerEaten());
            }
        }
    }
}
