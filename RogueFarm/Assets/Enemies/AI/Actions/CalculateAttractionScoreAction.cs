using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CalculateAttractionScore", story: "Calculate [self] to [object] attraction score into [attractionScore]", category: "Action", id: "1fd450a28bf0a4e28a100c6d20c51692")]
public partial class CalculateAttractionScoreAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    [SerializeReference] public BlackboardVariable<float> AttractionScore;
    
    protected override Status OnStart()
    {
        if (Self.Value == null || Object.Value == null)
        {
            AttractionScore.Value = 999999999;
            return Status.Success;
        }
            
        float dist = Vector3.Distance(
            Self.Value.transform.position,
            Object.Value.transform.position
        );

        float factor = 1f;

        if (Object.Value.TryGetComponent<Player>(out Player player))
            factor = player.EnemyAttractionFactor;
        else if (Object.Value.TryGetComponent<Building>(out Building building))
            factor = building.EnemyAttractionFactor;
        else if (Object.Value.TryGetComponent<Plant>(out Plant plant))
            factor = plant.EnemyAttractionFactor;

        AttractionScore.Value = dist * factor;

        return Status.Success;
    }
}

