using MelonLoader;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using VRC.Core;

[assembly: MelonInfo(typeof(Astrum.AstralRiskAcceptance), "SaferRiskAcceptance", "0.6.0", downloadLink: "github.com/Astrum-Project/AstralRiskAcceptance")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace Astrum
{
    public class AstralRiskAcceptance : MelonMod
    {
        const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

        public override void OnApplicationStart()
        {
            TryHook("WebRequest.CreateHttp",
                typeof(WebRequest).GetMethod(nameof(WebRequest.CreateHttp), new Type[1] { typeof(Uri) }),
                typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_Uri), PrivateStatic).ToNewHarmonyMethod()
            );

            TryHook("WebClient.DownloadString",
                typeof(WebClient).GetMethod(nameof(WebClient.DownloadString), new Type[1] { typeof(string) }),
                typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_string), PrivateStatic).ToNewHarmonyMethod()
            );
            
            TryHook("RoomManager.Method_Public_Static_Boolean_ApiWorld_ApiWorldInstance_String_Int32_0",
                typeof(RoomManager).GetMethod(nameof(RoomManager.Method_Public_Static_Boolean_ApiWorld_ApiWorldInstance_String_Int32_0)), 
                typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_ApiWorld), PrivateStatic).ToNewHarmonyMethod()
            );
        }

        public override void OnSceneWasLoaded(int _, string __)
        {
            if (UnityEngine.GameObject.Find("eVRCRiskFuncEnable") == null)
                UnityEngine.Object.DontDestroyOnLoad(new UnityEngine.GameObject("eVRCRiskFuncEnable"));

            if (UnityEngine.GameObject.Find("UniversalRiskyFuncEnable") == null)
                UnityEngine.Object.DontDestroyOnLoad(new UnityEngine.GameObject("UniversalRiskyFuncEnable"));

            UnityEngine.GameObject disabler;
            if ((disabler = UnityEngine.GameObject.Find("eVRCRiskFuncDisable")) != null)
                UnityEngine.Object.Destroy(disabler);

            if ((disabler = UnityEngine.GameObject.Find("UniversalRiskyFuncDisable")) != null)
                UnityEngine.Object.Destroy(disabler);
        }

        private void TryHook(string name, MethodBase method, HarmonyLib.HarmonyMethod pre, HarmonyLib.HarmonyMethod post = null)
        {
            try
            {
                if (method is null)
                {
                    MelonLogger.Msg("Skipping " + name);
                    return;
                }

                HarmonyInstance.Patch(method, pre, post);
                MelonLogger.Msg("Hooked " + name);
            } 
            catch { MelonLogger.Warning("Failed to hook " + name); }
        }

        private static void Prehook_0_string(ref string __0)
        {
            if (__0.ToLower().Contains("riskyfuncs"))
                __0 = "https://raw.githubusercontent.com/VRChat-is-Awesome/SaferRiskAcceptance/master/allowed.txt";
        }

        private static void Prehook_0_Uri(ref Uri __0)
        {
            if (__0.AbsoluteUri.ToLower().Contains("riskyfuncs"))
                __0 = new Uri("https://raw.githubusercontent.com/VRChat-is-Awesome/SaferRiskAcceptance/master/allowed.txt");
        }

        private static void Prehook_0_ApiWorld(ref ApiWorld __0)
        {
            __0.tags = new();
        }
    }
}
