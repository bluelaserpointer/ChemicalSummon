using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DebugScreen : MonoBehaviour
{
    public void UnlockAllReaction()
    {
        foreach (Reaction each in Reaction.GetAll())
        {
            PlayerSave.AddDiscoveredReaction(each);
        }
        MapManager.ReactionScreen.Init();
    }
    public void AllSubstancePlusOne()
    {
        foreach (Substance each in Substance.GetAll())
        {
            if(each.IsPureElement)
                PlayerSave.SubstanceStorage.Add(each);
        }
        MapManager.DeckScreen.Init();
    }
    public void AllItemPlusOne()
    {

    }
}
