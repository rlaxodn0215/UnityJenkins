using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField]
    private int currentGold = 100;

    public int CurrentGold
    {
        get
        {
            return currentGold;
        }
        set
        {
            currentGold = Mathf.Max(0, value);
        }
    }

}
