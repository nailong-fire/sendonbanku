using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;
   using TMPro;

   public class PlayerUIManager : MonoBehaviour
   {
       public TextMeshProUGUI playerNameText;
       public TextMeshProUGUI hopeHealthText;
       public UnityEngine.UI.Slider faithSlider;

       private PlayerController playerController;

       // 设置玩家控制器
       public void SetPlayerController(PlayerController controller)
       {
           playerController = controller;
           UpdateUI();
       }

       // 更新UI显示
       public void UpdateUI()
       {
           if (playerController == null) return;

           playerNameText.text = playerController.playerData.playerName;
           // Hope作为玩家血量显示
           hopeHealthText.text = $"Hope: {playerController.playerData.hope}/{playerController.playerData.maxHope}";
           faithSlider.value = (float)playerController.playerData.faith / playerController.playerData.maxFaith;
       }
    }