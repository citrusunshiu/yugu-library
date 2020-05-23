using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        public class Cutscene
        {
            string nameID;
            List<Scene> scenes;

            public Cutscene(string cutsceneJSONFileName)
            {
                CutsceneJSONParser cutsceneJSONParser = new CutsceneJSONParser(cutsceneJSONFileName);

                InitializeCutsceneValues(cutsceneJSONParser);
            }

            private void InitializeCutsceneValues(CutsceneJSONParser cutsceneJSONParser)
            {
                nameID = cutsceneJSONParser.GetNameID();
                scenes = cutsceneJSONParser.GetScenes();
            }


        }

        public class Scene
        {
            string instanceJSONFileName;

            int year;
            Seasons season;
            int date;
            Times time;

            bool isCinematic;

            List<string> unitsToPlace;
            List<Vector3Int> unitPositions;

            List<SceneChoreography> sceneChoreographies;

            public Scene(string instanceJSONFileName, int year, Seasons season, int date, Times time, bool isCinematic, 
                List<string> unitsToPlace, List<Vector3Int> unitPositions, List<SceneChoreography> sceneChoreographies)
            {
                this.instanceJSONFileName = instanceJSONFileName;
                this.year = year;
                this.season = season;
                this.date = date;
                this.time = time;

                this.isCinematic = isCinematic;

                this.unitsToPlace = unitsToPlace;
                this.unitPositions = unitPositions;

                this.sceneChoreographies = sceneChoreographies;
            }
        }

        public class SceneChoreography
        {
            Dialogue dialogue;
            List<SceneAnimation> sceneAnimations;

            public SceneChoreography(Dialogue dialogue, List<SceneAnimation> sceneAnimations)
            {
                this.dialogue = dialogue;
                this.sceneAnimations = sceneAnimations;

            }
        }

        public class Dialogue
        {
            string dialogueSpeaker;
            string dialogueText;
            string portraitFileName;
            int portraitEmote;

            public Dialogue(string dialogueSpeaker, string dialogueText, string portraitFileName, int portraitEmote)
            {
                this.dialogueSpeaker = dialogueSpeaker;
                this.dialogueText = dialogueText;
                this.portraitFileName = portraitFileName;
                this.portraitEmote = portraitEmote;
            }

            public Dialogue()
            {

            }
        }

        public class SceneAnimation
        {
            string unitName;
            int unitIndex;
            Vector3Int moveToTile;
            int animationIndex;

            public SceneAnimation(string unitName, int unitIndex, Vector3Int moveToTile, int animationIndex)
            {
                this.unitName = unitName;
                this.unitIndex = unitIndex;
                this.moveToTile = moveToTile;
                this.animationIndex = animationIndex;
            }
        }
    }
}
