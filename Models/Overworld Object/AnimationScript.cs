using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        public class AnimationScript
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

            private int animationPatternIndex;

            private int framesGiven = -1;

            private bool isLooping;
            #endregion

            #region Constructors
            public AnimationScript()
            {

            }

            public AnimationScript(string animationScriptJSONFileName)
            {
                isLooping = true;
                animationPatternIndex = 0;
                
                AnimationScriptJSONParser animationScriptJSONParser = new AnimationScriptJSONParser(animationScriptJSONFileName);

                InitializeAnimationScript(animationScriptJSONParser);

                spritesheet = Resources.LoadAll(UtilityFunctions.UNIT_SPRITESHEET_FILE_PATH + spritesheetFileName);
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

            public void SetAnimationPatternIndex(int animationPatternIndex)
            {
                this.animationPatternIndex = animationPatternIndex;
            }

            public void SetFramesGiven(int framesGiven)
            {
                this.framesGiven = framesGiven;
            }

            public void SetIsLooping(bool isLooping)
            {
                this.isLooping = isLooping;
            }

            public bool IsLooping()
            {
                return isLooping;
            }

            public void PlayAnimation()
            {
                AnimationPattern animationPattern = animationPatterns[animationPatternIndex];
                List<AnimationPatternSignal> animationPatternSignals = animationPattern.GetAnimationPatternSignals();

                float animationSpeedModifier = 1;

                if(framesGiven > 0)
                {
                    animationSpeedModifier = animationPattern.GetTotalFrames() / framesGiven;
                }

                int i = 0;

                foreach(AnimationPatternSignal signal in animationPatternSignals)
                {
                    if(i == 0)
                    {
                        overworldObjectCoordinator.animationQueue.Enqueue(Animate(signal.GetSpriteIndex(), 
                            SECONDS_PER_FRAME * signal.GetStartFrame() * animationSpeedModifier));
                    }
                    else
                    {
                        int frameDifference = GetFrameDifference(signal, animationPatternSignals[i - 1]);

                        overworldObjectCoordinator.animationQueue.Enqueue(Animate(signal.GetSpriteIndex(), 
                            SECONDS_PER_FRAME * frameDifference * animationSpeedModifier));

                    }
                    i++;
                }
            }

            private int GetFrameDifference(AnimationPatternSignal currentSignal, AnimationPatternSignal previousSignal)
            {
                return currentSignal.GetStartFrame() - previousSignal.GetStartFrame();
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
                yield return new WaitForSeconds(waitSeconds);
                overworldObjectCoordinator.spriteRenderer.sprite = (Sprite)spritesheet[index + 1];
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
