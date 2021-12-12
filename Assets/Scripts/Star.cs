using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    public int ID = -1;

    public Image imageLinked = null;

    private void Start()
    {
        imageLinked = GetComponent<Image>();
    }
}
