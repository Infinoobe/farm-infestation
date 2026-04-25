using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "HasTarget", story: " [CurrentTarget] is not null", category: "Conditions", id: "8b3125460e64433165ccae5ba9955754")]
public partial class HasTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> CurrentTarget;

    public override bool IsTrue()
    {
        return CurrentTarget != null && CurrentTarget.Value != null;
    }

    //public override void OnStart()
    //{
    //}

    //public override void OnEnd()
    //{
    //}
}
