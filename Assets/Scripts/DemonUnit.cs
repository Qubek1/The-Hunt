using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemonUnit : Unit
{
    public Button abilityButton;
    public TurnState abilityState;
    float HideTime;
    bool hide = true;

    protected override void UnitUpdate()
    {
        base.UnitUpdate();
        if (Time.time > HideTime && abilityButton.IsActive() && hide)
        {
            abilityButton.gameObject.SetActive(false);
        }
    }

    public void ShowAbilityButton(bool t)
    {
        hide = !t;
        if (t)
            abilityButton.gameObject.SetActive(t);
        else
            HideTime = Time.time + 0.1f;
    }

    public void CastAbility()
    {
        abilityButton.gameObject.SetActive(false);
        hide = true;
        //ShowAbilityButton(false);
        abilityState.StartState();
    }
}
