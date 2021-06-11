using HarmonyLib;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(ResultsViewController), nameof(ResultsViewController.Init),
        new Type[] { typeof(LevelCompletionResults), typeof(IDifficultyBeatmap), typeof(bool), typeof(bool) })]
    public class ResultViewPatch
    {
        public static void Prefix(ref LevelCompletionResults levelCompletionResults, ref IDifficultyBeatmap difficultyBeatmap, ref bool practice, ref bool newHighScore)
        {

#if DEBUG
            levelCompletionResults.SetField("levelEndStateType", LevelCompletionResults.LevelEndStateType.Cleared);
            practice = false;
            newHighScore = true;
#endif
        }
    }
}
