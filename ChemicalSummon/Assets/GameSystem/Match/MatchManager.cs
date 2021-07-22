﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// 所有战斗内公用功能的集合
///  - 寻找物体(我方手牌、我方信息栏...)
///  , 生成卡牌
///  , 管理回合
/// </summary>
[DisallowMultipleComponent]
public class MatchManager : ChemicalSummonManager
{
    public static MatchManager instance;

    //inspector

    [Header("Field")]
    [SerializeField]
    CardField myField;
    [SerializeField]
    CardField enemyField;
    [SerializeField]
    MySideStatusUI mySideStatusUI;
    [SerializeField]
    EnemySideStatusUI enemySideStatusUI;

    [Header("Info")]
    [SerializeField]
    Text matchNameText;
    [SerializeField]
    CardInfoDisplay cardInfoDisplay;
    [SerializeField]
    FusionPanel fusionPanel;

    [Header("Turn")]
    public Text turnText;
    public UnityEvent onMyTurnStart;
    public UnityEvent onEnemyTurnStart;
    public Animator animatedTurnPanel;

    [Header("Demo&Test")]
    public UnityEvent onInit;

    //data
    /// <summary>
    /// 当前战斗
    /// </summary>
    public Match Match => PlayerSave.ActiveMatch;
    /// <summary>
    /// 环境温度
    /// </summary>
    public static float DefaultTempreture => 27.0f;
    public Gamer myGamer, enemyGamer;
    /// <summary>
    /// 我方玩家
    /// </summary>
    public static Gamer MyGamer => instance.myGamer;
    /// <summary>
    /// 敌方玩家
    /// </summary>
    public static Gamer EnemyGamer => instance.enemyGamer;
    /// <summary>
    /// 我方场地
    /// </summary>
    public static CardField MyField => instance.myField;
    /// <summary>
    /// 敌方场地
    /// </summary>
    public static CardField EnemyField => instance.enemyField;
    /// <summary>
    /// 我方手牌
    /// </summary>
    public static HandCardsArrange MyHandCards => MySideStatusUI.HandCards;
    /// <summary>
    /// 我方信息栏
    /// </summary>
    public static MySideStatusUI MySideStatusUI => instance.mySideStatusUI;
    /// <summary>
    /// 敌方信息栏
    /// </summary>
    public static EnemySideStatusUI EnemySideStatusUI => instance.enemySideStatusUI;
    int turn;
    /// <summary>
    /// 卡牌信息栏
    /// </summary>
    public static CardInfoDisplay CardInfoDisplay => instance.cardInfoDisplay;
    /// <summary>
    /// 回合
    /// </summary>
    public static int Turn
    {
        get => instance.turn;
        set
        {
            if (instance.turn != value)
            {
                instance.turn = value;
                instance.turnText.text = "Turn " + value;
                instance.isMyTurn = !instance.isMyTurn;
                instance.animatedTurnPanel.GetComponentInChildren<Text>().text = instance.isMyTurn ? "你的回合" : "敌方回合";
                instance.animatedTurnPanel.GetComponent<AnimationStopper>().Play();
            }
        }
    }
    bool isMyTurn = true;
    /// <summary>
    /// 是我方回合
    /// </summary>
    public static bool IsMyTurn => instance.isMyTurn;

    private void Awake()
    {
        Init();
        instance = this;
        //set background and music
        matchNameText.text = Match.Name;
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = Match.PickRandomBGM();
        audioSource.Play();
        Instantiate(Match.BackGround);
        //gamer
        myGamer = new Gamer(Match.MySideCharacter);
        myGamer.deck = new Deck(PlayerSave.ActiveDeck);
        enemyGamer = new Gamer(Match.EnemySideCharacter);
        enemyGamer.deck = new Deck(Match.EnemyDeck);
        MySideStatusUI.Gamer = myGamer;
        EnemySideStatusUI.Gamer = enemyGamer;
        onMyTurnStart.AddListener(MySideStatusUI.OnTurnStart);
        onEnemyTurnStart.AddListener(EnemySideStatusUI.OnTurnStart);
        MyField.SetInteractable(true);
        EnemyField.SetInteractable(false);
        MyField.cardsChanged.AddListener(fusionPanel.UpdateList);
        MySideStatusUI.OnHandCardsChanged.AddListener(fusionPanel.UpdateList);
        //demo
        onInit.Invoke();
        //initial draw
        for (int i = 0; i < 5; ++i)
        {
            MySideStatusUI.DrawCard();
            EnemySideStatusUI.DrawCard();
        }
    }
    /// <summary>
    /// 获取该游戏者的场地
    /// </summary>
    /// <param name="gamer"></param>
    /// <returns></returns>
    public static Field GetField(Gamer gamer)
    {
        if (gamer.IsMe)
            return MyField;
        if (gamer.IsEnemy)
            return EnemyField;
        return null;
    }
    /// <summary>
    /// 获取该场地的游戏者
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public static Gamer GetGamer(Field field)
    {
        if (field.IsMine)
            return MyGamer;
        if (field.IsEnemies)
            return EnemyGamer;
        return null;
    }
    /// <summary>
    /// 结束回合
    /// </summary>
    public static void TurnEnd() {
        instance.TurnEnd_nonstatic();
    }
    /// <summary>
    /// 结束回合非静态函数(用于按钮事件)
    /// </summary>
    public void TurnEnd_nonstatic()
    {
        ++Turn;
        (IsMyTurn ? onMyTurnStart : onEnemyTurnStart).Invoke();
    }
}
