using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
namespace RustyHalloweenPack.Solution;

public class BroomFly : MonoBehaviour
{
    private ZNetView m_nview = null!;
    private Player m_player = null!;
    private GameObject m_broom = null!;
    private GameObject m_particles = null!;

    private readonly float m_checkJumpInterval = 1f;
    private float m_checkJumpTimer;
    private bool m_flying;
    private float m_staminaConsumptionTimer;
    private float m_eitrConsumptionTimer;

    private ItemDrop.ItemData? m_currentBroom;
    public static readonly List<BroomFly> m_instances = new();

    public void Awake()
    {
        m_nview = GetComponent<ZNetView>();
        m_player = GetComponent<Player>();

        Transform? hips = Utils.GetBoneTransform(m_player.m_animator, HumanBodyBones.Hips);
        m_broom = Instantiate(RustyHalloweenPackPlugin.m_broomObject, hips, false);
        m_particles = m_broom.GetComponentInChildren<ParticleSystem>().gameObject;
        m_broom.SetActive(false);
        SetParticles(RustyHalloweenPackPlugin._enableParticles.Value is RustyHalloweenPackPlugin.Toggle.On);
        if (!m_nview.IsValid()) return;
        m_instances.Add(this);
        if (m_nview.m_functions.ContainsKey(nameof(RPC_SetBroomVisible).GetStableHashCode())) return;
        m_nview.Register<bool>(nameof(RPC_SetBroomVisible), RPC_SetBroomVisible);
    }

    public void OnDestroy()
    {
        m_instances.Remove(this);
    }

    public void Update()
    {
        if (m_player.IsDebugFlying() || RustyHalloweenPackPlugin._enableBroom.Value is RustyHalloweenPackPlugin.Toggle.Off)
        {
            ToggleFly(false);
            return;
        }
        
        float dt = Time.deltaTime;
        HandleFlying(dt);
        UpdateStamina(dt);
        UpdateEitr(dt);
    }

    public void SetParticles(bool enable)
    {
        if (m_particles == null) return;
        m_particles.SetActive(enable);
    }

    public void HandleFlying(float dt)
    {
        m_checkJumpTimer += dt;
        if (m_checkJumpTimer < m_checkJumpInterval) return;
        m_checkJumpTimer = 0.0f;
        
        if (!ZInput.GetButton("Jump")) return;

        if (IsFlying())
        {
            ToggleFly(false);
            EquipBroom();
        }
        else
        {
            if (!HasBroomInHand() || !HasWizardHatEquipped()) return;
            ToggleFly(true);
        }
    }

    public void UpdateStamina(float dt)
    {
        if (!IsFlying()) return;
        m_staminaConsumptionTimer += dt;
        if (m_staminaConsumptionTimer < 1f) return;
        m_staminaConsumptionTimer = 0.0f;
        m_player.UseStamina(RustyHalloweenPackPlugin._staminaConsumption.Value * GetQualityCostReduction());
    }

    public void UpdateEitr(float dt)
    {
        if (!IsFlying()) return;
        m_eitrConsumptionTimer += dt;
        if (m_eitrConsumptionTimer < 1f) return;
        m_eitrConsumptionTimer = 0.0f;
        m_player.UseEitr(RustyHalloweenPackPlugin._eitrConsumption.Value * GetQualityCostReduction());
    }

    private float GetQualityCostReduction()
    {
        int quality = (m_currentBroom?.m_quality ?? 1) - 1;
        if (quality == 0) return 1;
        return RustyHalloweenPackPlugin._levelConsumptionReduction.Value / quality;
    }
        
    
    private bool HasEnergy()
    {
        if (RustyHalloweenPackPlugin._staminaConsumption.Value > 0f)
        {
            if (!m_player.HaveStamina(RustyHalloweenPackPlugin._staminaConsumption.Value)) return false;
        }

        if (RustyHalloweenPackPlugin._eitrConsumption.Value > 0f)
        {
            if (!m_player.HaveEitr(RustyHalloweenPackPlugin._eitrConsumption.Value)) return false;
        }

        return true;
    }
    
    private bool HasWizardHatEquipped()
    {
        var helmet = m_player.m_helmetItem;
        if (helmet == null) return false;
        return helmet.m_shared.m_name == "$item_rs_helmetwizardhat";
    }
    private void EquipBroom()
    {
        var broom = m_player.GetInventory().GetAllItems().Find(x => x.m_shared.m_name == "$item_rs_wizardbroom");
        if (broom == null) return;
        m_player.EquipItem(broom);
    }

    private bool HasBroomInHand()
    {
        var currentWeapon = m_player.GetCurrentWeapon();
        if (currentWeapon == null) return false;
        if (currentWeapon.m_shared.m_name != "$item_rs_wizardbroom") return false;
        return true;
    }

    private bool IsFlying() => m_flying;

    public void ToggleFly(bool enable)
    {
        if (m_flying == enable) return;
        m_player.m_zanim.SetBool("attach_asksvin", enable);
        if (enable)
        {
            m_currentBroom = m_player.GetCurrentWeapon();
            m_player.UnequipItem(m_currentBroom, false);
        }
        m_nview.InvokeRPC(nameof(RPC_SetBroomVisible), enable);
        m_flying = enable;
    }
    
    private void RPC_SetBroomVisible(long sender, bool enable) => m_broom.SetActive(enable);

    private static void UpdateBroomFly(BroomFly component, float dt)
    {
        Player player = component.m_player;
        Character.takeInputDelay = Mathf.Max(0.0f, Character.takeInputDelay - dt);
        float num = player.m_run ? RustyHalloweenPackPlugin._broomSpeed.Value * 2.5f : RustyHalloweenPackPlugin._broomSpeed.Value;
        if (!component.HasEnergy()) num = 0f;
        Vector3 direction = player.m_moveDir * num;

        if (component.HasEnergy())
        {
            float upwardSpeed = num * Mathf.Sin(-player.m_lookPitch * Mathf.Deg2Rad);
            direction.y = upwardSpeed;
        }
        else
        {
            --direction.y;
        }
        
        Transform playerTransform = player.transform;

        player.m_currentVel = Vector3.Lerp(player.m_currentVel, direction, RustyHalloweenPackPlugin._broomAcceleration.Value);
        player.m_body.velocity = player.m_currentVel;
        player.m_body.useGravity = false;
        player.m_lastGroundTouch = 0.0f;
        player.m_maxAirAltitude = playerTransform.position.y;
        player.m_body.rotation = Quaternion.RotateTowards(playerTransform.rotation, player.m_lookYaw, 100f * dt);
        player.m_body.angularVelocity = Vector3.zero;
        player.UpdateEyeRotation();
    }

    private static void UpdateBroomTilt(Player player, float dt)
    {
        Vector3 lookDirection = player.m_lookDir;
        float tiltAmount = Vector3.Dot(Vector3.Cross(lookDirection, Vector3.up), Vector3.forward);
        Quaternion targetRotation = Quaternion.Euler(0, 0, -tiltAmount * 30f); 
        Quaternion currentRotation = player.m_visual.transform.localRotation;
        Quaternion newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, dt * player.m_groundTiltSpeed);
        player.m_visual.transform.localRotation = newRotation;
        player.m_nview.GetZDO().Set(ZDOVars.s_tiltrot, newRotation);
        player.m_animator.SetFloat(Character.s_tilt, Vector3.Dot(player.m_visual.transform.forward, Vector3.up));
    }

    [HarmonyPatch(typeof(Character), nameof(Character.UpdateMotion))]
    private static class Character_UpdateMotion_Patch
    {
        private static bool Prefix(Character __instance, float dt)
        {
            if (__instance is not Player player) return true;
            if (!player.TryGetComponent(out BroomFly component)) return true;
            if (!component.IsFlying()) return true;
            UpdateBroomFly(component, dt);
            return false;
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.UpdateGroundTilt))]
    private static class Character_UpdateGroundTilt_Patch
    {
        private static bool Prefix(Character __instance, float dt)
        {
            if (__instance is not Player player) return true;
            if (!player.TryGetComponent(out BroomFly component)) return true;
            if (!component.IsFlying()) return true;
            UpdateBroomTilt(player, dt);
            return false;
        }
    }
    

    [HarmonyPatch(typeof(Character), nameof(Character.Jump))]
    private static class Character_Jump_Patch
    {
        private static bool Prefix(Character __instance)
        {
            if (__instance is not Player player) return true;
            if (!player.TryGetComponent(out BroomFly component)) return true;
            return !component.IsFlying();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.UpdateDodge))]
    private static class Player_UpdateDodge_Patch
    {
        private static bool Prefix(Player __instance)
        {
            if (__instance != Player.m_localPlayer) return true;
            if (!__instance.TryGetComponent(out BroomFly component)) return true;
            return !component.IsFlying();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.UpdateCrouch))]
    private static class Player_UpdateCrouch_Patch
    {
        private static bool Prefix(Player __instance)
        {
            if (__instance != Player.m_localPlayer) return true;
            if (!__instance.TryGetComponent(out BroomFly component)) return true;
            if (component.IsFlying())
            {
                __instance.SetCrouch(false);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.PlayerAttackInput))]
    private static class Player_PlayerAttackInput_Patch
    {
        private static bool Prefix(Player __instance)
        {
            if (!__instance != Player.m_localPlayer) return true;
            if (!__instance.TryGetComponent(out BroomFly component)) return true;
            return !component.IsFlying();
        }
    }
    
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    private static class ZNetScene_Awake_Patch
    {
        private static void Postfix(ZNetScene __instance)
        {
            if (!__instance) return;
            var player = __instance.GetPrefab("Player");
            if (!player) return;
            player.AddComponent<BroomFly>();
        }
    }
}