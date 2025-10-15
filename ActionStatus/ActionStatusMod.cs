using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace ActionStatus;

public static class ModInfo
{
    public const string Guid = "atherim.actionstatus";
    public const string Name = "Action Status";
    public const string Version = "1.0.0";
}

[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
internal class ActionStatusMod : BaseUnityPlugin
{
    private static ActionStatusMod? Instance { get; set; }

    private string? _previousAction;

    private void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModInfo.Guid);
    }

    private void Update()
    {
        if (!Core.Instance.IsGameStarted) return;

        var playerAction = EMono.player.chara.GetActionText();
        
        if (playerAction == _previousAction) return;

        _previousAction = playerAction;
        
        WidgetStatsBarPatch.ActionNotification!.text = MapToFriendyText(playerAction);
    }

    private static string MapToFriendyText(string playerAction)
    {
        // Known actions: Void, Go to, Collect (Woodcutting, Mining, Picking plants), Dig, Plow, Fishing, Place, Eat
        return playerAction switch
        {
            "Void" => new System.Random().Next(10) == 0 ? "Contemplating about life" : "Idle",
            _ => playerAction
        };
    }

    internal static void Log(object payload)
    {
        Instance!.Logger.LogInfo(payload);
    }
}