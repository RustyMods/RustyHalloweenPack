// using System;
// using System.Collections.Generic;
// using System.Linq;
// using HarmonyLib;
// using UnityEngine;
//
// namespace RustyHalloweenPack.Solution;
//
// public class PumpkinHead : MonoBehaviour
// {
//     private static readonly GameObject pumpkinHead = RustyHalloweenPackPlugin.m_assets.LoadAsset<GameObject>("PumpkinHead");
//
//     private static readonly Dictionary<string, HeadData> m_data = new()
//     {
//         ["Troll"] = new(Vector3.zero, new(0f, 180f, 0f), 0.025f),
//         // ["Troll_ragdoll"] = new(Vector3.zero, new(0f, 180f, 0f, 0f), new(0.025f, 0.025f, 0.025f)),
//         ["Neck"] = new(new(0f, 0f, -0.003f), new(-45f, 180f, 0f), 0.025f),
//         ["Greydwarf"] = new(new(0f, 0.0048f, -0.0029f), new(-90f, 180f, 0f), 0.015f),
//         ["Greyling"] = new(new(0f, 0.00527f, -0.00243f), new(-90f, 180f, 0f), 0.015f),
//         ["Skeleton"] = new(new(0.00048f, 0.00058f, 0f), new(0f, -90f, 0f), 0.005f),
//         // ["Goblin"] = new(new(0f, -0.0006f, -0.00101f), new(0f, -180f, 0f), 0.005f),
//         ["Draugr"] = new(new(0f, 0.001f, 0f), new(0f, 180f, 0f), 0.005f),
//         // ["GolbinShaman"] = new(new(0f, 0.00061f, -0.00103f), new(-90f, 180f, 0f), 0.005f),
//         ["Dverger"] = new(new(0f, -0.00037f, 0.00072f), Vector3.zero, 0.0065f)
//     };
//     
//
//     private ZNetView m_nview = null!;
//     private Character m_character = null!;
//     private GameObject m_pumpkinHead = null!;
//     
//     public static readonly List<PumpkinHead> m_instances = new();
//     public void Awake()
//     {
//         m_nview = GetComponent<ZNetView>();
//         m_character = GetComponent<Character>();
//         var head = Utils.GetBoneTransform(m_character.m_animator, HumanBodyBones.Head);
//         m_pumpkinHead = Instantiate(pumpkinHead, head, false);
//         SetScale();
//         m_pumpkinHead.SetActive(RustyHalloweenPackPlugin._enablePumpkinHeads.Value is RustyHalloweenPackPlugin.Toggle.On);
//         if (!m_nview.IsValid()) return;
//         m_instances.Add(this);
//         if (m_nview.m_functions.ContainsKey(nameof(RPC_SetPumpkin).GetStableHashCode())) return;
//         m_nview.Register<bool>(nameof(RPC_SetPumpkin), RPC_SetPumpkin);
//         
//     }
//
//     public void OnDestroy()
//     {
//         m_instances.Remove(this);
//     }
//
//     private void SetScale()
//     {
//         if (!m_data.TryGetValue(name.Replace("(Clone)", string.Empty), out HeadData data)) return;
//         m_pumpkinHead.transform.localPosition = data.m_position;
//         m_pumpkinHead.transform.localRotation = data.m_rotation;
//         m_pumpkinHead.transform.localScale = data.m_scale;
//     }
//
//     public void SetPumpkin(bool enable) => m_nview.InvokeRPC(nameof(RPC_SetPumpkin), enable);
//
//     public void RPC_SetPumpkin(long sender, bool enable) => m_pumpkinHead.SetActive(enable);
//
//     private class HeadData
//     {
//         public readonly Vector3 m_position;
//         public readonly Quaternion m_rotation;
//         public readonly Vector3 m_scale;
//
//         public HeadData(Vector3 pos, Vector3 rot, float scale)
//         {
//             m_position = pos;
//             m_rotation = new Quaternion(rot.x, rot.y, rot.z, 0f);
//             m_scale = new Vector3(scale, scale, scale);
//         }
//     }
//
//     [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
//     private static class ZNetScene_Awake_Patch
//     {
//         private static void Postfix(ZNetScene __instance)
//         {
//             if (!__instance) return;
//             foreach (var prefab in __instance.m_prefabs)
//             {
//                 if (!m_data.ContainsKey(prefab.name)) continue;
//                 prefab.AddComponent<PumpkinHead>();
//             }
//         }
//     }
// }