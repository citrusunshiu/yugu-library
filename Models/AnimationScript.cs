using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        public abstract class AnimationScript
        {
            #region Variables
            /// <summary>
            /// The amount of seconds elapsed in 1 frame. (60 FPS)
            /// </summary>
            readonly float SECONDS_PER_FRAME = 0.016667F;

            /// <summary>
            /// The OverworldObjectCoordinator object that the AnimationScript is attached to.
            /// </summary>
            protected OverworldObjectCoordinator overworldObjectCoordinator;

            /// <summary>
            /// 
            /// </summary>
            private string nameID;

            /// <summary>
            /// 
            /// </summary>
            private string spritesheetFileName;

            /// <summary>
            /// 
            /// </summary>
            private List<AnimationPattern> animationPatterns;

            /// <summary>
            /// The spriteesheet associated with the AnimationScript.
            /// </summary>
            protected object[] spritesheet;
            #endregion

            #region Constructors
            public AnimationScript()
            {

            }

            public AnimationScript(string animationScriptJSONFileName)
            {
                AnimationScriptJSONParser animationScriptJSONParser = new AnimationScriptJSONParser(animationScriptJSONFileName);

                InitializeAnimationScript(animationScriptJSONParser);
            }
            #endregion

            #region Functions
            private void InitializeAnimationScript(AnimationScriptJSONParser animationScriptJSONParser)
            {
                nameID = animationScriptJSONParser.GetNameID();
                spritesheetFileName = animationScriptJSONParser.GetSpritesheetFileName();
                animationPatterns = animationScriptJSONParser.GetAnimationPatterns();
            }

            public void AttachOverworldObjectCoordinator(OverworldObjectCoordinator overworldObjectCoordinator)
            {
                this.overworldObjectCoordinator = overworldObjectCoordinator;
            }

            public void PlayAnimation()
            {
                //TODO: placeholder code; fix later
                for (int i = 0; i < 60; i++)
                {
                    overworldObjectCoordinator.animationQueue.Enqueue(Animate(i, SECONDS_PER_FRAME * 10));
                }
            }

            /// <summary>
            /// Changes the currently displayed sprite after a specified delay.
            /// </summary>
            /// <param name="index">Spritesheet index of the sprite to change to.</param>
            /// <param name="waitSeconds">Amount of seconds to wait before the sprite can be changed again.</param>
            /// <returns></returns>
            IEnumerator Animate(int index, float waitSeconds)
            {
                overworldObjectCoordinator.currentlyAnimating = true;
                overworldObjectCoordinator.spriteRenderer.sprite = (Sprite)spritesheet[index + 1];
                yield return new WaitForSeconds(waitSeconds);
                overworldObjectCoordinator.currentlyAnimating = false;
            }

            /// <summary>
            /// Plays an animation using a script of specified spritesheet indices and wait times.
            /// </summary>
            /// <param name="spriteIndices">Indices of the sprites to be used.</param>
            /// <param name="framesPerSprite">Wait time for every sprite change.</param>
            void AnimateScript(int[] spriteIndices, int[] framesPerSprite)
            {
                for (int i = 0; i < spriteIndices.Length; i++)
                {
                    overworldObjectCoordinator.animationQueue.Enqueue(Animate(spriteIndices[i], framesPerSprite[i]));
                }
            }

            void ChangeColorTint()
            {

            }
            #endregion
        }
    }
    

}
