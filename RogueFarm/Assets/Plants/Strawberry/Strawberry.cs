using UnityEngine;

public class Strawberry : Plant
{
    [SerializeField] int midGrow = 2;

    public override void Grow()
    {
        base.Grow();
        if(currGrowthTime == midGrow)
        {
            isGrown = true;
        }
    }

    public override void CollectItem()
    {
        base.CollectItem();
        isGrown = false;
        SetUpObjectVisual();
    }
}
