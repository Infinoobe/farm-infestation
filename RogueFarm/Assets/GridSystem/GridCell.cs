using UnityEngine;
using UnityEngine.Events;

public class GridCell : MonoBehaviour
{
    public GameObject grassModel;
    public Material greenGrass;
    public Material deadGrass;
    public GridSystemRuntime myGrid;
    public int treeSeedsInRange = 0;
    public GameObject currBuilding;

    public UnityEvent<GridCell> OnBuildingPlaced = new();
    public UnityEvent<GridCell> OnBuildingRemoved = new();


    private void Start()
    {
        if (treeSeedsInRange > 0) grassModel.GetComponent<MeshRenderer>().material = greenGrass;
        else grassModel.GetComponent<MeshRenderer>().material = deadGrass;
    }

    public void AddTreeSeedInRange()
    {
        treeSeedsInRange += 1;
        if (treeSeedsInRange > 1) return;
        grassModel.GetComponent<MeshRenderer>().material = greenGrass;
    }

    public void SubtractTreeSeedInRange()
    {
        treeSeedsInRange -= 1;
        if (treeSeedsInRange > 0) return;
        grassModel.GetComponent<MeshRenderer>().material = deadGrass;
        // TODO if curr building is field -> destroy?
    }

    public bool IsEmpty()
    {
        return currBuilding == null;
    }

    public void SetBuilding(GameObject newBuilding)
    {
        currBuilding = newBuilding;
        OnBuildingPlaced.Invoke(this);
    }

    public void UnSetBuilding()
    {
        currBuilding = null;
    }

    public void RemoveBuilding()
    {
        if (!currBuilding) return;
        Building currBuildingScript = currBuilding.GetComponent<Building>();
        if (!currBuildingScript) return;
        OnBuildingRemoved.Invoke(this);
        currBuildingScript.DestroyBuilding();
    }
}
