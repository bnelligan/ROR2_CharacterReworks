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
            
        }
        
        public void Awake()
        {
            // Add blink as a jump skill
            // Add freeze dash as utility skill
        }


        public void Update()
        {
            
        }
        
    }

    
}