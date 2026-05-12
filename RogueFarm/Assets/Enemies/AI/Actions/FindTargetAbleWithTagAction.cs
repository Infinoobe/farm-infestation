using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindTargetAbleWithTag", story: "Find targetable [object] closest to [self] with tag: [tag]", category: "Action", id: "3cf8678816590db583a9b24ab73b7f0a")]
public partial class FindTargetAbleWithTagAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<string> Tag;
    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            LogFailure("No agent provided.");
            return Status.Failure;
        }

        Vector3 agentPosition = Self.Value.transform.position;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(Tag.Value);
        float closestDistanceSq = Mathf.Infinity;
        GameObject closestGameObject = null;
        foreach (GameObject gameObject in gameObjects)
        {
            float distanceSq = Vector3.SqrMagnitude(agentPosition - gameObject.transform.position);
            if (closestGameObject == null || distanceSq < closestDistanceSq)
            {
                if (gameObject.TryGetComponent<Building>(out Building building) && !building.CanBeTargetedByEnemy) continue;

                closestDistanceSq = distanceSq;
                closestGameObject = gameObject;
            }
        }

        Object.Value = closestGameObject;
        //return Object.Value == null ? Status.Failure : Status.Success;
        return Status.Success;
    }
}

