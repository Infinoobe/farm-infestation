using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsTargetPlayer", story: "Is [currTarget] a [Player]", category: "Conditions", id: "e76eb9cd4272678cd8b8d7d6abaa5838")]
public partial class IsTargetPlayerCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> CurrTarget;
    [SerializeReference] public BlackboardVariable<Player> Player;

    public override bool IsTrue()
    {
        return CurrTarget.Value != null &&
           CurrTarget.Value.TryGetComponent<Player>(out _);
    }
}
