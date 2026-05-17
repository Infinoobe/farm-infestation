using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "SwitchTargetFromPlayer", story: "Should [self] abandon attacking [player]", category: "Conditions", id: "fe0dce2763a7dd43e9df34d2a6211a85")]
public partial class SwitchTargetFromPlayerCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Player> Player;

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
