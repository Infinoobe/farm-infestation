using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "currTarget assigned", story: "Agent has assigned [currTarget]", category: "Conditions", id: "27b4847eac2d317642e2e0ac82b4d346")]
public partial class CurrTargetAssignedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> CurrTarget;
    public override bool IsTrue()
    {
        return CurrTarget != null && CurrTarget.Value != null;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
