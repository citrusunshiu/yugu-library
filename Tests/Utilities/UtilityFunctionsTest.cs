using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class UtilityFunctionsTest
    {
        [Test]
        public void UtilityFunctionsTestGetActiveUnitDetector()
        {
            /* set up the controllerhub; attach unitdetector object to udc; try to call the method, check for equality
             */
            throw new NotImplementedException();
        }

        [Test]
        public void UtilityFunctionsTestGetActivePlayer()
        {
            /* set up the controllerhub; attach player object to pc; try to call the method, check for equality
             */
            throw new NotImplementedException();
        }

        [Test]
        public void UtilityFunctionsTestSetSpriteDefaultPosition()
        {
            /*create unit, set transform randomly; check according to formulae:
             * 
             *   //for every x: x += 1.225, y += 0.6125; if x is negative, equations are negated
             *   xpos += (currentPosition.x * 1.225f);
             *   ypos += (currentPosition.x * 0.6125f);
             *
             *   //for every y: x -= 1.225, y += 0.6125; if y is negative, equations are negated
             *   xpos -= (currentPosition.y * 1.225f);
             *   ypos += (currentPosition.y * 0.6125f);
             *   float zpos = (currentPosition.z * 3.5f);
             * */
            throw new NotImplementedException();
        }

        [Test]
        public void UtilityFunctionsTestTranslateZAsY()
        {
            // set up a v3i; for every z, subtract x/y by 1; check positive, negative, zero
            throw new NotImplementedException();
        }
    }
}
