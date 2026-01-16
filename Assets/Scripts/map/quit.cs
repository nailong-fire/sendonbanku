using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    void Update()
    {
        // 检测ESC键按下
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            
            // 在编辑器中停止运行
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}



