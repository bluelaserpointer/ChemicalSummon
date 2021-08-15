﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FusionButton : MonoBehaviour
{
    [SerializeField]
    Button button;
    [SerializeField]
    Text formulaText;
    [SerializeField]
    Transform iconsTf;
    [SerializeField]
    GameObject counterIconPrefab, explosionIconPrefab, heatIconPrefab, electricIconPrefab;
    [SerializeField]
    GameObject newSign;

    //data
    public Button Button => button;
    public Reaction Reaction { get; protected set; }

    public void SetReaction(Reaction reaction, bool isCounter = false)
    {
        Reaction = reaction;
        formulaText.text = reaction.description;
        foreach (Transform each in iconsTf)
            Destroy(each.gameObject);
        if (isCounter)
            Instantiate(counterIconPrefab, iconsTf);
        if (reaction.explosionDamage > 0)
            Instantiate(explosionIconPrefab, iconsTf).GetComponentInChildren<Text>().text = reaction.explosionDamage.ToString();
        if (reaction.electricDamage > 0)
            Instantiate(electricIconPrefab, iconsTf).GetComponentInChildren<Text>().text = reaction.electricDamage.ToString();
        if (reaction.heatDamage > 0)
            Instantiate(heatIconPrefab, iconsTf).GetComponentInChildren<Text>().text = reaction.heatDamage.ToString();
    }
    public void MarkNew(bool cond)
    {
        newSign.gameObject.SetActive(cond);
    }
}
