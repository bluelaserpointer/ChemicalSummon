﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FusionPanelButton : MonoBehaviour
{
    [SerializeField]
    TranslatableSentenceSO fusionSentence;
    [SerializeField]
    FusionButton prefabFusionButton;
    [SerializeField]
    Text fusionCountText;
    [SerializeField]
    Image fusionCountImage;
    [SerializeField]
    VerticalLayoutGroup fusionButtonList;
    [SerializeField]
    Transform showListAnchor, hideListAnchor;
    [SerializeField]
    Color noFusionColor, hasFusionColor;
    [SerializeField]
    SBA_FadingExpand newFusionNoticeAnimation;
    [SerializeField]
    AudioClip clickSE;

    Reaction lastReaction;
    public Reaction LastReaction => lastReaction;
    int lastFusionAmount;
    bool showingList;

    private void Start()
    {
        fusionCountImage.color = noFusionColor;
        fusionCountText.text = fusionSentence + " 0";
    }
    public void UpdateList()
    {
        //in counterMode, only counter fusions are avaliable
        SubstanceCard currentAttacker = MatchManager.Player.CurrentAttacker;
        bool counterMode = MatchManager.CurrentTurnType.Equals(TurnType.EnemyAttackTurn) && currentAttacker != null;
        foreach (Transform childTransform in fusionButtonList.transform)
            Destroy(childTransform.gameObject);
        List<Reaction.ReactionMethod> reactionMethods = MatchManager.Player.FindAvailiableReactions();
        foreach (var method in reactionMethods)
        {
            Reaction reaction = method.reaction;
            FusionButton fusionButton = Instantiate(prefabFusionButton, fusionButtonList.transform);
            fusionButton.SetReaction(reaction, counterMode);
            fusionButton.Button.onClick.AddListener(() => {
                MatchManager.FusionDisplay.StartReactionAnimation(() =>
                {
                    lastReaction = reaction;
                    MatchManager.Player.DoReaction(method);
                    //counter fusion
                    if (counterMode)
                    {
                        MatchManager.Player.EndDefence();
                    }
                });
            });
        }
        if(lastFusionAmount < reactionMethods.Count)
        {
            newFusionNoticeAnimation.StartAnimation();
        }
        lastFusionAmount = reactionMethods.Count;
        fusionCountImage.color = lastFusionAmount == 0 ? noFusionColor : hasFusionColor;
        fusionCountText.text = fusionSentence + " " + lastFusionAmount;
    }
    public void OnFusionPanelButtonPress()
    {
        MatchManager.PlaySE(clickSE);
        showingList = !showingList;
        SBA_TracePosition tracer = fusionButtonList.GetComponent<SBA_TracePosition>();
        tracer.SetTarget(showingList ? showListAnchor : hideListAnchor);
        tracer.StartAnimation();
    }
    public void HideFusionList()
    {
        if(showingList)
        {
            SBA_TracePosition tracer = fusionButtonList.GetComponent<SBA_TracePosition>();
            tracer.SetTarget(hideListAnchor);
            tracer.StartAnimation();
            showingList = false;
        }
    }
}
