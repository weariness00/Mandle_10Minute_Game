using DG.Tweening;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GamePlay
{
    public class PostProcessingUtility : MonoBehaviour
    {
        public Volume volume;

        private Dictionary<string, Sequence> sequenceDictionary = new Dictionary<string, Sequence>();

        private Sequence GetSequence(string key)
        {
            if (key == null) return null;
            sequenceDictionary.TryGetValue(key, out var sequence);
            return sequence;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="power"> 빛의 밝기 현재 민감도에서 +로 적용 </param>
        /// <param name="moveDuration">power로 이동하는 시간</param>
        /// <param name="stayDuration">power에 도달했을때 체류하는 시간</param>
        public void Bright(float power, float moveDuration, float stayDuration)
        {
            string key = MethodBase.GetCurrentMethod()?.Name;
            var sequence = GetSequence(key);
            sequence?.Kill();
            sequence = DOTween.Sequence();
            {   // 블룸
                if (!volume.profile.TryGet(out Bloom bloom))
                    bloom = volume.profile.Add<Bloom>(true);
                bloom.active = true;
                bloom.threshold.overrideState = true;
                bloom.intensity.overrideState = true;

                var originValue = bloom.intensity.value;

                sequence.OnKill(() =>
                {
                    bloom.intensity.value = originValue;
                });
                
                sequence.Append(DOTween.To(
                    () => bloom.intensity.value,
                    value => bloom.intensity.value = value,
                    originValue + power,
                    moveDuration)
                    .SetEase(Ease.Flash));
                sequence.AppendInterval(stayDuration);
                sequence.Append(DOTween.To(
                        () => bloom.intensity.value,
                        value => bloom.intensity.value = value,
                        originValue,
                        moveDuration)
                    .SetEase(Ease.Flash));
            }
        }
        
        /// <summary>
        /// 피곤함으로 화면이 깜박깜박 암전한다.
        /// </summary>
        /// <param name="repeatCount"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="duration"></param>
        /// <param name="isReset">원래 값으로 돌아갈 것인지</param>
        public void Tired(int repeatCount, float duration)
        {
            if(repeatCount <= 0) return;
            
            string key = MethodBase.GetCurrentMethod()?.Name;
            var sequence = GetSequence(key);
            sequence?.Kill();
            sequence = DOTween.Sequence();
            { // Vignette 조절
                
                var vignetteSequence = DOTween.Sequence();
                if(!volume.profile.TryGet(out Vignette vignette))
                    vignette = volume.profile.Add<Vignette>(true);
                vignette.active = true;
                vignette.smoothness.overrideState = true;
                vignette.smoothness.value = 1f;
            
                vignette.intensity.overrideState = true;

                var originValue = vignette.intensity.value;

                sequence.OnKill(() =>
                {
                    vignette.intensity.value = originValue;
                });
                
                vignetteSequence.Append(DOTween.To(
                        () => vignette.intensity.value,
                        value => vignette.intensity.value = value,
                        1f,
                        duration)
                    .SetLoops(repeatCount, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine));

                sequence.Join(vignetteSequence);
            }
            {   // Color Adjustments
                var colorAdjustmentsSequence = DOTween.Sequence();
                if (!volume.profile.TryGet(out ColorAdjustments colorAdjustments))
                    colorAdjustments = volume.profile.Add<ColorAdjustments>(true);

                colorAdjustments.active = true;
                colorAdjustments.postExposure.overrideState = true;
                colorAdjustments.contrast.overrideState = true;
                colorAdjustments.colorFilter.overrideState = true;

                var originPostExposure = colorAdjustments.postExposure.value;
                var originContrast = colorAdjustments.contrast.value;
                var originColorFilter = colorAdjustments.colorFilter.value;
                
                colorAdjustmentsSequence.OnKill(() =>
                {
                    colorAdjustments.postExposure.value = originPostExposure;
                    colorAdjustments.contrast.value = originContrast;
                    colorAdjustments.colorFilter.value = originColorFilter;
                });
                
                colorAdjustmentsSequence.Join(DOTween.To(
                    () => colorAdjustments.postExposure.value,
                    value => colorAdjustments.postExposure.value = value,
                    -1,
                    duration)
                    .SetLoops(repeatCount, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine));
                
                colorAdjustmentsSequence.Join(DOTween.To(
                        () => colorAdjustments.contrast.value,
                        value => colorAdjustments.contrast.value = value,
                        -10,
                        duration)
                    .SetLoops(repeatCount, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine));
                
                sequence.Join(colorAdjustmentsSequence);
            }
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(PostProcessingUtility))]
    public class PostProcessingUtilityEditor : Editor
    {
        private float brightMoveDuration;
        private float brightStayDuration;
        private float brightPower = 1f;
        
        private int tiredRepeatCount = 1;
        private float tiredDuration = 2f;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as PostProcessingUtility;
            if (EditorApplication.isPlaying)
            {
                GUILayout.Label("화면 밝기 조절 관련");
                brightMoveDuration = EditorGUILayout.FloatField("Power에 도달하는 시간", brightMoveDuration);
                brightStayDuration = EditorGUILayout.FloatField("Power에 도달했을대 체류 시간", brightStayDuration);
                brightPower = EditorGUILayout.FloatField("밝기 추가 Power", brightPower);
                if (GUILayout.Button("화면 밝기 시작"))
                {
                    script.Bright(brightPower, brightMoveDuration, brightStayDuration);
                }
                
                GUILayout.Label("피곤함 관련");
                tiredRepeatCount = EditorGUILayout.IntField("암전 횟수", tiredRepeatCount);
                tiredDuration = EditorGUILayout.FloatField("지속 시간", tiredDuration);
                if (GUILayout.Button("피곤함 시작"))
                {
                    script.Tired(tiredRepeatCount, tiredDuration);
                }
            }
        }
    }

#endif
}

