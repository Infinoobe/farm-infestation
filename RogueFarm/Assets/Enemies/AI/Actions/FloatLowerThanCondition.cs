using System;
using Unity.Behavior;
using Unity.Mathematics;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Float lower than", story: "[Float] lower than [float2]", category: "Conditions", id: "c97c6ba11272dd1c35c85433a82951ac")]
public partial class FloatLowerThanCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> Float;
    [SerializeReference] public BlackboardVariable<float> Float2;

    public override bool IsTrue()
    {
        return Float.Value <= Float2.Value;
    }
}
