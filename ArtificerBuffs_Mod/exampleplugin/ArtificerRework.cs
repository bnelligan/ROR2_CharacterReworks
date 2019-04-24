using BepInEx;
using RoR2;
using EntityStates;
using UnityEngine;


namespace BnellsCharacterReworks.ArtificerRework
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.bnelligan.artrework", "ArtificerRework", "1.0.6")]
    public class ArtificerRework : BaseUnityPlugin
    {
        public ArtificerRework()
        {
            SkillAPI.InitHooks();
        }
        
        public void Awake()
        {
            SkillAPI.AddSkillOverride(ExpandedSkillSlot.Jump, SurvivorIndex.Engineer, new MageBlinkState());

        }
    }

    
}