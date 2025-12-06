using UnityEngine;
using System;

[System.Serializable]
public class ResourceSystem
{
    [Header("Hope设置")]
    public int maxHope = 8;
    [SerializeField] private int _currentHope = 8;
    
    [Header("Faith设置")]
    [SerializeField] private int _currentFaith = 0;
    public int maxFaith = 20;
    
    // 事件
    public event Action<int> OnHopeChanged;
    public event Action<int> OnFaithChanged;

    // 添加UI事件
    public event System.Action<int, int, int> OnHopeChangedWithDetails;  // (当前值, 变化量)
    public event System.Action<int, int, int> OnFaithChangedWithDetails; // (当前值, 变化量)
    
    // 属性
    public int CurrentHope
    {
        get => _currentHope;
        set
        {
            int oldValue = _currentHope;
            _currentHope = Mathf.Clamp(value, 0, maxHope);
            int changeAmount = _currentHope - oldValue;
            
            OnHopeChanged?.Invoke(_currentHope);
            OnHopeChangedWithDetails?.Invoke(_currentHope, maxHope, changeAmount);
            
            // 调试日志
            if (changeAmount != 0)
            {
                Debug.Log($"Hope变化: {oldValue} -> {_currentHope} (变化: {changeAmount})");
            }
        }
    }
    
    public int CurrentFaith
    {
        get => _currentFaith;
        set
        {
            int oldValue = _currentFaith;
            _currentFaith = Mathf.Clamp(value, 0, maxFaith);
            int changeAmount = _currentFaith - oldValue;
            
            OnFaithChanged?.Invoke(_currentFaith);
            OnFaithChangedWithDetails?.Invoke(_currentFaith, maxFaith, changeAmount);
            
            if (changeAmount != 0)
            {
                Debug.Log($"Faith变化: {oldValue} -> {_currentFaith} (变化: {changeAmount})");
            }
        }
    }
    
    // 构造函数
    public ResourceSystem()
    {
        _currentHope = maxHope;
        _currentFaith = 0;
    }
    
    public ResourceSystem(int startingHope, int startingFaith)
    {
        maxHope = startingHope;
        _currentHope = startingHope;
        _currentFaith = startingFaith;
    }
    
    // 检查是否有足够Faith
    public bool CanAfford(int cost) => _currentFaith >= cost;
    
    // 消耗Faith
    public bool SpendFaith(int amount)
    {
        if (CanAfford(amount))
        {
            CurrentFaith -= amount;
            return true;
        }
        return false;
    }
    
    // 增加Faith
    public void GainFaith(int amount)
    {
        CurrentFaith += amount;
    }
    
    // 回合结束获得Faith（根据Hope值计算）
    public void EndTurnGainFaith()
    {
        int gain = CalculateFaithGain();
        GainFaith(gain);
    }
    
    // 计算Faith增长（Hope高时增长慢，低时增长快）
    private int CalculateFaithGain()
    {
        float hopeRatio = (float)_currentHope / maxHope;
        
        // Hope在中间时增长最慢，两端增长快
        if (hopeRatio > 0.8f) return 1;  // Hope很高，增长慢
        if (hopeRatio > 0.6f) return 2;  // Hope较高
        if (hopeRatio > 0.4f) return 4;  // Hope中等
        if (hopeRatio > 0.2f) return 2;  // Hope较低
        return 6;  // Hope很低，增长快
    }
    
    // 检查是否存活
    public bool IsAlive => _currentHope > 0;
    
    // 重置资源
    public void ResetResources()
    {
        CurrentHope = maxHope;
        CurrentFaith = 0;
    }
    
    // 复制资源状态
    public ResourceSystem Clone()
    {
        return new ResourceSystem
        {
            maxHope = this.maxHope,
            _currentHope = this._currentHope,
            _currentFaith = this._currentFaith,
            maxFaith = this.maxFaith
        };
    }

    // 添加一个方法用于显示浮动数值
    public void ShowFloatingNumber(Vector3 position, int amount, Color color)
    {
        // 触发事件，让UI管理器显示浮动数值
        OnShowFloatingNumber?.Invoke(position, amount, color);
    }
    
    public event System.Action<Vector3, int, Color> OnShowFloatingNumber;
}