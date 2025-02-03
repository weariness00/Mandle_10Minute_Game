using UnityEditor;
using UnityEngine;

namespace Util.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxValue<float>))]
    public class MinMaxValueFloatPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty min;
        private SerializedProperty max;
        private SerializedProperty current;

        private bool isAdjustingSlider = false;
        private float currentValueInterval = 30;
        private float minValueInterval = 30;
        private float maxValueInterval = 30;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
                
            // 슬라이더를 조작중인지 이벤트 확인
            if (Event.current.type == EventType.MouseDown)
                isAdjustingSlider = true; // 슬라이더 조작 시작
            else if(Event.current.type == EventType.MouseUp)
                isAdjustingSlider = false; // 슬라이더 조작 종료
            
            max = property.FindPropertyRelative("_max");
            min = property.FindPropertyRelative("_min");
            current = property.FindPropertyRelative("_current");
            
            Rect labelPosition = new Rect(position.x, position.y, position.width, position.height);
            position = EditorGUI.PrefixLabel(
                labelPosition,
                EditorGUIUtility.GetControlID(FocusType.Passive),
                label
            );

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float sumInterval = 0;
            
            // current Value 필드
            int digitCount = current.floatValue.GetDigitCount();
            if(!isAdjustingSlider) currentValueInterval = digitCount < 3 ? 25 : digitCount * 11;
            var intFieldPos = new Rect(position.x + sumInterval, position.y, currentValueInterval, position.height);
            current.floatValue = EditorGUI.FloatField(intFieldPos, current.floatValue);
            if (current.floatValue < min.floatValue)
                current.floatValue = min.floatValue;
            else if (current.floatValue > max.floatValue)
                current.floatValue = max.floatValue;
            sumInterval += currentValueInterval + 10;

            // Slider 필드
            float sliderInterval = EditorGUIUtility.currentViewWidth - 250 - minValueInterval - maxValueInterval;
            if (sliderInterval < 50) sliderInterval = 50;
            else if(sliderInterval > 200) sliderInterval = 200;
            var currentSliderPos = new Rect(position.x + sumInterval, position.y, sliderInterval, position.height);
            current.floatValue = GUI.HorizontalSlider(currentSliderPos, current.floatValue, min.floatValue, max.floatValue);
            sumInterval += sliderInterval + 10;
                
            // Min Value 필드
            int minDigitCount = min.floatValue.GetDigitCount();
            minValueInterval = minDigitCount <= 2 ? 25 : minDigitCount * 11; 
            var minPos = new Rect(position.x + sumInterval, position.y, minValueInterval, position.height);
            min.floatValue = EditorGUI.FloatField(minPos, min.floatValue);
            sumInterval += minValueInterval;
                
            int textInterval = 20;
            var rangeTextPos = new Rect(position.x + sumInterval, position.y, textInterval, position.height);
            EditorGUI.LabelField(rangeTextPos, $" ~ ");
            sumInterval += textInterval;
                
            int maxDigitCount = max.floatValue.GetDigitCount();
            maxValueInterval = maxDigitCount <= 2 ? 25 : maxDigitCount * 11; 
            var maxPos = new Rect(position.x + sumInterval, position.y, maxValueInterval, position.height);
            max.floatValue = EditorGUI.FloatField(maxPos, max.floatValue);
            sumInterval += maxValueInterval;

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        [CustomPropertyDrawer(typeof(MinMaxValue<int>))]
        public class MinMaxValueIntegerPropertyDrawer : PropertyDrawer
        {
            private SerializedProperty min;
            private SerializedProperty max;
            private SerializedProperty current;
            
            private bool isAdjustingSlider = false;
            private float currentValueInterval = 30;
            private float minValueInterval = 30;
            private float maxValueInterval = 30;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);

                max = property.FindPropertyRelative("_max");
                min = property.FindPropertyRelative("_min");
                current = property.FindPropertyRelative("_current");
                
                // 슬라이더를 조작중인지 이벤트 확인
                if (Event.current.type == EventType.MouseDown)
                    isAdjustingSlider = true; // 슬라이더 조작 시작
                else if(Event.current.type == EventType.MouseUp)
                    isAdjustingSlider = false; // 슬라이더 조작 종료
                
                // Label 필드
                Rect labelPosition = new Rect(position.x, position.y, label.text.Length * 1, position.height);
                position = EditorGUI.PrefixLabel(
                    labelPosition,
                    EditorGUIUtility.GetControlID(FocusType.Passive),
                    label
                );

                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                float sumInterval = 0; // 전체 간격 길이
                
                // current int 필드 값
                int digitCount = current.intValue.GetDigitCount();
                if(!isAdjustingSlider) currentValueInterval = digitCount <= 3 ? 25 : digitCount * 10;
                var intFieldPos = new Rect(position.x + sumInterval, position.y, currentValueInterval, position.height);
                current.intValue = EditorGUI.IntField(intFieldPos, current.intValue);
                if (current.intValue < min.intValue)
                    current.intValue = min.intValue;
                else if (current.intValue > max.intValue)
                    current.intValue = max.intValue;
                sumInterval += currentValueInterval + 10;

                // slider 필드 값
                float sliderInterval = EditorGUIUtility.currentViewWidth - 250 - minValueInterval - maxValueInterval;
                if (sliderInterval < 50) sliderInterval = 50;
                else if(sliderInterval > 200) sliderInterval = 200;
                var currentSliderPos = new Rect(position.x + sumInterval, position.y, sliderInterval, position.height);
                current.intValue = (int)GUI.HorizontalSlider(currentSliderPos, current.intValue, min.intValue, max.intValue);
                sumInterval += sliderInterval + 10;
                
                // min value 필드 값
                int minDigitCount = min.intValue.GetDigitCount();
                minValueInterval = minDigitCount <= 2 ? 25 : minDigitCount * 9; 
                var minPos = new Rect(position.x + sumInterval, position.y, minValueInterval, position.height);
                min.intValue =EditorGUI.IntField(minPos, min.intValue);
                sumInterval += minValueInterval;
                
                // "~" 문자열 필드
                int textInterval = 20;
                var rangeTextPos = new Rect(position.x + sumInterval, position.y, textInterval, position.height);
                EditorGUI.LabelField(rangeTextPos, $" ~ ");
                sumInterval += textInterval;
                
                // Max Value 필드 값
                int maxDigitCount = max.intValue.GetDigitCount();
                maxValueInterval = maxDigitCount <= 2 ? 25 : maxDigitCount * 9; 
                var maxPos = new Rect(position.x + sumInterval, position.y, maxValueInterval, position.height);
                max.intValue = EditorGUI.IntField(maxPos, max.intValue);
                sumInterval += maxValueInterval;
                
                EditorGUI.indentLevel = indent;
                EditorGUI.EndProperty();

                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
