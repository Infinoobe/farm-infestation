using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackPlayer", story: "[Self] attacks [Player]", category: "Action", id: "e0758f0f6070ce9b305e1a4c6d14d997")]
public partial class AttackPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<Player> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        var z = Self.Value.GetComponent<Zombie>();
        if (z != null)
        {
            z.animator.Play("Attack");
        }
        else
        {
            var w = Self.Value.GetComponent<Wendigo>();
            if (w != null)
            {
                w.animator.Play("Attack");
            }
        }

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

