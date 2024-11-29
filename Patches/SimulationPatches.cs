using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation;
using MelonLoader;

namespace BloonariusMasteryMode.Patches;
using static BloonariusMasteryModeMod;

internal static class SimulationPatches {

    public static List<(int start, int end, double multiplier)> multipliers = new List<(int start, int end, double multiplier)>
    {
        (1, 50, 1.0),
        (51, 60, 0.5),
        (61, 85, 0.2),
        (86, 100, 0.1),
        (101, 120, 0.05),
        (121, int.MaxValue, 0.02)
    };

    public static List<(int start, int end, double multiplier)> getMasteryMultipliers(int roundsEarly){
        return new List<(int start, int end, double multiplier)>{
            (1, 50-roundsEarly, 1.0),
            (51-roundsEarly, 60-roundsEarly, 0.5),
            (61-roundsEarly, 85-roundsEarly, 0.2),
            (86-roundsEarly, 100-roundsEarly, 0.1),
            (101-roundsEarly, 120, 0.05),
            (121, int.MaxValue, 0.02)
        };
    }

    private static double totalRoundCash = 0;

    [HarmonyPatch(typeof(Simulation), "RoundStart")]
    internal static class RoundStartHook {
        [HarmonyPrefix]
        private static void Prefix(int spawnedRound){
            #if DEBUG
            if (IsMasteryModeEnabled){
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Round {spawnedRound+1} (Mastery Mode) started!");
            }
            #endif
            currentRound = spawnedRound+1;
        }
    }

    [HarmonyPatch(typeof(Simulation), "AddCash")]
    internal static class AddCash_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(ref double c, ref Simulation.CashSource source)
        {
            if ((source == Simulation.CashSource.Normal) && IsMasteryModeEnabled)
            {
                c *= getMasteryMultipliers((int)(long) RoundsEarly.GetValue()).First(range => (currentRound >= range.start) && (currentRound <= range.end)).multiplier /
                     multipliers.First(range => (currentRound >= range.start) && (currentRound <= range.end)).multiplier;

                totalRoundCash += c;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Simulation), "RoundEnd")]
    internal static class RoundEndHook {
        [HarmonyPrefix]
        private static void Prefix(int round, int highestCompletedRound){
            #if DEBUG
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Round {round+1} = ${totalRoundCash}");
            #endif
            totalRoundCash = 0.0;
        }
    }
}