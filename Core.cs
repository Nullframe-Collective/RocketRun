using CustomGameModesFramework;
using CustomGameModesFramework.Patches;
using HarmonyLib;
using Il2Cpp;
using Il2CppCodeStage.AntiCheat.ObscuredTypes;
using MelonLoader;
using System.Collections;
using UnityEngine;

[assembly: MelonInfo(typeof(RocketRun.Core), "RocketRun!", "1.0.0", "Nullframe Collective")]
[assembly: MelonGame("ZeoWorks", "Slendytubbies 3")]
[assembly: MelonColor(1, 255, 0, 0)]

namespace RocketRun
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }

    [GameModeInfo("rocketrun", "ROCKET RUN!", "12", "1-2", "1-6", "1-12", BaseMode = "DM")]
    public class RocketRun : CustomGameMode
    {
        private RoomMultiplayerMenu _roomMultiplayerMenu;

        public override void OnSetupRoom(RoomMultiplayerMenu instance)
        {
            _roomMultiplayerMenu = instance;

            RespawnPlayerGUIPatch.ShowRespawnText = false;
            OnGUIPatch.ShowLeaderText = true;
            OnGUIPatch.IsWaitingForPlayers = true;

            OnGUIPatch.RoundEndedText =
                string.IsNullOrEmpty(instance?.HGKEEDKPGOD) ?
                "Masuka Wins :)" :
                $"{instance?.HGKEEDKPGOD} Wins";
        }

        public override void OnStartGame(RoomMultiplayerMenu instance)
        {
            instance.DKOBDBGFJMO = true;
            instance.INCDKGADOBA = new ObscuredString[] { "RPG" };
            instance.SpawnPlayer(instance.OJPBAOICLJK.teamName);
            instance.IOMIAHNBDOG = false;
        }

        public override void OnSpawnPlayer(RoomMultiplayerMenu instance)
        {
            MelonCoroutines.Start(InitializePlayerSettings(instance));
            MelonCoroutines.Start(InitializeWeaponSettingsAsync(instance));
        }

        public override void OnRespawnPlayer(RagdollController instance)
        {
            _roomMultiplayerMenu.SpawnPlayer(_roomMultiplayerMenu.OJPBAOICLJK.teamName);
        }

        public override void OnQuitRoom(RoomMultiplayerMenu instance)
        {
            OnGUIPatch.ShowLeaderText = false;
            OnGUIPatch.IsWaitingForPlayers = false;
        }

        private IEnumerator InitializePlayerSettings(RoomMultiplayerMenu instance)
        {
            yield return new WaitForSeconds(1f);

            if (instance == null) yield break;

            FPScontroller fpsController = instance.CNLHJAICIBH.GetComponent<FPScontroller>();
            if (fpsController == null) yield break;

            fpsController.HKCDMBALAAK.WalkSpeed = 8f;
            fpsController.HKCDMBALAAK.RunSpeed = 15f;
            fpsController.AALHECCKHFD.baseHeight = 3f;
        }

        private IEnumerator InitializeWeaponSettingsAsync(RoomMultiplayerMenu instance)
        {
            yield return new WaitForSeconds(1f);

            if (instance == null) yield break;

            FPScontroller fpsController = instance?.CNLHJAICIBH.GetComponent<FPScontroller>();
            WeaponManager weaponManager = fpsController?.GetComponent<WeaponPickUp>()?.AKBOKOLNGGL;
            WeaponScript weaponScript = weaponManager?.FGGENNCGLJI;

            if (weaponScript == null) yield break;

            weaponScript.MJIKHNADGAG.reloadTime = 0.5f;
            weaponScript.MJIKHNADGAG.ammoCount = int.MaxValue;

            yield break;
        }
    }

    [HarmonyPatch(typeof(ExplosionDamage), "AGIIFJLGHJB")]
    public class RPGDamageFix
    {
        [HarmonyPrefix]
        public static bool Prefix(ExplosionDamage __instance)
        {
            foreach (Collider collider in Physics.OverlapSphere(__instance.GFLEMICGHHP, __instance.OPGAMEEGIPH))
            {
                if (collider == null) continue;

                float distanceToCollider = Vector3.Distance(collider.ClosestPointOnBounds(__instance.GFLEMICGHHP), __instance.GFLEMICGHHP);
                float damageMultiplier = 1f - Mathf.Clamp01(distanceToCollider / __instance.OPGAMEEGIPH);
                float scaledDamage = damageMultiplier * __instance.FABMEIDIGFC;

                if (scaledDamage < 10f)
                {
                    scaledDamage += 3f;
                }

                if (collider.gameObject.CompareTag("Remote"))
                {
                    ApplyDamage(collider, scaledDamage);
                }
            }

            return true;
        }

        private static void ApplyDamage(Collider collider, float damage)
        {
            PlayerDamage playerDamage = collider?.GetComponent<PlayerDamage>();
            playerDamage?.E100050(damage, string.Empty);
        }
    }
}