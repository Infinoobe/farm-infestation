using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ShouldChangeTarget", story: "Should [Self] change [currTarget]", category: "Conditions", id: "f018259d600e037522ab5ac8c5f7c358")]
public partial class ShouldChangeTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> CurrTarget;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
