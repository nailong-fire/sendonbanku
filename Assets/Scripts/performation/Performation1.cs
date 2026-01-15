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
    private Map.PlayerAnimController playerMovement;
    private bool ismoving = false;
    // Start is called before the first frame update
    void Start()
    {
        // ⭐ 如果已经见过村长，说明不是第一次进入主世界
        if (GameState.Instance != null &&
            GameState.Instance.story != null &&
            GameState.Instance.story.metVillageChief)
        {
            // 直接禁用这个演出
            gameObject.SetActive(false);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        wall.SetActive(false);
        playerMovement = player.GetComponent<Map.PlayerAnimController>();

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
