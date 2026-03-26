using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private Building currBuilding;

    bool isEmpty()
    {
        return currBuilding == null;
    }

    void setBuilding()
    {

    }

    void removeBuilding()
    {
        Destroy(currBuilding.gameObject);
        currBuilding = null;
    }
}
