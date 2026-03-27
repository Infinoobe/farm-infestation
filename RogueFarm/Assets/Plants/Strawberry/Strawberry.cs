
public class Strawberry : Plant
{
    public override void CollectItem()
    {
        base.CollectItem();
        GameState.Instance.Player.Heal(20);
    }
}
