using HarmonyLib;

namespace ActionStatus;

[HarmonyPatch]
public class WidgetStatsBarPatch
{
    
    public static BaseNotification? ActionNotification;

    [HarmonyPostfix, HarmonyPatch(typeof(WidgetStats), nameof(WidgetStats._OnActivate))]
    public static void AddActionStatus()
    {
        ActionNotification = new BaseNotification
        {
            item = null,
            text = EMono.player.chara.GetActionText(),
            lastText = null
        };

        WidgetStats.Instance.Add(ActionNotification);
    }
    
}