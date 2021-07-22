﻿using System.Collections;
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
    public override void OnFusionTurnStart()
    {
        base.OnFusionTurnStart(); //card draw
        CardSlot[] slots = Field.Slots;
        //back all cards
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
        List<SubstanceCardAndATK> highestATKs = new List<SubstanceCardAndATK>();
        foreach(SubstanceCard card in HandCards)
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
        //place strongest card
        List<CardSlot> lestEmptySlots = new List<CardSlot>(slots);
        foreach(SubstanceCardAndATK highestATKPair in highestATKs)
        {
            SubstanceCard card = highestATKPair.card;
            RemoveHandCard(card);
            foreach(CardSlot slot in lestEmptySlots)
            {
                if (!card.GetStateInTempreture(slot.Tempreture).Equals(ThreeState.Solid))
                    continue;
                slot.SlotSet(highestATKPair.card.gameObject);
                lestEmptySlots.Remove(slot);
                break;
            }
            if (lestEmptySlots.Count == 0)
                break;
        }
        //end turn
        MatchManager.TurnEnd();
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
        if(generatedAttackSign != null)
            Destroy(generatedAttackSign.gameObject);
        foreach (CardSlot slot in Field.Slots)
        {
            if (slot.IsEmpty || attackedSlot.Contains(slot))
                continue;
            attackedSlot.Add(slot);
            generatedAttackSign = Instantiate(MatchManager.AttackButtonPrefab, MatchManager.MainCanvas.transform);
            generatedAttackSign.transform.position = slot.transform.position;
            generatedAttackSign.SetDirection(false);
            MatchManager.Player.Defense(slot.Card);
            return;
        }
        //no more slot can attack
        MatchManager.TurnEnd();
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
