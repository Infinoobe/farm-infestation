using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ShouldChangeTarget", story: "Should agent change [currTarget]", category: "Conditions", id: "f018259d600e037522ab5ac8c5f7c358")]
public partial class ShouldChangeTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> CurrTarget;

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
