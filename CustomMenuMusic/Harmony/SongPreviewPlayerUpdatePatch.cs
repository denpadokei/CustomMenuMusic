using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(SongPreviewPlayer), nameof(SongPreviewPlayer.Update))]
    public class SongPreviewPlayerUpdatePatch
    {
        private static readonly CodeInstruction s_arguments = new CodeInstruction(OpCodes.Ldc_R4, -1f);
        /// <summary>
        /// Update関数内で呼ばれる<see cref="SongPreviewPlayer.CrossfadeTo(AudioClip, float, float, float, bool, Action)"/>のsongStartTimeを-1にする処理。
        /// 絶対バグる気がする
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions.Select((x, i) => new { x, i })) {
                yield return code.x.opcode == OpCodes.Ldc_R4 && (float)code.x.operand == 0f && code.i == 104 ? s_arguments : code.x;
            }
        }
    }
}
