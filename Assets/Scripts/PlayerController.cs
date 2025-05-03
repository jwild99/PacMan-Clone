using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController movementController;
    public SpriteRenderer sprite;
    public Animator animator;
    public GameObject startNode;
    public Vector2 startPosition;
    public GameManager gameManager;

    public bool isDead = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        startPosition = new Vector2(-0.06f, -0.63f);
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        movementController = GetComponent<MovementController>();
        startNode = movementController.currentNode;
    }

    public void Setup()
    {
        isDead = false;
        animator.SetBool("dead", false);
        animator.SetBool("moving", false);

        movementController.currentNode = startNode;
        movementController.direction = "left";
        movementController.lastMovingDirection = "left";

        sprite.flipX = false;

        transform.position = startPosition;
        animator.speed = 1;
    }

    public void Stop()
    {
        animator.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameIsRunning)
        {
            if (!isDead)
            {
                animator.speed = 0;
            }
            return;
        }

        animator.speed = 1;


        animator.SetBool("moving", true);
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            movementController.SetDirection("left");
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movementController.SetDirection("right");
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            movementController.SetDirection("up");
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            movementController.SetDirection("down");
        }

        bool flipX = false;
        bool flipY = false;

        if (movementController.lastMovingDirection == "left")
        {
            animator.SetInteger("direction", 0);
        }
        else if (movementController.lastMovingDirection == "right")
        {
            animator.SetInteger("direction", 0);
            flipX = true;
        }
        else if (movementController.lastMovingDirection == "up")
        {
            animator.SetInteger("direction", 1);
        }
        else if (movementController.lastMovingDirection == "down")
        {
            animator.SetInteger("direction", 1);
            flipY = true;
        }

        sprite.flipX = flipX;
        sprite.flipY = flipY;
    }

    public void Death()
    {
        Debug.Log("Entered Death Function");

        isDead = true;
        animator.SetBool("moving", false);
        animator.SetBool("dead", true);
        animator.speed = 1;
    }
}
