using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // 点击开始按钮时调用
    public void StartGame()
    {
        // 确保你的 Build Settings 中已经添加了名为 "start" 的场景
        SceneManager.LoadScene("start");
    }

    // 点击退出按钮时调用
    public void QuitGame()
    {
        // 在编辑器中输出日志，方便调试
        Debug.Log("退出游戏！");
        
        // 退出应用程序
        Application.Quit();
    }
}
