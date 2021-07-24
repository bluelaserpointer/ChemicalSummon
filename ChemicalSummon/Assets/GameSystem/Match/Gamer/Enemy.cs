﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Enemy : Gamer
{
    [SerializeField]
    Text handCardsAmountText;

    public override void AddHandCard(SubstanceCard substanceCard)
    {
        base.AddHandCard(substanceCard);
        handCardsAmountText.text = GetHandCardCount().ToString();
    }
    public override bool RemoveHandCard(SubstanceCard substanceCard)
    {
        bool b = base.RemoveHandCard(substanceCard);
        handCardsAmountText.text = GetHandCardCount().ToString();
        return b;
    }
    struct SubstanceCardAndATK
    {
        public SubstanceCard card;
        public int atk;
        public SubstanceCardAndATK(SubstanceCard card, int atk)
        {
            this.card = card;
            this.atk = atk;
        }
    }
    List<SubstanceCardAndATK> highestATKs = new List<SubstanceCardAndATK>();
    List<CardSlot> lestEmptySlots = new List<CardSlot>();
    public void OnFusionTurnLoop(int step)
    {
        CardSlot[] slots = Field.Slots;
        switch (step)
        {
            case 0: //back all cards & find highestATK
                MatchManager.MatchLogDisplay.AddAction(() =>
                {
                    foreach (CardSlot slot in slots)
                    {
                        if (!slot.IsEmpty)
                        {
                            SubstanceCard card = slot.Card;
                            slot.SlotClear();
                            AddHandCard(card);
                        }
                    }
                    //find highestATK
                    highestATKs.Clear();
                    foreach (SubstanceCard card in HandCards)
                    {
                        int atk = card.ATK;
                        for (int i = 0; ; ++i)
                        {
                            if (i == highestATKs.Count)
                            {
                                highestATKs.Add(new SubstanceCardAndATK(card, atk));
                                break;
                            }
                            if (atk > highestATKs[i].atk)
                            {
                                highestATKs.Insert(i, new SubstanceCardAndATK(card, atk));
                                break;
                            }
                        }
                    }
                    lestEmptySlots.Clear();
                    lestEmptySlots.AddRange(slots);
                    OnFusionTurnLoop(step + 1);
                });
                break;
            case 1:
                //place strongest card
                bool foundSlot = false;
                for (int i = 0; i < highestATKs.Count; ++i)
                {
                    SubstanceCardAndATK highestATKPair = highestATKs[i];
                    SubstanceCard card = highestATKPair.card;
                    foreach (CardSlot slot in lestEmptySlots)
                    {
                        if (!card.GetStateInTempreture(slot.Tempreture).Equals(ThreeState.Solid))
                            continue;
                        foundSlot = true;
                        highestATKs.RemoveAt(i);
                        MatchManager.MatchLogDisplay.AddAction(() =>
                        {
                            lestEmptySlots.Remove(slot);
                            RemoveHandCard(card);
                            slot.SlotSet(highestATKPair.card.gameObject);
                            OnFusionTurnLoop((highestATKs.Count > 0 && lestEmptySlots.Count > 0) ? 1 : 2);
                        });
                        break;
                    }
                    if(foundSlot)
                    {
                        break;
                    }
                    //no slot can place in
                    highestATKs.RemoveAt(i);
                    --i;
                }
                if(!foundSlot)
                    OnFusionTurnLoop(2);
                break;
            case 2:
                MatchManager.MatchLogDisplay.AddAction(() =>
                {
                    MatchManager.TurnEnd();
                });
                break;
        }
    }
    public override void OnFusionTurnStart()
    {
        base.OnFusionTurnStart(); //card draw
        OnFusionTurnLoop(0);
    }
    public override void OnAttackTurnStart()
    {
        base.OnAttackTurnStart();
        attackedSlot.Clear();
        AttackTurnLoop();
    }
    List<CardSlot> attackedSlot = new List<CardSlot>();
    AttackButton generatedAttackSign;
    public void AttackTurnLoop()
    {
        if (generatedAttackSign != null)
            Destroy(generatedAttackSign.gameObject);
        MatchManager.MatchLogDisplay.AddAction(() =>
        {
            foreach (CardSlot slot in Field.Slots)
            {
                if (slot.IsEmpty || attackedSlot.Contains(slot))
                    continue;
                attackedSlot.Add(slot);
                generatedAttackSign = Instantiate(MatchManager.AttackButtonPrefab, MatchManager.MainCanvas.transform);
                generatedAttackSign.transform.position = slot.transform.position + new Vector3(0, -150, 0);
                generatedAttackSign.SetDirection(false);
                MatchManager.Player.Defense(slot.Card);
                return;
            }
            //no more slot can attack
            MatchManager.TurnEnd();
        });
    }
    public override void Defense(SubstanceCard attacker)
    {
        foreach(CardSlot slot in Field.Slots)
        {
            if (slot.IsEmpty)
                continue;
            slot.Card.Battle(attacker);
            return;
        }
        HP -= attacker.ATK;
    }
}