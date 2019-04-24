using System;
using EntityStates.Huntress;
using RoR2;
using UnityEngine;
using EntityStates;

namespace BnellsCharacterReworks.ArtificerRework
{
    // Token: 0x020006EF RID: 1775
    public class MageBlinkState : BaseState
    {
        // Token: 0x060027A9 RID: 10153 RVA: 0x000B8758 File Offset: 0x000B6958
        public override void OnEnter()
        {
            base.OnEnter();
            this.SetStaticParams();
            Util.PlaySound(MageBlinkState.beginSoundString, base.gameObject);
            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
            }
            if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            base.characterMotor.velocity *= MageBlinkState.speedCoefficient;
            this.blinkVector = base.characterMotor.velocity;
            this.duration = this.blinkVector.magnitude / 100f;
            this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
        }

        // Token: 0x060027AA RID: 10154 RVA: 0x000B8854 File Offset: 0x000B6A54
        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkVector);
            effectData.origin = origin;
            EffectManager.instance.SpawnEffect(MageBlinkState.blinkPrefab, effectData, false);
        }

        // Token: 0x060027AB RID: 10155 RVA: 0x0001CBA3 File Offset: 0x0001ADA3
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        // Token: 0x060027AC RID: 10156 RVA: 0x000B8890 File Offset: 0x000B6A90
        public override void OnExit()
        {
            if (!this.outer.destroying)
            {
                Util.PlaySound(MageBlinkState.endSoundString, base.gameObject);
                this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                this.modelTransform = base.GetModelTransform();
                if (this.modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 0.6f;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashBright");
                    temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                    TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay2.duration = 0.7f;
                    temporaryOverlay2.animateShaderAlpha = true;
                    temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay2.destroyComponentOnEnd = true;
                    temporaryOverlay2.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashExpanded");
                    temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                }
            }
            if (this.characterModel)
            {
                this.characterModel.invisibilityCount--;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            base.OnExit();
        }

        // Token: 0x060027AD RID: 10157 RVA: 0x0001CBDE File Offset: 0x0001ADDE
        private void SetStaticParams()
        {
            MageBlinkState.beginSoundString = BlinkState.beginSoundString;
            MageBlinkState.blinkPrefab = BlinkState.blinkPrefab;
            MageBlinkState.endSoundString = BlinkState.endSoundString;
            MageBlinkState.speedCoefficient = 1.2f;
        }

        // Token: 0x040029DD RID: 10717
        private Transform modelTransform;

        // Token: 0x040029DE RID: 10718
        public static GameObject blinkPrefab;

        // Token: 0x040029DF RID: 10719
        private float stopwatch;

        // Token: 0x040029E0 RID: 10720
        private Vector3 blinkVector = Vector3.zero;

        // Token: 0x040029E1 RID: 10721
        private float duration = 0.3f;

        // Token: 0x040029E2 RID: 10722
        public static float speedCoefficient = 25f;

        // Token: 0x040029E3 RID: 10723
        public static string beginSoundString;

        // Token: 0x040029E4 RID: 10724
        public static string endSoundString;

        // Token: 0x040029E5 RID: 10725
        private CharacterModel characterModel;

        // Token: 0x040029E6 RID: 10726
        private HurtBoxGroup hurtboxGroup;
    }
}
