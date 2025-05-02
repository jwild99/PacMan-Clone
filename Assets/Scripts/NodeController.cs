using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public bool canMoveLeft = false;
    public bool canMoveRight = false;
    public bool canMoveUp = false;
    public bool canMoveDown = false;

    public GameObject nodeLeft;
    public GameObject nodeRight;
    public GameObject nodeUp;
    public GameObject nodeDown;

    public bool isWarpRightNode = false;
    public bool isWarpLeftNode = false;

    // if the node contains a pellet when the game starts
    public bool isPelletNode = false;

    // if the node still contains a pellet
    public bool hasPellet = false;
    public SpriteRenderer pelletSprite;
    public GameManager gameManager;

    public bool isGhostStartingNode = false;
    public bool isSideNode = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (transform.childCount > 0)
        {
            hasPellet = true;
            isPelletNode = true;
            pelletSprite = GetComponentInChildren<SpriteRenderer>();
        }

        RaycastHit2D[] hitsDown;
        // Shoot a raycast line going down to check for nodes below current node
        hitsDown = Physics2D.RaycastAll(transform.position, -Vector2.up);

        // loop through all game objects that the raycast hit
        for (int i = 0; i < hitsDown.Length; i ++)
        {
            float distance = Mathf.Abs(hitsDown[i].point.y - transform.position.y);
            if (distance < 0.4f && hitsDown[i].collider.tag == "Node")
            {
                canMoveDown = true;
                nodeDown = hitsDown[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsUp;
        // Shoot a raycast line going up to check for nodes above current node
        hitsUp = Physics2D.RaycastAll(transform.position, Vector2.up);

        // loop through all game objects that the raycast hit
        for (int i = 0; i < hitsUp.Length; i ++)
        {
            float distance = Mathf.Abs(hitsUp[i].point.y - transform.position.y);
            if (distance < 0.4f && hitsUp[i].collider.tag == "Node")
            {
                canMoveUp = true;
                nodeUp = hitsUp[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsLeft;
        // Shoot a raycast line going to the left to check for nodes to the left of current node
        hitsLeft = Physics2D.RaycastAll(transform.position, Vector2.left);

        // loop through all game objects that the raycast hit
        for (int i = 0; i < hitsLeft.Length; i ++)
        {
            float distance = Mathf.Abs(hitsLeft[i].point.x - transform.position.x);
            if (distance < 0.4f && hitsLeft[i].collider.tag == "Node")
            {
                canMoveLeft = true;
                nodeLeft = hitsLeft[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsRight;
        // Shoot a raycast line going to the right to check for nodes to the right of current node
        hitsRight = Physics2D.RaycastAll(transform.position, Vector2.right);

        // loop through all game objects that the raycast hit
        for (int i = 0; i < hitsRight.Length; i ++)
        {
            float distance = Mathf.Abs(hitsRight[i].point.x - transform.position.x);
            if (distance < 0.4f && hitsRight[i].collider.tag == "Node")
            {
                canMoveRight = true;
                nodeRight = hitsRight[i].collider.gameObject;
            }
        }

        if (isGhostStartingNode)
        {
            canMoveDown = true;
            nodeDown = gameManager.ghostNodeCenter;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject GetNodeFromDirection(string direction)
    {
        if (direction == "left" && canMoveLeft)
            return nodeLeft;
        else if (direction == "right" && canMoveRight)
            return nodeRight;
        else if (direction == "up" && canMoveUp)
            return nodeUp;
        else if (direction == "down" && canMoveDown)
            return nodeDown;
        else
            return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && hasPellet)
        {
            hasPellet = false;
            pelletSprite.enabled = false;
            gameManager.CollectedPellet(this);
        }
    }
}
