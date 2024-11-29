using System.Collections.Generic;
using System.Linq;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppAssets.Scripts.Unity;
using MelonLoader;

namespace BloonariusMasteryMode;
using static BloonariusMasteryModeMod;
using static BloonariusMasteryMode.Patches.SimulationPatches;

internal static class CashCalculator {
    private static readonly Dictionary<string, (int normal, int round80)> cashMap = new()
    {
        {"Red", (1, 1)},
        {"RedCamo", (1, 1)},
        {"RedRegrow", (1, 1)},
        {"RedRegrowCamo", (1, 1)},

        {"Blue", (2, 2)},
        {"BlueCamo", (2, 2)},
        {"BlueRegrow", (2, 2)},
        {"BlueRegrowCamo", (2, 2)},

        {"Green", (3, 3)},
        {"GreenCamo", (3, 3)},
        {"GreenRegrow", (3, 3)},
        {"GreenRegrowCamo", (3, 3)},

        {"Yellow", (4, 4)},
        {"YellowCamo", (4, 4)},
        {"YellowRegrow", (4, 4)},
        {"YellowRegrowCamo", (4, 4)},

        {"Pink", (5, 5)},
        {"PinkCamo", (5, 5)},
        {"PinkRegrow", (5, 5)},
        {"PinkRegrowCamo", (5, 5)},

        {"Black", (11, 6)},
        {"BlackCamo", (11, 6)},
        {"BlackRegrow", (11, 6)},
        {"BlackRegrowCamo", (11, 6)},

        {"White", (11, 6)},
        {"WhiteCamo", (11, 6)},
        {"WhiteRegrow", (11, 6)},
        {"WhiteRegrowCamo", (11, 6)},

        {"Purple", (11, 6)},
        {"PurpleCamo", (11, 6)},
        {"PurpleRegrow", (11, 6)},
        {"PurpleRegrowCamo", (11, 6)},

        {"Lead", (23, 7)},
        {"LeadCamo", (23, 7)},
        {"LeadRegrow", (23, 7)},
        {"LeadRegrowCamo", (23, 7)},
        {"LeadFortified", (23, 7)},
        {"LeadRegrowFortified", (23, 7)},
        {"LeadFortifiedCamo", (23, 7)},
        {"LeadRegrowFortifiedCamo", (23, 7)},

        {"Zebra", (23, 7)},
        {"ZebraCamo", (23, 7)},
        {"ZebraRegrow", (23, 7)},
        {"ZebraRegrowCamo", (23, 7)},

        {"Rainbow", (47, 8)},
        {"RainbowCamo", (47, 8)},
        {"RainbowRegrow", (47, 8)},
        {"RainbowRegrowCamo", (47, 8)},

        {"Ceramic", (95, 95)},
        {"CeramicCamo", (95, 95)},
        {"CeramicRegrow", (95, 95)},
        {"CeramicRegrowCamo", (95, 95)},
        {"CeramicFortified", (95, 95)},
        {"CeramicFortifiedCamo", (95, 95)},
        {"CeramicRegrowFortified", (95, 95)},
        {"CeramicRegrowFortifiedCamo", (95, 95)},

        {"Moab", (381, 381)},
        {"MoabFortified", (381, 381)},

        {"Bfb", (1525, 1525)},
        {"BfbFortified", (1525, 1525)},

        {"DdtCamo", (381, 381)},
        {"DdtFortifiedCamo", (381, 381)},

        {"Zomg", (6101, 6101)},
        {"ZomgFortified", (6101, 6101)},

        {"Bad", (13346, 13346)},
        {"BadFortified", (13346, 13346)},

        {"Bloonarius3", (42000, 42000)},
        {"BloonariusElite3", (69000, 69000)},

        {"Lych3", (42000, 42000)},
        {"LychElite3", (69000, 69000)}
    };


    private static double IncomeMultiplier(int round)
    {
        return getMasteryMultipliers((int)(long) RoundsEarly.GetValue()).First(range => (round >= range.start) && (round <= range.end)).multiplier;
    }

    private static bool outputtedRoundSet = false;
    public static void OnUpdate(){
        GameModel? gameModel = Game.instance?.model;
        if ((gameModel != null) && (!outputtedRoundSet)){
            RoundSetModel? roundSet =  GameData.Instance?.RoundSetByName("DefaultRoundSet");
            if (roundSet != null){
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Roundset: {roundSet.name}");
                int cumulativeCash = 650;
                for (int index = 0; index < roundSet.rounds.Length; index++){
                    RoundModel round = roundSet.rounds.ElementAt(index);
                    int poppingCash = round.groups.Select(group => CashFromBloon(PromoteBloon(group.bloon), index+1) * group.count)
                                                  .Sum();
                    int cashReward = 100 + index+1;

                    int roundCash = (int) ((poppingCash + cashReward) * IncomeMultiplier(index+1));
                    cumulativeCash += roundCash;

                    Melon<BloonariusMasteryModeMod>.Logger.Msg($"{index+1}: ${roundCash} ${cumulativeCash}");
                }
                outputtedRoundSet = true;
            }
        }
    }

    private static int CashFromBloon(string bloon, int round){
        if (bloon == ModContent.BloonID<Bloonarius>())
        {
            bloon = "Bloonarius3";
        } 
        else if (bloon == ModContent.BloonID<BloonariusFortified>())
        {
            bloon = "BloonariusElite3";
        }
        else if (bloon == ModContent.BloonID<Lych>())
        {
            bloon = "Lych3";
        }
        else if (bloon == ModContent.BloonID<LychFortified>())
        {
            bloon = "LychElite3";
        }
        else if (bloon == ModContent.BloonID<Dreadbloon>())
        {
            bloon = "Dreadbloon3";
        }
        else if (bloon == ModContent.BloonID<DreadbloonFortified>())
        {
            bloon = "DreadbloonElite3";
        }
        else if (bloon == ModContent.BloonID<Vortex>())
        {
            bloon = "Vortex3";
        }
        else if (bloon == ModContent.BloonID<VortexFortified>())
        {
            bloon = "VortexElite3";
        }
        else if (bloon == ModContent.BloonID<Phayze>())
        {
            bloon = "Phayze";
        }
        else if (bloon == ModContent.BloonID<PhayzeFortified>())
        {
            bloon = "PhayzeFortified";
        }

        var (normal, round80) = cashMap.GetValueOrDefault(bloon);
        
        if (round > 80){
            return round80;
        } else {
            return normal;
        }
        
    }
}