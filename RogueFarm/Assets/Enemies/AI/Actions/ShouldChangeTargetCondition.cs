using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ShouldChangeTarget", story: "Should [Self] change [currTarget] with [PlayerAttractionScore] [PlantAttractionScore] [BuildingAttractionScore] [Player]", category: "Conditions", id: "f018259d600e037522ab5ac8c5f7c358")]
public partial class ShouldChangeTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> CurrTarget;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> PlayerAttractionScore;
    [SerializeReference] public BlackboardVariable<float> PlantAttractionScore;
    [SerializeReference] public BlackboardVariable<float> BuildingAttractionScore;
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
