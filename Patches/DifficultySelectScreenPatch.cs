using HarmonyLib;
using UnityEngine;
using Il2CppAssets.Scripts.Unity.UI_New.Main.DifficultySelect;
using UnityEngine.UI;
using Il2CppInterop.Runtime;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api;
using System.Linq;
using MelonLoader.ICSharpCode.SharpZipLib.Core;
using Il2CppAssets.Scripts.Models.Bloons;
using MelonLoader;

namespace ImprovedMasteryMode.Patches;
using static ImprovedMasteryModeMod;

internal static class DifficultySelectScreenPatch {
    private static readonly string[] ShowOnMenus =
    {
        "DifficultySelectUI", "ModeSelectUI",
        "DifficultySelectScreen", "ModeSelectScreen"
    };

    private static Color defaultBackgroundColor = Color.white;
    private static Color masteryModeBackgroundColor = new Color(1.0f, 0.4f, 0.4f, 1.0f);

    [HarmonyPatch(typeof(DifficultySelectScreen), nameof(DifficultySelectScreen.Open))]
    public class DifficultySelectScreen_Open
    {
        [HarmonyPrefix]
        private static void Prefix(DifficultySelectScreen __instance){
            Info info = new Info("Mastery Mode", 800, 2350, 350, 350, new Vector2(0, 0), new Vector2(0, 1));
            GameObject gameObject = new GameObject(info.Name, Il2CppType.Of<RectTransform>());
            ModHelperButton modHelperButton = gameObject.AddComponent<ModHelperButton>();
            modHelperButton.initialInfo = info;
            info.Apply((ModHelperComponent) modHelperButton);

            modHelperButton.AddText(new Info("Mode", 0, -175, 500, 100), "Mastery Mode", 60f);
            
            Image image = modHelperButton.AddComponent<Image>();
           
            
            image.type = Image.Type.Sliced;
            if (IsMasteryModeEnabled){
                image.SetSprite(ModContent.GetSprite<ImprovedMasteryModeMod>("MasteryModeButton"));
                
                //CommonBackgroundScreen.instance.mainMenuWorldBlurredImg.CrossFadeColor(masteryModeBackgroundColor, 2, false, true, true);
            } else if (IsChaosModeEnabled){
                image.SetSprite(ModContent.GetSprite<ImprovedMasteryModeMod>("ChaosModeButton"));

                //CommonBackgroundScreen.instance.mainMenuWorldBlurredImg.CrossFadeColor(defaultBackgroundColor, 2, false, true, true);
            }
            else {
                image.SetSprite(VanillaSprites.WoodenRoundButton);
                
                //CommonBackgroundScreen.instance.mainMenuWorldBlurredImg.CrossFadeColor(defaultBackgroundColor, 2, false, true, true);
            }

            Button button = modHelperButton.AddComponent<Button>();
            button.onClick.AddListener(() => {
                MenuManager.instance.buttonClick3Sound.Play("ClickSounds");

                
                if (IsMasteryModeEnabled){
                    button.image.SetSprite(ModContent.GetSprite<ImprovedMasteryModeMod>("ChaosModeButton"));
                    IsMasteryModeEnabled = !IsMasteryModeEnabled;
                    IsChaosModeEnabled = !IsChaosModeEnabled;

                    //CommonBackgroundScreen.instance.mainMenuWorldBlurredImg.CrossFadeColor(masteryModeBackgroundColor, 2, false, true, true);
                }
                else if (IsChaosModeEnabled){
                    button.image.SetSprite(VanillaSprites.WoodenRoundButton);
                    IsChaosModeEnabled = !IsChaosModeEnabled;

                    //CommonBackgroundScreen.instance.mainMenuWorldBlurredImg.CrossFadeColor(defaultBackgroundColor, 2, false, true, true);
                }
                else {
                    button.image.SetSprite(ModContent.GetSprite<ImprovedMasteryModeMod>("MasteryModeButton"));
                    IsMasteryModeEnabled = !IsMasteryModeEnabled;

                    //CommonBackgroundScreen.instance.mainMenuWorldBlurredImg.CrossFadeColor(defaultBackgroundColor, 2, false, true, true);
                }


            });

            button.transition = Selectable.Transition.Animation;
            Animator animator = modHelperButton.AddComponent<Animator>();
            animator.runtimeAnimatorController = Animations.GlobalButtonAnimation;
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;

            

            // button.AddImage(
            //      new Info("Tick", -75, -75, 100, 100, Vector2.one), VanillaSprites.SelectedTick
            // );

            __instance.gameObject.AddModHelperComponent(modHelperButton);
        }
    }

    internal static void OnUpdate(){
        var shouldShow = IsMasteryModeEnabled && ShowOnMenus.Contains(MenuManager.instance?.GetCurrentMenuName());
        Color blurredImgColor = shouldShow ? masteryModeBackgroundColor: defaultBackgroundColor;
    }
}