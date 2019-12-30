using System;
using RoR2;
using RoR2.ConVar;
using Run = On.RoR2.Run;

namespace RoR2Pro.Modules
{
    public class SkipTeleporterWaitingPeriod : IModule
    {
        public Action Initialize()
        {
            var enabled = RoR2.Console.instance
                              .FindConVar(new EnableSkipTeleporterWaitingPeriod().name)
                              .GetString() == "1";

            Chat.AddMessage($"Skip teleporter waiting period enabled: {enabled}");

            if (!enabled)
            {
                return () => { };
            }

            Run.OnServerBossDefeated += OnServerBossDefeated;

            return () => { Run.OnServerBossDefeated -= OnServerBossDefeated; };
        }

        private void OnServerBossDefeated(Run.orig_OnServerBossDefeated orig, RoR2.Run self, BossGroup bossGroup)
        {
            var remainingChargeTimer = RoR2.TeleporterInteraction.instance.remainingChargeTimer;

            if (remainingChargeTimer > 0)
            {
                var diff = (float) (remainingChargeTimer * 0.1);

                self.fixedTime += diff;
                self.time += diff;
                TeleporterInteraction.instance.remainingChargeTimer = diff;
            }
        }
    }

    public class EnableSkipTeleporterWaitingPeriod : BoolConVar
    {
        public EnableSkipTeleporterWaitingPeriod() : base("pro_enable_skip_teleporter_waiting_period",
            ConVarFlags.Archive | ConVarFlags.SenderMustBeServer, "1", "Skip teleporter waiting period")
        {
        }
    }
}