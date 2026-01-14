using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Performation1 : MonoBehaviour
{
    [Header("Characters")]
    public GameObject leader;
    public GameObject player;
    public GameObject wall;
    public GameObject dialog;
    private PlayerMovement2D playerMovement;
    private bool ismoving = false;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        wall.SetActive(false);
        playerMovement = player.GetComponent<PlayerMovement2D>();
        leader.transform.position = new Vector3(10.0f, leader.transform.position.y, leader.transform.position.z);
        player.transform.position = new Vector3(11.0f, player.transform.position.y, player.transform.position.z);
        playerMovement.EnableMove(false);
        ismoving = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ismoving == true)
        {
            leader.transform.position = new Vector3(leader.transform.position.x - 0.025f, leader.transform.position.y, leader.transform.position.z);
            player.transform.position = new Vector3(player.transform.position.x - 0.025f, player.transform.position.y, player.transform.position.z);

            if (player.transform.position.x <= 8.0f)
            {
                ismoving = false;
                wall.SetActive(true); 
                playerMovement.EnableMove(true);
                dialog.SetActive(true);
            }
        }
    }
}
