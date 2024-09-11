using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsBuildTower { get; set; }

    private void Awake()
    {
        IsBuildTower = false;
    }
}
