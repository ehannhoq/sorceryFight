using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    internal string actionName;
    private Dialog(string speaker, List<string> lines, Dictionary<string, string> replies, string actionName)
    {
        this.speaker = speaker;
        this.lines = lines;
        this.replies = replies;
        this.actionName = actionName;
    }

    public static Dialog Create(string dialogKey)
    {
        string interactableDialogPath = $"sorceryFight/Content/UI/Dialog/{Language.ActiveCulture.Name}.InteractableDialog";

        if (!ModContent.FileExists(interactableDialogPath))
        {
            interactableDialogPath = "sorceryFight/Content/UI/Dialog/en_US.InteractableDialog";
        }
        interactableDialogPath += ".json";

        using MemoryStream memoryStream = new MemoryStream(ModContent.GetFileBytes(interactableDialogPath));
        string jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());
        var root = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(jsonString);
        if (root == null)
            throw new Exception("Invalid dialog JSON.");

        var parts = dialogKey.Split(".");
        if (parts.Length != 2)
            throw new Exception($"Invalid dialog key: {dialogKey}.");

        string container = parts[0];
        string dialog = parts[1];

        if (!root.ContainsKey(container) || !root[container].ContainsKey(dialog))
            throw new Exception($"Dialog Key {dialogKey} not found.");



        var dialogData = JsonConvert.DeserializeObject<Dictionary<string, object>>(root[container][dialog].ToString());

        var speaker = dialogData["Speaker"].ToString();
        var lines = dialogData["Text"].ToString().Split("\n").ToList();

        var replies = new Dictionary<string, string>();
        var replyData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(dialogData["Replies"].ToString());
        foreach (var reply in replyData)
        {
            string text = reply["Text"].ToString();
            string response = reply["Response"].ToString();

            replies.Add(text, response);
        }

        var actionName = dialogData["Action"]?.ToString();

        if (actionName == null)
        {
            actionName = string.Empty;
        }

        return new Dialog(speaker, lines, replies, actionName);
    }
}
