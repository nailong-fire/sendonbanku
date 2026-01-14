using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 把两个按钮绑定到 SimpleBattleManager 的 Win/Lose。
/// 
/// 用法：挂到 Canvas 下任意物体；拖拽两个 Button 引用。
/// </summary>
public class SimpleBattleUI : MonoBehaviour
{
    [Header("Refs")]
    public Button winButton;
    public Button loseButton;

    private void Awake()
    {
        if (winButton != null)
            winButton.onClick.AddListener(OnWinClicked);

        if (loseButton != null)
            loseButton.onClick.AddListener(OnLoseClicked);
    }

    private void OnDestroy()
    {
        if (winButton != null)
            winButton.onClick.RemoveListener(OnWinClicked);

        if (loseButton != null)
            loseButton.onClick.RemoveListener(OnLoseClicked);
    }

    private void OnWinClicked()
    {
        if (SimpleBattleManager.Instance != null)
            SimpleBattleManager.Instance.Win();
        else
            Debug.LogError("SimpleBattleManager.Instance is null. Ensure one exists in the scene.");
    }

    private void OnLoseClicked()
    {
        if (SimpleBattleManager.Instance != null)
            SimpleBattleManager.Instance.Lose();
        else
            Debug.LogError("SimpleBattleManager.Instance is null. Ensure one exists in the scene.");
    }
}
