using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.UI.Dialog;

public class Dialog
{
    internal string speaker;
    internal List<string> lines;
    internal Dictionary<string, string> replies;
    internal List<IAction> actions;
    private Dialog(string speaker, List<string> lines, Dictionary<string, string> replies, List<IAction> actions)
    {
        this.speaker = speaker;
        this.lines = lines;
        this.replies = replies;
        this.actions = actions;
    }

    public static Dialog Create(string dialogKey)
    {
        string interactableDialogPath = $"sorceryFight/Localization/{Language.ActiveCulture.Name}/InteractableDialog.json";
        if (!ModContent.FileExists(interactableDialogPath))
        {
            Main.NewText($"Couldn't find {interactableDialogPath}, defaulting to en-US");

            interactableDialogPath = "sorceryFight/Localization/en-US/InteractableDialog.json";
        }

        using MemoryStream memoryStream = new MemoryStream(ModContent.GetFileBytes(interactableDialogPath));
        string jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());
        var root = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(jsonString);
        if (root == null)
            throw new Exception("Invalid dialog JSON.");

        var parts = dialogKey.Split(".");
        if (parts.Length != 2)
            throw new Exception($"Invalid dialog key: {dialogKey}.");

        string dialogSource = parts[0];
        string dialog = parts[1];

        if (!root.ContainsKey(dialogSource) || !root[dialogSource].ContainsKey(dialog))
            throw new Exception($"Dialog Key {dialogKey} not found.");

        var dialogData = JsonConvert.DeserializeObject<Dictionary<string, object>>(root[dialogSource][dialog].ToString());

        var lines = dialogData["Text"].ToString().Split("\n").ToList();


        // List<string> additionalLines;
        // var additionalLinesData = dialogData["AdditionalText"]?.ToString();
        // if (additionalLinesData != null)
        // {
        //     additionalLines = JsonConvert.DeserializeObject<List<string>>(additionalLinesData);
        // }

        var replies = new Dictionary<string, string>();
        var replyData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(dialogData["Replies"].ToString());
        foreach (var reply in replyData)
        {
            string text = reply["Text"].ToString();
            string response = reply["Response"].ToString();

            replies.Add(text, response);
        }

        var actions = new List<IAction>();
        var actionData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(dialogData["Actions"].ToString());
        foreach (var action in actionData)
        {
            string type = action["Type"]?.ToString() ??
                throw new NullReferenceException($"An action in {dialogKey} doesn't have a 'ActionType' field!");

            string uiText = action["UIText"]?.ToString() ??
                throw new NullReferenceException($"An action in {dialogKey} doesn't have a 'UIText' field!");

            switch (type)
            {
                case "OpenShop":
                    actions.Add(new OpenShopAction(action["ShopName"], uiText));
                    break;
                case "InvokeMethod":
                    actions.Add(new InvokeMethodAction(action["MethodName"], uiText));
                    break;
                case "QueryQuest":
                    actions.Add(new QueryQuestAction(uiText));
                    break;
                case "GiveQuest":
                    actions.Add(new GiveQuestAction(action["QuestName"], uiText));
                    break;
                case "CloseDialog":
                    actions.Add(new CloseDialogAction(uiText));
                    break;
                default:
                    throw new Exception($"No such action type of type '{type}'");
            }
        }

        return new Dialog(dialogSource, lines, replies, actions);
    }
}
