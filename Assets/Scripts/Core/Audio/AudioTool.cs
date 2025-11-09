using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

namespace ilsFramework.Core
{
    public static class AudioTool
    {
        public static AudioMixerGroup FindCurrentMixerGroup(AudioMixer mixer, string audioMixerGroupName)
        {
            var result = mixer.FindMatchingGroups(audioMixerGroupName);
            if (result.Length == 0) result = mixer.FindMatchingGroups("Master");
            return result[0];
        }

        public static float RemapVolumeTodB(float volume, float min = -80, float max = 0)
        {
            return math.lerp(min, max, volume);
        }

        public static void MixerParamterSafeSetFloat(AudioMixer mixer, string paramName, float value)
        {
            if (!mixer)
            {
                Debug.LogError("mixer为空");
                return;
            }

            if (string.IsNullOrEmpty(paramName)) return;
            if (!mixer.GetFloat(paramName, out var currentValue))
            {
                Debug.LogError($"参数 {paramName} 不存在!");
                return;
            }

            // 设置参数值
            mixer.SetFloat(paramName, value);
        }
        
        // 百分比转分贝
        public static float PercentToDB(float percent)
        {
            percent = Mathf.Clamp01(percent);
            if (percent <= 0.0001f) return -80f; // 静音
            return 20f * Mathf.Log10(percent);
        }
    
        // 分贝转百分比
        public static float DBToPercent(float db)
        {
            if (db <= -80f) return 0f;
            return Mathf.Pow(10f, db / 20f);
        }
    }
}