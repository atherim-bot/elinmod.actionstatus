using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace ActionStatus;

public static class ModInfo
{
    public const string Guid = "atherim.actionstatus";
    public const string Name = "Action Status";
    public const string Version = "1.0.1";
}

[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
internal class ActionStatusMod : BaseUnityPlugin
{
    private static ActionStatusMod? Instance { get; set; }

    private string? _previousAction;
    private bool _isSleeping;
    private bool _isInCombat;

    private void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModInfo.Guid);
    }

    private void Update()
    {
        if (!Core.Instance.IsGameStarted || EMono.player == null || EMono.player.chara == null) return;
        
        var playerChara = EMono.player.chara;
        var playerAction = playerChara.GetActionText();
        
        switch (_isSleeping)
        {
            case false when playerChara.conSleep != null:
                _isSleeping = true;
                WidgetStatsBarPatch.ActionNotification!.text = "Sleeping";
                return;
            case true when playerChara.conSleep == null:
                _isSleeping = false;
                WidgetStatsBarPatch.ActionNotification!.text = "Idle";
                return;
        }

        switch (_isInCombat)
        {
            case false when playerChara.IsInCombat:
                Log("go to combat");
                _isInCombat = true;
                WidgetStatsBarPatch.ActionNotification!.text = "Fighting!";
                WidgetStatsBarPatch.ActionNotification.Refresh();
                return;
            case true when !playerChara.IsInCombat:
                Log("out of combat");
                _isInCombat = false;
                WidgetStatsBarPatch.ActionNotification!.text = "Idle";
                return;
        }
        
        if (playerAction == _previousAction || _isSleeping || _isInCombat) return;

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