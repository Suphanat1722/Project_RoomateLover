using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void ToggleGameObject(GameObject target, bool isActive)
    {
        if (target != null)
            target.SetActive(isActive);
    }
}
