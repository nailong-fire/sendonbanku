// === 1. 资源系统 ===
using System;

[System.Serializable]
public class ResourceSystem
{
    public int MaxHope = 8;
    private int _currentHope;
    
    private int _faith;
    public int Faith => _faith;
    
    public int CurrentHope
    {
        get => _currentHope;
        set
        {
            _currentHope = Math.Clamp(value, 0, MaxHope);
            OnHopeChanged?.Invoke(_currentHope);
        }
    }

    // 根据hope计算faith增长量的委托
    public delegate int FaithGrowthCalculator(int currentHope, int maxHope);
    public FaithGrowthCalculator CalculateFaithGrowth;

    public event Action<int> OnHopeChanged;
    public event Action<int> OnFaithChanged;

    public ResourceSystem()
    {
        CurrentHope = MaxHope;
        _faith = 0;
        
        // 默认faith增长曲线：hope中间值增长慢，两端增长快
        CalculateFaithGrowth = (hope, maxHope) =>
        {
            float normalized = (float)hope / maxHope;
            // 抛物线曲线：在hope=0.5时增长最慢，两端增长快
            float growthFactor = 1f + MathF.Abs(normalized - 0.5f) * 2f;
            return Mathf.RoundToInt(growthFactor);
        };
    }

    public bool CanAfford(int faithCost) => _faith >= faithCost;

    public bool SpendFaith(int amount)
    {
        if (_faith >= amount)
        {
            _faith -= amount;
            OnFaithChanged?.Invoke(_faith);
            return true;
        }
        return false;
    }

    public void GainFaith(int amount)
    {
        _faith += amount;
        OnFaithChanged?.Invoke(_faith);
    }

    public void EndTurnGainFaith()
    {
        int growth = CalculateFaithGrowth(CurrentHope, MaxHope);
        GainFaith(growth);
    }
}