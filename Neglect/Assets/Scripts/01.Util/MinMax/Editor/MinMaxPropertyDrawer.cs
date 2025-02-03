using UnityEditor;
using UnityEngine.UIElements;

namespace Util.Editor
{
    [CustomPropertyDrawer(typeof(MinMax<int>))]
    public class MinMaxIntPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row; // 가로 정렬
            
            var propertyLabel = new Label(property.displayName); // 부모 변수 이름 가져오기
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");

            var minField = new IntegerField("");
            var maxField = new IntegerField("");
            
            minField.style.marginRight = 5;

            minField.value = min.intValue;
            minField.style.width = 100;
            minField.RegisterValueChangedCallback(evt =>
            {
                min.intValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            
            maxField.value = max.intValue;
            maxField.style.width = 100;
            maxField.RegisterValueChangedCallback(evt =>
            {
                max.intValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            
            container.Add(propertyLabel);
            container.Add(minField);
            container.Add(new Label("~"));
            container.Add(maxField);

            return container;
        }
    }
    
    [CustomPropertyDrawer(typeof(MinMax<float>))]
    public class MinMaxFloatPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row; // 가로 정렬
            
            var propertyLabel = new Label(property.displayName); // 부모 변수 이름 가져오기
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");

            var minField = new FloatField("");
            var maxField = new FloatField("");
            
            minField.style.marginRight = 5;

            minField.value = min.floatValue;
            minField.style.width = 100;
            minField.RegisterValueChangedCallback(evt =>
            {
                min.floatValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            
            maxField.value = max.floatValue;
            maxField.style.width = 100;
            maxField.RegisterValueChangedCallback(evt =>
            {
                max.floatValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            
            container.Add(propertyLabel);
            container.Add(minField);
            container.Add(new Label("~"));
            container.Add(maxField);

            return container;
        }
    }
}