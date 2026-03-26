using UnityEngine;

public class GridCell : MonoBehaviour
{
    private GameObject currBuilding;

    public bool IsEmpty()
    {
        return currBuilding == null;
    }

    public void SetBuilding()
    {

    }

    public void RemoveBuilding()
    {
        Destroy(currBuilding.gameObject);
        currBuilding = null;
    }

    public void ShowGizmo(GameObject buildingPrefab)
    {
        if (!IsEmpty()) return;
        currBuilding = Instantiate(buildingPrefab, gameObject.transform.position, Quaternion.identity);
    }


    public void HideGizmo()
    {
        if (IsEmpty()) return;
        Destroy(currBuilding);
        currBuilding = null;
    }
}
