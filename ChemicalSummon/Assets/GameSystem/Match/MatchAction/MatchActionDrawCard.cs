using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchActionDrawCard : MatchAction
{
    [SerializeField]
    [Min(1)]
    int amount = 1;

    public override bool CanAction(Gamer gamer)
    {
        return gamer.DrawPile.Count >= amount;
    }

    public override void DoAction(Gamer gamer, Action afterAction = null, Action cancelAction = null)
    {
        for(int i = 0; i < amount; ++i)
            gamer.DrawCard();
        afterAction.Invoke();
    }

    protected override string GetDescription()
    {
        return ChemicalSummonManager.LoadTranslatableSentence("DrawXCard").ToString().Replace("$amount", amount.ToString());
    }
}
