using UnityEngine;
using UnityEngine.UI;

public class DEBUGCommands : MonoBehaviour
{
    public static DEBUGCommands instance = null;

    public InputField input = null;
    public Text console = null;
    public ScrollRect scrollRect = null;

    private const float scrollSize = 15;
    private float toMove = 0;
    private float verticalScrollTotal = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        console.text = "";
    }

    public void UseCommand(string aCommand)
    {
        input.text = aCommand;
        CheckCommand();
    }

    public void CheckCommand()
    {
        string text = input.text.ToLower();

        verticalScrollTotal += toMove;
        scrollRect.content.anchoredPosition = Vector2.up * verticalScrollTotal;

        string[] commands = text.Split(' ');
        if (commands[0] == "add")
        {
            if (commands[1] == "gold")
            {
                InventoryManager.instance.AddGold(float.Parse(commands[2]));
                console.text += "Added " + commands[2] + " gold\n";
                toMove = scrollSize;
            }
            else if (commands[1] == "horse")
            {
                InventoryManager.instance.horseSpeed = 2;
                console.text += "Set horseSpeed to 2\n";
                toMove = scrollSize;
            }
        }
        else if (commands[0] == "remove")
        {
            if (commands[1] == "gold")
            {
                InventoryManager.instance.AddGold(float.Parse(commands[2]) * -1);
                console.text += "Removed " + commands[2] + " gold\n";
                toMove = scrollSize;
            }
            else if (commands[1] == "horse")
            {
                InventoryManager.instance.horseSpeed = 1;
                console.text += "Set horseSpeed to 1\n";
                toMove = scrollSize;
            }
        }
        else if (commands[0] == "help")
        {
            console.text += "Command : Add, Remove \n Add : Gold (float), Horse \n";
            toMove = scrollSize * 2;
        }
        else
        {
            console.text += "UNKNOWN COMMAND :" + text + "\n";
            toMove = scrollSize;
        }

        input.text = "";
    }
}
