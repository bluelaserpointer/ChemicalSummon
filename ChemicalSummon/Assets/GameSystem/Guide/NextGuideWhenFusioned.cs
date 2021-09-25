using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class NextGuideWhenFusioned : NextGuide
{
    [SerializeField]
    List<Reaction> reactions;
    protected override void OnClose()
    {
        MatchManager.Instance.onFusionFinish.AddListener(CheckFusion);
    }
    private void CheckFusion()
    {
        Reaction reaction = MatchManager.FusionPanel.LastReaction;
        if (reactions.Contains(reaction))
        {
            ShowNextGuide();
            MatchManager.Instance.onFusionFinish.RemoveListener(CheckFusion);
        }
    }
}