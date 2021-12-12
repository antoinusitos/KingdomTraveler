using UnityEngine;

public class ButtonTab : MonoBehaviour
{
    public void ShowPage(int page)
    {
        UIManager.instance.ActivatePage(page);
    }
}
