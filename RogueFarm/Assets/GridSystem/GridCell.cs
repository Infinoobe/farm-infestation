using UnityEngine;

public class GridCell : MonoBehaviour
{
    private GameObject currBuilding;

    public bool IsEmpty()
    {
        return currBuilding == null;
    }

    public void SetBuilding(GameObject newBuilding)
    {
        currBuilding = newBuilding;
    }

    public void UnSetBuilding()
    {
        currBuilding = null;
    }

    public void RemoveBuilding()
    {
        currBuilding.GetComponent<Building>().RemoveBuilding();
    }

}
