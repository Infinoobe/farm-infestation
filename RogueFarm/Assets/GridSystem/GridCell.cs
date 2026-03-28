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
        if (!currBuilding) return;
        Building currBuildingScript = currBuilding.GetComponent<Building>();
        if (!currBuildingScript || currBuildingScript.CanBeDestroyed) return;
        currBuildingScript.RemoveBuilding();
    }
}
