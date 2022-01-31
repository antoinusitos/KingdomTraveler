using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;

    private bool waitingForInput = false;
    private bool inputDone = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(waitingForInput && Input.GetMouseButtonDown(0))
        {
            inputDone = true;
            waitingForInput = false;
        }
    }

    public void SetWaitingForInput()
    {
        waitingForInput = true;
    }

    public bool GetInputDone()
    {
        if(inputDone)
        {
            inputDone = false;
            return true;
        }
        return false;
    }
}
