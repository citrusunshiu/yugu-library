﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuguLibrary
{
    namespace Models
    {
        public class AnimationPattern
        {
            private string nameID;
            private int totalFrames;
            private List<AnimationPatternSignal> animationPatternSignals;

            public AnimationPattern(string nameID, int totalFrames, List<AnimationPatternSignal> animationPatternSignals)
            {
                this.nameID = nameID;
                this.totalFrames = totalFrames;
                this.animationPatternSignals = animationPatternSignals;
            }

            public List<AnimationPatternSignal> GetAnimationPatternSignals()
            {
                return animationPatternSignals;
            }

            public int GetTotalFrames()
            {
                return totalFrames;
            }
        }

        public class AnimationPatternSignal
        {
            private int startFrame;
            private int spriteIndex;

            public AnimationPatternSignal(int startFrame, int spriteIndex)
            {
                this.startFrame = startFrame;
                this.spriteIndex = spriteIndex;
            }

            public int GetStartFrame()
            {
                return startFrame;
            }

            public int GetSpriteIndex()
            {
                return spriteIndex;
            }
        }

    }
}
