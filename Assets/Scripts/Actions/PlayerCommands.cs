using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

public class PlayerCommands : MonoBehaviour
{
    public TextMeshProUGUI output;
    public TMP_InputField playerInput;
    public PlayerInteraction player;

    private Dictionary<string, System.Action<string[]>> commandMap;

    void Start()
    {
        commandMap = new Dictionary<string, System.Action<string[]>>()
        {
            //Interpreter Mapping:
            { "/talk", args => HandleCommand("/talk", args) },
            { "/flirt", args => HandleCommand("/flirt", args) },
            { "/give_gift", args => HandleCommand("/give_gift", args) },
            { "/check_status", args => HandleCommand("/check_status", args) }
        };
    }

    public void parseInput()
    {
        string input = playerInput.text.Trim();
        output.text = "";

        if (string.IsNullOrEmpty(input)) return;

        string[] tokens = input.Split(' ');
        string command = tokens[0].ToLower();

        string[] args = new string[tokens.Length - 1];
        for (int i = 1; i < tokens.Length; i++) args[i - 1] = tokens[i];

        //Call Interpreter:
        if (commandMap.ContainsKey(command))
        {
            commandMap[command].Invoke(args);
        }
        else
        {
            output.text = "Unknown command. Try /talk, /flirt, /give_gift, /check_status.";
        }
    }

    //Give Command to NPC
    private void HandleCommand(string command, string[] args)
    {
        if (player == null || player.GetNearbyNPC() == null)
        {
            output.text = "No one nearby to interact with.";
            return;
        }

        NPC npc = player.GetNearbyNPC();
        output.text = npc.ReceiveCommand(command, args);
    }
}
