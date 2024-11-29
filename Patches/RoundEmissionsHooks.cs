using System.Linq;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppAssets.Scripts.Simulation.Track.RoundManagers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace BloonariusMasteryMode.Patches;
using static BloonariusMasteryModeMod;

internal static class RoundEmissionsHooks {
    private static Il2CppReferenceArray<BloonEmissionModel> PromoteEmissions(Il2CppReferenceArray<BloonEmissionModel> emissions){
        
        if (IsMasteryModeEnabled){
            return emissions.Select(
                emissionModel => {
                    BloonEmissionModel promotedEmissionModel = emissionModel.Duplicate();
                    promotedEmissionModel.bloon = PromoteBloon(emissionModel.bloon);
                    return promotedEmissionModel;
                }
            ).ToIl2CppReferenceArray();
        } 
        else if (IsChaosModeEnabled){
            return emissions.Select(
                emissionModel => {
                    BloonEmissionModel promotedEmissionModel = emissionModel.Duplicate();
                    promotedEmissionModel.bloon = ChaosPromoteBloon(emissionModel.bloon);
                    return promotedEmissionModel;
                }
            ).ToIl2CppReferenceArray();
        }
        else {
            return emissions;
        }
    }

    [HarmonyPatch(typeof(FreeplayRoundManager), nameof(FreeplayRoundManager.GetRoundEmissions))]
    internal static class FreeplayRoundManager_GetRoundEmissionsHook {
        [HarmonyPostfix]
        private static void Postfix(FreeplayRoundManager __instance, int roundArrayIndex, ref Il2CppReferenceArray<BloonEmissionModel> __result){
            // Bypass the RBE limit by promoting bloons after selecting the bloon groups.
            __result = PromoteEmissions(__result);
        }
    }

    [HarmonyPatch(typeof(DefaultRoundManager), nameof(DefaultRoundManager.GetRoundEmissions))]
    internal static class DefaultRoundManager_GetRoundEmissionsHook {
        [HarmonyPostfix]
        private static void Postfix(DefaultRoundManager __instance, int roundArrayIndex, ref Il2CppReferenceArray<BloonEmissionModel> __result){
            // Bypass the RBE limit by promoting bloons after selecting the bloon groups.
            __result = PromoteEmissions(__result);
        }
    }
}