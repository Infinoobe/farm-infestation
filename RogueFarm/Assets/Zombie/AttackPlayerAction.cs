using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackPlayer", story: "Agent attacks [Player]", category: "Action", id: "e0758f0f6070ce9b305e1a4c6d14d997")]
public partial class AttackPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<Player> Player;

    protected override Status OnStart()
    {
        Player.Value.DealDamage(10);
        return Status.Success;
    }

    // protected override Status OnUpdate()
    // {
    //     return Status.Success;
    // }

    // protected override void OnEnd()
    // {
    // }
}

