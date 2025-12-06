using UnityEngine;

public interface ICharacterUI
{
    void UpdateHopeUI(int currentHope, int maxHope, int changeAmount);
    void UpdateFaithUI(int currentFaith, int maxFaith, int changeAmount);
    //void UpdateHandCountUI(int handCount, int maxHandSize);
    //void UpdateDeckCountUI(int deckCount);
    //void ShowFloatingText(Vector3 worldPosition, string text, Color color);
    //void ShowDamageEffect(CardEntity target, int damage);
    //void ShowHealEffect(CardEntity target, int healAmount);
    void ShowTurnIndicator(bool isMyTurn);
    //void UpdateCharacterName(string name);
}