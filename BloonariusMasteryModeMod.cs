using MelonLoader;
using Il2CppAssets.Scripts.Simulation.Bloons;
using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper;
using Il2CppAssets.Scripts.Models.Bloons;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using UnityEngine.InputSystem.Utilities;
using BTD_Mod_Helper.Api.ModOptions;

using System.Collections.Immutable;
using BloonariusMasteryMode.Patches;
using System;
using UnityEngine;
using System.Linq;

[assembly: MelonInfo(typeof(BloonariusMasteryMode.BloonariusMasteryModeMod), BloonariusMasteryMode.ModHelperData.Name, BloonariusMasteryMode.ModHelperData.Version, BloonariusMasteryMode.ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace BloonariusMasteryMode;

public class BloonariusMasteryModeMod : BloonsTD6Mod
{
    private static readonly ModSettingBool LimitMinions = new(false) {
        displayName = "Limit minion spawning"
    };

    public static bool IsMasteryModeEnabled = false;

    public static bool IsChaosModeEnabled = false;

    public static readonly ModSettingInt RoundsEarly = new(10){
        displayName = "Rounds early that cash per pop is reduced.",
        min = 0,
        max = 20
    };

    public static readonly ModSettingInt PromotionTimes = new(1)
    {
        displayName = "Number of Times a Bloon is promoted, also affects demotion in Chaos Mode",
        min = 0,
        max = 10
    };

    public static readonly ModSettingDouble BloonariusSpeed = new(2.0f)
    {
        displayName = "Change the speed of Bloonarius and Elite Bloonarius when upgraded in Mastery Mode",
        min = .01,
        max = 20
    };

    public static readonly ModSettingDouble LychSpeed = new(3.0f)
    {
        displayName = "Change the speed of Lych and Elite Lych when upgraded in Mastery Mode",
        min = .01,
        max = 20
    };

    public static readonly ModSettingDouble DreadbloonSpeed = new(2.0f)
    {
        displayName = "Change the speed of Dreadbloon and Elite Dreadbloon when upgraded in Mastery Mode",
        min = .01,
        max = 20
    };

    public static readonly ModSettingDouble VortexSpeed = new(5.0f)
    {
        displayName = "Change the speed of Vortex and Elite Vortex when upgraded in Mastery Mode",
        min = .01,
        max = 20
    };

    public static readonly ModSettingDouble PhayzeSpeed = new(2.0f)
    {
        displayName = "Change the speed of Phayze and Elite Phayze when upgraded in Mastery Mode",
        min = .01,
        max = 20
    };
    private static readonly Dictionary<string, string> promotionMap = new()
    {
        { "Red", "Blue" },
        { "RedCamo", "BlueCamo" },
        { "RedRegrow", "BlueRegrow" },
        { "RedRegrowCamo", "BlueRegrowCamo" },

        { "Blue", "Green" },
        { "BlueCamo", "GreenCamo" },
        { "BlueRegrow", "GreenRegrow" },
        { "BlueRegrowCamo", "GreenRegrowCamo" },

        { "Green", "Yellow" },
        { "GreenCamo", "YellowCamo" },
        { "GreenRegrow", "YellowRegrow" },
        { "GreenRegrowCamo", "YellowRegrowCamo" },

        { "Yellow", "Pink" },
        { "YellowCamo", "PinkCamo" },
        { "YellowRegrow", "PinkRegrow" },
        { "YellowRegrowCamo", "PinkRegrowCamo" },

        { "Pink", "Black" },
        { "PinkCamo", "BlackCamo" },
        { "PinkRegrow", "BlackRegrow" },
        { "PinkRegrowCamo", "BlackRegrowCamo" },

        { "Black", "Zebra" },
        { "BlackCamo", "ZebraCamo" },
        { "BlackRegrow", "ZebraRegrow" },
        { "BlackRegrowCamo", "ZebraRegrowCamo" },

        { "White", "Purple" },
        { "WhiteCamo", "PurpleCamo" },
        { "WhiteRegrow", "PurpleRegrow" },
        { "WhiteRegrowCamo", "PurpleRegrowCamo" },

        { "Purple", "LeadFortified" },
        { "PurpleCamo", "LeadFortifiedCamo" },
        { "PurpleRegrow", "LeadRegrowFortified" },
        { "PurpleRegrowCamo", "LeadRegrowFortifiedCamo" },

        { "Lead", "Rainbow" },
        { "LeadCamo", "RainbowCamo" },
        { "LeadRegrow", "RainbowRegrow" },
        { "LeadRegrowCamo", "RainbowRegrowCamo" },
        { "LeadFortified", "RainbowRegrowCamo" },
        { "LeadRegrowFortified", "RainbowRegrowCamo" },
        { "LeadFortifiedCamo", "RainbowRegrowCamo" },
        { "LeadRegrowFortifiedCamo", "RainbowRegrowCamo" },

        { "Zebra", "Rainbow" },
        { "ZebraCamo", "RainbowCamo" },
        { "ZebraRegrow", "RainbowRegrow" },
        { "ZebraRegrowCamo", "RainbowRegrowCamo" },

        { "Rainbow", "Ceramic" },
        { "RainbowCamo", "CeramicCamo" },
        { "RainbowRegrow", "CeramicRegrow" },
        { "RainbowRegrowCamo", "CeramicRegrowCamo" },

        { "Ceramic", "Moab" },
        { "CeramicCamo", "Moab" },
        { "CeramicRegrow", "Moab" },
        { "CeramicRegrowCamo", "Moab" },
        { "CeramicFortified", "MoabFortified" },
        { "CeramicFortifiedCamo", "MoabFortified" },
        { "CeramicRegrowFortified", "MoabFortified" },
        { "CeramicRegrowFortifiedCamo", "MoabFortified" },

        { "Moab", "Bfb" },
        { "MoabFortified", "BfbFortified" },

        { "Bfb", "DdtCamo" },
        { "BfbFortified", "DdtFortifiedCamo" },

        { "DdtCamo", "Zomg" },
        { "DdtFortifiedCamo", "ZomgFortified" },

        { "Zomg", "Bad" },
        { "ZomgFortified", "BadFortified" },
        
        { "Bad", "Boss3" },
        { "BadFortified", "BossElite3" },

    };
    private static readonly Dictionary<string, string> demotionMap = new()
    {
        { "Blue", "Red"  },
        { "BlueCamo", "RedCamo"  },
        { "BlueRegrow", "RedRegrow"  },
        { "BlueRegrowCamo", "RedRegrowCamo"  },
        //
        { "Green", "Blue" },
        { "GreenCamo", "BlueCamo" },
        { "GreenRegrow", "BlueRegrow" },
        { "GreenRegrowCamo", "BlueRegrowCamo" },
        //
        { "Yellow", "Green" },
        { "YellowCamo", "GreenCamo" },
        { "YellowRegrow", "GreenRegrow" },
        { "YellowRegrowCamo", "GreenRegrowCamo" },
        //
        { "Pink", "Yellow" },
        { "PinkCamo", "YellowCamo" },
        { "PinkRegrow", "YellowRegrow" },
        { "PinkRegrowCamo", "YellowRegrowCamo" },
        //
        { "Black", "Pink" }, 
        { "BlackCamo", "PinkCamo" },
        { "BlackRegrow", "PinkRegrow" },
        { "BlackRegrowCamo", "PinkRegrowCamo" },
        //
        { "White", "Pink" },
        { "WhiteCamo", "PinkCamo" },
        { "WhiteRegrow", "PinkRegrow" },
        { "WhiteRegrowCamo", "PinkRegrowCamo" },
        //
        { "Purple", "White" },
        { "PurpleCamo", "WhiteCamo" },
        { "PurpleRegrow", "WhiteRegrow" },
        { "PurpleRegrowCamo", "WhiteRegrowCamo" },
        //
        { "Lead", "Black" },
        { "LeadCamo", "BlackCamo" },
        { "LeadRegrow", "BlackRegrow" },
        { "LeadRegrowCamo", "BlackRegrowCamo" },
        { "LeadFortified", "Purple" },
        { "LeadFortifiedCamo", "PurpleCamo" },
        { "LeadRegrowFortified", "PurpleRegrow" },
        { "LeadRegrowFortifiedCamo", "PurpleRegrowCamo" },
        //
        { "Zebra", "Black" },
        { "ZebraCamo", "BlackCamo" },
        { "ZebraRegrow", "BlackRegrow" },
        { "ZebraRegrowCamo", "BlackRegrowCamo" },
        //
        { "Rainbow", "Zebra" },
        { "RainbowCamo", "ZebraCamo" },
        { "RainbowRegrow", "ZebraRegrow" },
        { "RainbowRegrowCamo", "ZebraRegrowCamo" },
        //
        { "Ceramic", "Rainbow" },
        { "CeramicCamo", "RainbowCamo" },
        { "CeramicRegrow", "RainbowRegrow" },
        { "CeramicRegrowCamo", "RainbowRegrowCamo" },
        { "CeramicFortified", "Ceramic" },
        { "CeramicFortifiedCamo", "CeramidCamo" },
        { "CeramicRegrowFortified", "CeramicRegrow" },
        { "CeramicRegrowFortifiedCamo", "CeramicRegrowCamo" },
        //
        { "Moab", "Ceramic" },
        { "MoabFortified", "CeramicFortified" },
        //
        { "Bfb", "Moab" },
        { "BfbFortified", "MoabFortified" },
        //
        { "DdtCamo", "Bfb" },
        { "DdtFortifiedCamo", "BfbFortified" },
        //
        { "Zomg", "Ddt" },
        { "ZomgFortified", "DdtFortified" },
        //
        { "Bad", "Zomg" },
        { "BadFortified", "ZomgFortified" },
    };

    public override void OnUpdate(){
        #if DEBUG
        CashCalculator.OnUpdate();
        #endif
        //DifficultySelectScreenPatch.OnUpdate();
    }

    public static int currentRound = 1;
    public override void OnBloonCreated(Bloon bloon)
    {
        if ((bloon.bloonModel.id == ModContent.BloonID<Bloonarius>()) || 
            (bloon.bloonModel.id == ModContent.BloonID<BloonariusFortified>()) ||
            (bloon.bloonModel.id == ModContent.BloonID<Lych>()) ||
            (bloon.bloonModel.id == ModContent.BloonID<LychFortified>())||
            (bloon.bloonModel.id == ModContent.BloonID<Dreadbloon>()) ||
            (bloon.bloonModel.id == ModContent.BloonID<DreadbloonFortified>()) ||
            (bloon.bloonModel.id == ModContent.BloonID<Vortex>()) ||
            (bloon.bloonModel.id == ModContent.BloonID<VortexFortified>()) ||
            (bloon.bloonModel.id == ModContent.BloonID<Phayze>()) ||
            (bloon.bloonModel.id == ModContent.BloonID<PhayzeFortified>()))
        {
            float speedMultiplier;
            if (currentRound <= 100){
                speedMultiplier = 1.0f;
            } else if (currentRound <= 150){
                speedMultiplier = 1.0f + (0.02f * (float)(currentRound - 101));
            } else if (currentRound <= 200){
                speedMultiplier = 2.4f + (0.02f * (float)(currentRound - 151));
            } else if (currentRound <= 250){
                speedMultiplier = 3.9f + (0.02f * (float)(currentRound - 201));
            } else {
                speedMultiplier = 5.4f + (0.02f * (float)(currentRound - 252));
            }

            float healthMultiplier;
            if (currentRound <= 100){
                healthMultiplier = 1.0f;
            } else if (currentRound <= 120){
                healthMultiplier = 1.0f + (0.02f * (float)(currentRound - 100));
            } else if (currentRound <= 144){
                healthMultiplier = 1.4f + (0.05f * (float)(currentRound - 120));
            } else if (currentRound <= 170){
                healthMultiplier = 2.6f + (0.15f * (float)(currentRound - 144));
            } else if (currentRound <= 270){
                healthMultiplier = 6.5f + (0.35f * (float)(currentRound - 170));
            } else if (currentRound <= 320){
                healthMultiplier = 41.5f + (1.0f * (float)(currentRound - 270));
            } else if (currentRound <= 420){
                healthMultiplier = 91.5f + (1.5f * (float)(currentRound - 320));
            } else if (currentRound <= 520){
                healthMultiplier = 241.5f + (2.5f * (float)(currentRound - 420));
            } else {
                healthMultiplier = 491.5f + (5.0f * (float)(currentRound - 520));
            }

            #if DEBUG
            LoggerInstance.Msg($"Bloonarius Freeplay Rules: {speedMultiplier}x speed, {healthMultiplier}x health!");
            #endif

            float defaultSpeed;
            int defaultHealth;

            if (bloon.bloonModel.id == ModContent.BloonID<Bloonarius>()){
                defaultSpeed = (float) BloonariusSpeed.GetValue();
                defaultHealth = Bloonarius.HEALTH;
            } 
            else if (bloon.bloonModel.id == ModContent.BloonID<BloonariusFortified>()){
                defaultSpeed = (float) BloonariusSpeed.GetValue();
                defaultHealth = BloonariusFortified.HEALTH;
            }
            else if (bloon.bloonModel.id == ModContent.BloonID<Lych>())
            {
                defaultSpeed = (float) LychSpeed.GetValue();
                defaultHealth = Lych.HEALTH;
            }
            else if (bloon.bloonModel.id == ModContent.BloonID<LychFortified>())
            {
                defaultSpeed = (float) LychSpeed.GetValue();
                defaultHealth = LychFortified.HEALTH;
            }
            else if (bloon.bloonModel.id == ModContent.BloonID<Dreadbloon>())
            {
                defaultSpeed = (float) DreadbloonSpeed.GetValue();
                defaultHealth = Dreadbloon.HEALTH;
            }
            else if (bloon.bloonModel.id == ModContent.BloonID<DreadbloonFortified>())
            {
                defaultSpeed = (float) DreadbloonSpeed.GetValue();
                defaultHealth = DreadbloonFortified.HEALTH;
            }
            else if (bloon.bloonModel.id == ModContent.BloonID<Vortex>())
            {
                defaultSpeed = (float) VortexSpeed.GetValue();
                defaultHealth = Vortex.HEALTH;
            }
            else if (bloon.bloonModel.id == ModContent.BloonID<VortexFortified>())
            {
                defaultSpeed = (float) VortexSpeed.GetValue();
                defaultHealth = VortexFortified.HEALTH;
            }
            else if (bloon.bloonModel.id == ModContent.BloonID<Phayze>())
            {
                defaultSpeed = (float) PhayzeSpeed.GetValue();
                defaultHealth = Phayze.HEALTH;
            }
            else if (bloon.bloonModel.id == ModContent.BloonID<PhayzeFortified>())
            {
                defaultSpeed = (float) PhayzeSpeed.GetValue();
                defaultHealth = PhayzeFortified.HEALTH;
            }
            else {
                LoggerInstance.Error($"Boss has invalid id - {bloon.bloonModel.id}");
                return;
            }

            bloon.bloonModel.Speed = defaultSpeed * speedMultiplier;
            bloon.bloonModel.maxHealth = (int)(defaultHealth * healthMultiplier);
        }
    }

    public class Bloonarius : ModBloon 
    {
        public override string BaseBloon => "Bloonarius3";

        public const int HEALTH = 350000;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            #if DEBUG
            // Default Speed: 1.25x. BAD Speed: 4.5x
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Bloonarius Default Speed: {bloonModel.Speed} -> {(float)(double) BloonariusSpeed.GetValue()}");
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Bloonarius Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
            #endif
            bloonModel.leakDamage = (float) HEALTH;
            bloonModel.Speed = (float)(double) BloonariusSpeed.GetValue(); 
            bloonModel.GetBehavior<DistributeCashModel>().cash = 42000f;
            bloonModel.isBoss = false;

        }
    }
    public class Lych : ModBloon
    {
        public override string BaseBloon => "Lych3";


        public const int HEALTH = 220000;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
#if DEBUG
            // Default Speed: 1.25x. BAD Speed: 4.5x
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Lych Default Speed: {bloonModel.Speed} -> {(float)(double) LychSpeed.GetValue()}");
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Lych Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
#endif
            bloonModel.leakDamage = (float)HEALTH;
            bloonModel.Speed = (float)(double) LychSpeed.GetValue(); ; 
            bloonModel.GetBehavior<DistributeCashModel>().cash = 42000f;
            bloonModel.isBoss = false;



        }
    }
    public class Dreadbloon : ModBloon
    {
        public override string BaseBloon => "Dreadbloon3";

        public const int HEALTH = 120000;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
#if DEBUG
            // Default Speed: 1.25x. BAD Speed: 4.5x
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Dreadbloon Default Speed: {bloonModel.Speed} -> {(float)(double) DreadbloonSpeed.GetValue()}");
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Dreadbloon Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
#endif
            bloonModel.leakDamage = (float)HEALTH;
            bloonModel.Speed = (float)(double) DreadbloonSpeed.GetValue(); 
            bloonModel.GetBehavior<DistributeCashModel>().cash = 42000f;
            bloonModel.isBoss = false;



        }
    }
    public class Vortex : ModBloon
    {
        public override string BaseBloon => "Vortex3";

        public const int HEALTH = 294000;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
#if DEBUG
            // Default Speed: 1.25x. BAD Speed: 4.5x
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Vortex Default Speed: {bloonModel.Speed} -> {(float)(double) VortexSpeed.GetValue()}");
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Votex Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
#endif
            bloonModel.leakDamage = (float)HEALTH;
            bloonModel.Speed = (float)(double) VortexSpeed.GetValue(); 
            bloonModel.GetBehavior<DistributeCashModel>().cash = 42000f;
            bloonModel.isBoss = false;



        }
    }
    public class Phayze : ModBloon
    {
        public override string BaseBloon => "Phayze3";

        public const int HEALTH = 175000;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
#if DEBUG
            // Default Speed: 1.25x. BAD Speed: 4.5x
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Phayze Default Speed: {bloonModel.Speed} -> {(float)(double) PhayzeSpeed.GetValue()}");
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Phayze Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
#endif
            bloonModel.leakDamage = (float)HEALTH;
            bloonModel.Speed = (float)(double) PhayzeSpeed.GetValue();
            bloonModel.GetBehavior<DistributeCashModel>().cash = 42000f;
            bloonModel.isBoss = false;



        }
    }
    public class BloonariusFortified : ModBloon
        {
            public override string BaseBloon => "BloonariusElite3";

            public const int HEALTH = 2000000;

            public override void ModifyBaseBloonModel(BloonModel bloonModel)
            {
#if DEBUG
                // Default Speed: 1.25x. BAD Speed: 4.5x
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Bloonarius Default Speed: {bloonModel.Speed} -> {(float)(double) BloonariusSpeed.GetValue()}");
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Bloonarius Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
#endif
                bloonModel.leakDamage = (float)HEALTH;
                bloonModel.Speed = (float)(double) BloonariusSpeed.GetValue(); 
                bloonModel.GetBehavior<DistributeCashModel>().cash = 69000f;
                bloonModel.isBoss = false;
                bloonModel.dontShowInSandbox = true;
                bloonModel.dontShowInSandboxOnRelease = true;

                if ((bool)LimitMinions.GetValue())
                {
                    // Don't Spawn Ceramic Bloons
                    foreach (SpawnBloonsActionModel behaviour in bloonModel.GetBehaviors<SpawnBloonsActionModel>())
                    {
                        if (behaviour.bloonType == "Ceramic")
                        {
                            behaviour.spawnCount = 0;
                        }
                    }
                }
            }
        }
    public class LychFortified : ModBloon
        {
            public override string BaseBloon => "LychElite3";


            public const int HEALTH = 1100000;

            public override void ModifyBaseBloonModel(BloonModel bloonModel)
            {
#if DEBUG
                // Default Speed: 1.25x. BAD Speed: 4.5x
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Lych Default Speed: {bloonModel.Speed} -> {(float)(double) LychSpeed.GetValue()}");
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Lych Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
#endif
                bloonModel.leakDamage = (float)HEALTH;
                bloonModel.Speed = (float)(double) LychSpeed.GetValue(); 
                bloonModel.GetBehavior<DistributeCashModel>().cash = 69000f;
                bloonModel.isBoss = false;
                bloonModel.dontShowInSandbox = true;
                bloonModel.dontShowInSandboxOnRelease = true;

                
            }
        }
    public class DreadbloonFortified : ModBloon
    {
        public override string BaseBloon => "DreadbloonElite3";


        public const int HEALTH = 650000;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
#if DEBUG
            // Default Speed: 1.25x. BAD Speed: 4.5x
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Dreadbloon Default Speed: {bloonModel.Speed} -> {(float)(double) DreadbloonSpeed.GetValue()}");
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Dreadbloon Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
#endif
            bloonModel.leakDamage = (float)HEALTH;
            bloonModel.Speed = (float)(double) DreadbloonSpeed.GetValue();
            bloonModel.GetBehavior<DistributeCashModel>().cash = 69000f;
            bloonModel.isBoss = false;
            bloonModel.dontShowInSandbox = true;
            bloonModel.dontShowInSandboxOnRelease = true;


        }
    }
    public class VortexFortified : ModBloon
    {
        public override string BaseBloon => "VortexElite3";


        public const int HEALTH = 1675000;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
#if DEBUG
            // Default Speed: 1.25x. BAD Speed: 4.5x
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Vortex Default Speed: {bloonModel.Speed} -> {(float)(double) VortexSpeed.GetValue()}");
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Vortex Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
#endif
            bloonModel.leakDamage = (float)HEALTH;
            bloonModel.Speed = (float)(double) VortexSpeed.GetValue(); 
            bloonModel.GetBehavior<DistributeCashModel>().cash = 69000f;
            bloonModel.isBoss = false;
            bloonModel.dontShowInSandbox = true;
            bloonModel.dontShowInSandboxOnRelease = true;


        }
    }
    public class PhayzeFortified : ModBloon
    {
        public override string BaseBloon => "PhayzeElite3";


        public const int HEALTH = 800000;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
#if DEBUG
            // Default Speed: 1.25x. BAD Speed: 4.5x
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Phayze Default Speed: {bloonModel.Speed} -> {(float)(double)PhayzeSpeed.GetValue()}");
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Phayze Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
#endif
            bloonModel.leakDamage = (float)HEALTH;
            bloonModel.Speed = (float)(double)PhayzeSpeed.GetValue();
            bloonModel.GetBehavior<DistributeCashModel>().cash = 69000f;
            bloonModel.isBoss = false;
            bloonModel.dontShowInSandbox = true;
            bloonModel.dontShowInSandboxOnRelease = true;


        }
    }


    public static string PromoteBloon(string bloon)
    {

        string temp = bloon;
        for (int i = 0; i < (long) PromotionTimes.GetValue(); i++)
        {
            temp = promotionMap.GetValueOrDefault(temp, temp);
        }

        // uses coin to determine whether lych or bloonarius is sent out
        System.Random coin = new System.Random();
        string boss3, bossElite3;

        int coinToss = coin.Next(0, 5);
        

        if (coinToss == 0)
        {
            boss3 = ModContent.BloonID<Bloonarius>();
            bossElite3 = ModContent.BloonID<BloonariusFortified>();
        }
        else if (coinToss == 1)
        {
            boss3 = ModContent.BloonID<Lych>();
            bossElite3 = ModContent.BloonID<LychFortified>();
        }
        else if (coinToss == 2)
        {
            boss3 = ModContent.BloonID<Dreadbloon>();
            bossElite3 = ModContent.BloonID < DreadbloonFortified>();
        }
        else if (coinToss == 3)
        {
            boss3 = ModContent.BloonID<Vortex>();
            bossElite3 = ModContent.BloonID<VortexFortified>();
        }
        else
        {
            boss3 = ModContent.BloonID<Phayze>();
            bossElite3 = ModContent.BloonID<PhayzeFortified>();
        }
        return temp switch
        {
            "Boss3" => boss3,
            "BossElite3" => bossElite3,
            _ => temp,
        };
    }

    //seperate chaose promotion method so bloons are promoted/demoted a random number of times 
    public static string ChaosPromoteBloon(string bloon)
    {

        string temp = bloon;
        System.Random coin = new System.Random();

        // promotionTimes is how many times promotion/demotion happens, promote is whether to promote or demote,
        // 0 and 1 representing respectively
        long promotionTimes = coin.Next(0, (int)(long)PromotionTimes.GetValue() + 1);
        bool promote = coin.Next(0, 2) == 0;
        if (promote)
        {
            for (int i = 0; i < promotionTimes; i++)
            {
                temp = promotionMap.GetValueOrDefault(temp, temp);
            }
        }
        else
        {
            for (int i = 0; i < promotionTimes; i++)
            {
                temp = demotionMap.GetValueOrDefault(temp, temp);
            }
        }
        


        if (temp != "Boss3" && temp != "BossElite3")
        {
            return temp;
        }
        // uses coin to determine whether lych or bloonarius is sent out
        string boss3, bossElite3;

        int coinToss = coin.Next(0, 5);


        if (coinToss == 0)
        {
            boss3 = ModContent.BloonID<Bloonarius>();
            bossElite3 = ModContent.BloonID<BloonariusFortified>();
        }
        else if (coinToss == 1)
        {
            boss3 = ModContent.BloonID<Lych>();
            bossElite3 = ModContent.BloonID<LychFortified>();
        }
        else if (coinToss == 2)
        {
            boss3 = ModContent.BloonID<Dreadbloon>();
            bossElite3 = ModContent.BloonID<DreadbloonFortified>();
        }
        else if (coinToss == 3)
        {
            boss3 = ModContent.BloonID<Vortex>();
            bossElite3 = ModContent.BloonID<VortexFortified>();
        }
        else
        {
            boss3 = ModContent.BloonID<Phayze>();
            bossElite3 = ModContent.BloonID<PhayzeFortified>();
        }
        return temp switch
        {
            "Boss3" => boss3,
            "BossElite3" => bossElite3,
            _ => temp,
        };
    }
}