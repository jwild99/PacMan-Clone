using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ONLY USED FOR SETTING UP THE GAME BOARD- NOT NEEDED ANYMORE (but didn't want to delete)
public class NodeDeleter : MonoBehaviour
{
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
                if (collision.tag == "Node")
                {
                        Destroy(collision.gameObject);
                }
        }
}
