using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCommands : MonoBehaviour
{
    public TextMeshProUGUI output;
    public TMP_InputField playerInput;

    // ✅ Declare commandMap here so it is accessible throughout the class
    private Dictionary<string, System.Action<string[]>> commandMap;

    void Start()
    {
        // Initialize command map with method references
        // NEW ACTIONS ADD HERE:
        commandMap = new Dictionary<string, System.Action<string[]>>()
        {
            { "/talk", Talk },
            { "/flirt", Flirt },
            { "/give_gift", GiveGift },
            { "/check_status", CheckStatus }
            /*
                Love Language Essentials:
                /touch
                /compliment
                /gift give

                Attachment Style Essentials:
                /reassure
                /??

                Personality Essentials: Do we need?
            */
        };
    }

    // ✅ Parse player input and dispatch command
    public void parseInput()
    {
        string input = playerInput.text.Trim(); // Gets ride white space on the side
        output.text = ""; // Clear previous output

        if (string.IsNullOrEmpty(input)) return; //Nothing in input

        string[] tokens = input.Split(' '); // "/hello dad" ==> ["/hello","dad", ...]
        string command = tokens[0].ToLower(); //get command and lowers it, not-case sensitive

        string[] args = new string[tokens.Length - 1];
        // /action arg1 arg2 arg3 ...
        for (int i = 1; i < tokens.Length; i++) args[i - 1] = tokens[i]; //rest of args

        // Command Activation:
        if (commandMap.ContainsKey(command))
        {
            commandMap[command].Invoke(args);
        }
        else
        {
            output.text = "Unknown command. Try /talk, /flirt, /give_gift, /check_status.";
        }
    }

    /* Command Initializations and Templates:
        How do we want to handle each action? 
    */ 

    private void Talk(string[] args)
    {
        output.text = $"You talk to {(args.Length > 0 ? args[0] : "someone")} (placeholder response).";
    }

    private void Flirt(string[] args)
    {
        output.text = $"You flirt with {(args.Length > 0 ? args[0] : "someone")} (placeholder response).";
    }

    private void GiveGift(string[] args)
    {
        string target = args.Length > 0 ? args[0] : "someone";
        string gift = args.Length > 1 ? args[1] : "something";
        output.text = $"You give {gift} to {target} (placeholder response).";
    }

    private void CheckStatus(string[] args)
    {
        string target = args.Length > 0 ? args[0] : "someone";
        output.text = $"{target} seems distant but curious. (placeholder status)";
    }


    // Need NPC Metadata Entry-point for Phycological Frameworks
    public void actionApprasial() { }


    //Demo-Entry: 
    public void ButtonDemo()
    {
        output.text = playerInput.text;
    }
}
