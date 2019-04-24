using BepInEx;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using MonoMod.RuntimeDetour;
using RoR2;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using AetherLib;
using AetherLib.Util.Reflection;
using AetherLib.Util.Config;
using R2API;

namespace BnellsCharacterReworks
{
    public static class SkillAPI
    {
        /// <summary>
        /// Lookup for jump overrides by body prefab name
        /// </summary>
        public static Dictionary<SurvivorIndex, EntityState> JumpOverrides;

        public static bool IsInit { get; private set; } = false;
        public static void InitHooks()
        {
            if(!IsInit)
            {
                On.EntityStates.GenericCharacterMain.FixedUpdate += GenericCharacterMain_FixedUpdate;
                JumpOverrides = new Dictionary<SurvivorIndex, EntityState>();
                IsInit = true;
            }
        }

        private static void GenericCharacterMain_FixedUpdate(On.EntityStates.GenericCharacterMain.orig_FixedUpdate orig, GenericCharacterMain self)
        {
            // Check for jump input first, since the base function resets input
            CharacterBody characterBody = ReflectionUtil.GetPropertyValue<CharacterBody>(self, "characterBody");
            string baseNameToken = ReflectionUtil.GetFieldValue<string>(characterBody, "baseNameToken");
            SurvivorIndex survivorIndex = SurvivorIndex.None;
            bool doJump = false;
            Debug.Log("Searching for name token == " + baseNameToken);
            
            bool isAuthority = ReflectionUtil.GetPropertyValue<bool>(self, "isAuthority");
            if (isAuthority)
            {
                
                if (characterBody && JumpOverrides.ContainsKey(survivorIndex))
                {
                    Debug.Log("Jump override for: " + survivorIndex);

                    ReflectionUtil.InvokeMethod(self, "GatherInputs");
                    CharacterMotor characterMotor = ReflectionUtil.GetPropertyValue<CharacterMotor>(self, "characterMotor");
                    bool jumpInputRecieved = ReflectionUtil.GetFieldValue<bool>(self, "jumpInputReceived");
                    int jumpCount = ReflectionUtil.GetFieldValue<int>(characterMotor, "jumpCount");
                    int maxJumpCount = ReflectionUtil.GetPropertyValue<int>(characterBody, "maxJumpCount");
                    if (jumpInputRecieved && jumpCount < maxJumpCount)
                    {
                        doJump = true;
                    }
                }
            }
            // Now call orig and override, if needed
            orig.Invoke(self);
            if(doJump)
            {
                Debug.Log($"Entering jump override state {survivorIndex} for {characterBody.name}");
                self.outer.SetNextState(JumpOverrides[survivorIndex]);
            }
        }

        public static void AddSkillOverride(ExpandedSkillSlot skillSlot, SurvivorIndex survivor, EntityState activationState)
        {
            if(skillSlot != ExpandedSkillSlot.Jump)
            {
                SkillSlot baseSlot = skillSlot.ToBaseSkillSlot();
            }
            else
            {
                JumpOverrides[survivor] = activationState;
            }
        }

        public static GenericSkill GetSkill(ExpandedSkillSlot expandedSlot, SurvivorIndex survivor)
        {
            GameObject survivorbodyPrefab = SurvivorCatalog.GetSurvivorDef(survivor).bodyPrefab;
            return GetSkill(expandedSlot, survivorbodyPrefab);
        }

        public static GenericSkill GetSkill(ExpandedSkillSlot expandedSlot, GameObject bodyPrefab)
        {
            GenericSkill skill = null;
            if(expandedSlot == ExpandedSkillSlot.Jump)
            {
                // Get jump skill when implemented...
            }
            else if(expandedSlot != ExpandedSkillSlot.None)
            {
                SkillLocator locator = bodyPrefab.GetComponent<SkillLocator>();
                skill = locator?.GetSkill(expandedSlot.ToBaseSkillSlot());
            }
            return skill;
        }

        private static void SetSkill(ExpandedSkillSlot expandedSlot, GameObject bodyPrefab)
        {

        }

        private static SkillLocator FindSkillLocator(SurvivorIndex survivor)
        {
            SkillLocator locator = SurvivorCatalog.GetSurvivorDef(survivor)?.bodyPrefab.GetComponent<SkillLocator>();
            return locator;
        }
        private static SkillLocator FindSkillLocator(int bodyIndex)
        {
            SkillLocator locator = BodyCatalog.GetBodyPrefab(bodyIndex)?.GetComponent<SkillLocator>();
            return locator;
        }



        /// <summary>
        /// Convert expanded skill slot to the original enum value (if possible)
        /// </summary>
        /// <param name="expandedSkillSlot"></param>
        /// <returns></returns>
        public static SkillSlot ToBaseSkillSlot(this ExpandedSkillSlot expandedSkillSlot)
        {
            switch (expandedSkillSlot)
            {
                case ExpandedSkillSlot.Primary:  return SkillSlot.Primary;
                case ExpandedSkillSlot.Secondary: return SkillSlot.Secondary;
                case ExpandedSkillSlot.Utility: return SkillSlot.Utility;
                case ExpandedSkillSlot.Special: return SkillSlot.Special;
                case ExpandedSkillSlot.Jump:
                    Debug.LogWarning("CANNOT CONVERT JUMP TO BASE SKILL SLOT!!");
                    return SkillSlot.None;
                default:
                    return SkillSlot.None;
            }

        }
    }
    
    public enum ExpandedSkillSlot : sbyte
    {
        None = -1,
        Primary = 0,
        Secondary = 1,
        Utility = 2,
        Special = 3,
        Jump = 4
    }
}
