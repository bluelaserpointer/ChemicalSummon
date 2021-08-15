using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FirstMoverDecider : MonoBehaviour
{
    [SerializeField]
    List<Substance> PurePool, CompoundPool;
    [SerializeField]
    Transform cardParentTf;
    [SerializeField]
    SubstanceCard substanceCardPrefab;
    [SerializeField]
    Text middleText;
    [SerializeField]
    TranslatableSentenceSO firstMoverSentence, secondMoverSentence;

    //data
    SubstanceCard drawedCard;
    bool playerIsFirstMover;
    [HideInInspector]
    public bool mustCorrect;
    public void Draw()
    {
        drawedCard = Instantiate(substanceCardPrefab, cardParentTf);
        drawedCard.Substance = (Random.Range(0, 2) == 0 ? PurePool : CompoundPool).GetRandomElement();
        drawedCard.invokeCardInfo = false;
        drawedCard.SetDraggable(false);
        drawedCard.transform.eulerAngles = new Vector3(0, 180, 0);
    }
    public void Guess(bool isPure)
    {
        if(mustCorrect)
        {
            drawedCard.Substance = (isPure ? PurePool : CompoundPool).GetRandomElement();
            drawedCard.InitCardAmount(1);
            mustCorrect = false;
        }
        playerIsFirstMover = isPure == drawedCard.Substance.IsPureElement;
        drawedCard.TraceRotation(Vector3.zero, () =>
        {
            middleText.text = playerIsFirstMover ? firstMoverSentence : secondMoverSentence;
            Invoke("EndDecision", 1.5F);
        });
    }
    public void EndDecision()
    {
        gameObject.SetActive(false);
        MatchManager.SetFirstMover(playerIsFirstMover ? (Gamer)MatchManager.Player : MatchManager.Enemy);
    }
}
