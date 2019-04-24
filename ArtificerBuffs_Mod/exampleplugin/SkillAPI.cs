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
using System.Collections.Generic;

namespace BnellsCharacterReworks
{
    public static class SkillAPI
    {
        public static void InitHooks()
        {
            IL.EntityStates.GenericCharacterMain.FixedUpdate += GenericCharacterMain_FixedUpdate;
        }

        private static void GenericCharacterMain_FixedUpdate(ILContext il)
        {
            ILCursor c = il.
        }

        public static void AddSkillOverride(ExpandedSkillSlot skillSlot, SurvivorIndex survivor, SkillOverride skillOverride)
        {
            if((sbyte)skillSlot <= (sbyte)SkillSlot.Special)
            {

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
    public class SkillOverride : GenericSkill
    {
        ExpandedSkillSlot skillSlot;
        GenericSkill origSkill;
        
        public SkillOverride(GenericSkill originalSkill)
        {

            
        }
        public SkillOverride(ExpandedSkillSlot skillSlot, SurvivorIndex survivor)
        {
            origSkill = this;
        }
    }
}
