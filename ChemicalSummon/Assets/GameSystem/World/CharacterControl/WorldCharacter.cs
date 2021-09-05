using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WorldCharacter : MonoBehaviour
{
    [SerializeField]
    Collider stepInCollider;
    [SerializeField]
    Collider interactionCollider;

    public Collider StepInCollider => stepInCollider;
    public Collider InteractionCollider => interactionCollider;
}