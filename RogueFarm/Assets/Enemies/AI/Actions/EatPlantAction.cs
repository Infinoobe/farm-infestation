using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Eat Plant", story: "Agent eats plant [Plant]", category: "Action", id: "1098e4349eaccdfcb9ea203974db999d")]
public partial class EatPlantAction : Action
{

    [SerializeReference] public BlackboardVariable<Plant> Plant;

    protected override Status OnStart()
    {
        Plant.Value.TakeDamage(10);
        return Status.Success;
    }
    //
    // protected override Status OnUpdate()
    // {
    //     return Status.Success;
    // }
    //
    // protected override void OnEnd()
    // {
    // }
}

