using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Util
{
    static class CSVReaderExtension
    {
        public static T DynamicCast<T>(this object objectValue) => objectValue.DynamicCast<T>(default);
        public static T DynamicCast<T>(this object objectValue, T defaultValue)
        {
            if (objectValue is T value)
                return value;
            if (typeof(T) == typeof(string))
            {
                if (objectValue is string[] stringArray)
                {
                    return (T)(object)string.Join("", stringArray);
                }
                if (objectValue is List<string> stringList)
                {
                    return (T)(object)string.Join("", stringList);
                }
            }
            
            if (typeof(T).IsArray)
            {
                var type = typeof(T).GetElementType();
                var array = Array.CreateInstance(typeof(T).GetElementType(), 1);
                if (objectValue.GetType() == typeof(T).GetElementType())
                    array.SetValue(objectValue, 0);
                else
                    array = Array.CreateInstance(type, 0);
                if (array is T value2)
                {
                    return value2;
                }
            }

            // T가 List일 때
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                var list = (System.Collections.IList)Activator.CreateInstance(typeof(T));
                if (objectValue.GetType() == typeof(T).GetElementType())
                {
                    list.Add(objectValue); // 단일 값을 리스트에 추가
                }

                return (T)list; // List<T>를 T로 변환
            }
            return defaultValue;
        }
        public static T DynamicCast<T>(this Dictionary<string, object> dictionary, string key) => dictionary.DynamicCast<T>(key, default);
        public static T DynamicCast<T>(this Dictionary<string,object> dictionary, string key, T defaultValue)
        {
            if (dictionary.TryGetValue(key, out var objectValue))
            {
                if (objectValue is T value)
                    return value;
                if (typeof(T) == typeof(string))
                {
                    if (objectValue is string[] stringArray)
                    {
                        return (T)(object)string.Join("", stringArray);
                    }
                    if (objectValue is List<string> stringList)
                    {
                        return (T)(object)string.Join("", stringList);
                    }
                }
                if (typeof(T).IsArray)
                {
                    var type = typeof(T).GetElementType();
                    var array = Array.CreateInstance(typeof(T).GetElementType(), 1);
                    if (objectValue.GetType() == typeof(T).GetElementType())
                        array.SetValue(objectValue, 0);
                    else
                        array = Array.CreateInstance(type, 0);
                    if (array is T value2)
                        return value2;
                }

                // T가 List일 때
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                {
                    var list = (System.Collections.IList)Activator.CreateInstance(typeof(T));
                    if (objectValue.GetType() == typeof(T).GetElementType())
                    {
                        list.Add(objectValue); // 단일 값을 리스트에 추가
                    }

                    return (T)list; // List<T>를 T로 변환
                }
            }
            return defaultValue;
        }

        public static List<T> DynamicCastList<T>(this List<object> objectValue) => objectValue.DynamicCastList<T>(default);
        public static List<T> DynamicCastList<T>(this List<object> objectValue, T defaultData)
        {
            List<T> list = new(objectValue.Count);
            foreach (T value in objectValue)
            {
                list.Add(value.DynamicCast<T>(defaultData));
            }
            return list;
        }
        
        public static List<Dictionary<string,object>> ReadHorizon(this TextAsset csvFile)
        {
            Debug.Assert(csvFile != null, "CSV파일이 없어서 데이터를 셋팅하지 못했습니다.");
            var path = GetResourcePathToCSV(csvFile);
            var csv = CSVReader.ReadHorizon(path);
            return csv;
        }

        public static Dictionary<string, List<object>> ReadVertical(this TextAsset csvFile)
        {
            Debug.Assert(csvFile != null, "CSV파일이 없어서 데이터를 셋팅하지 못했습니다.");
            var path = GetResourcePathToCSV(csvFile);
            var csv = CSVReader.ReadVertical(path);
            return csv;
        }

        private static string GetResourcePathToCSV(this TextAsset csvFile)
        {
            var path = AssetDatabase.GetAssetPath(csvFile);
            
            // Resource 포함 이전 경로 제거
            string[] parts = path.Split('/');
            int index = Array.IndexOf(parts, "Resources");
            if (index != -1 && index + 1 < parts.Length)
                path = string.Join("/", parts.Skip(index + 1));
            
            // 확장자 제거
            path = path.Replace(".csv", "");
            return path;
        }
    }
    
    public class CSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"(?:\r\n|\n\r|\r)(?=(?:[^""]*""[^""]*"")*[^""]*$)";
        static char[] TRIM_CHARS = { '\"' };
        
        public static T DynamicCast<T>(object objectValue) where T : new()
        {
            if (objectValue is T value)
                return value;
            return new T();
        }
        
        public static List<Dictionary<string, object>> ReadHorizon(string file)
        {
            var list = new List<Dictionary<string, object>>();
            TextAsset data = Resources.Load<TextAsset>(file);
            if (data == null) return new();

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            // 나중에 값이 안들어 있는 경우에 빈 값을 넣어주도록 바꾸어야한다.
            var headerData = FindHeader(data, "header");
            var header = headerData.Item1;
            var headerIndex = headerData.Item2;
            for (var i = headerIndex; i < lines.Length - 1; i++)
            {
                var lineValues = Regex.Split(lines[i], SPLIT_RE);
                if (lineValues.Length == 0 || lineValues[0] == "//" || lineValues[0].ToLower() == "header") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < lineValues.Length; j++)
                {
                    string value = lineValues[j];
                    if (value.StartsWith(TRIM_CHARS[0]) && value.EndsWith(TRIM_CHARS[0]))
                    {
                        value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                        var values = Regex.Split(value, SPLIT_RE);
                        bool isString = true;
                        bool isFloat = false;
                        foreach (string v in values)
                        {
                            if (int.TryParse(v, out var nn))
                            {
                                isString = false;
                            }
                            else if (float.TryParse(v, out var ff))
                            {
                                isFloat = true;
                                isString = false;
                                break;
                            }
                        }

                        object objectValue = null;
                        if (isString)
                        {
                            objectValue = values;
                        }
                        else if (isFloat)
                        {
                            float[] f = new float[values.Length];
                            for (var index = 0; index < values.Length; index++)
                                float.TryParse(values[index], out f[index]);
                            objectValue = f;
                        }
                        else
                        {
                            int[] n = new int[values.Length];
                            for (var index = 0; index < values.Length; index++)
                                int.TryParse(values[index], out n[index]);
                            objectValue = n;
                        }
                        entry[header[j]] = objectValue;
                    }
                    else
                    {
                        value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                        object finalvalue = value;
                        int n;
                        float f;
                        if (int.TryParse(value, out n))
                        {
                            finalvalue = n;
                        }
                        else if (float.TryParse(value, out f))
                        {
                            finalvalue = f;
                        }
                        entry[header[j]] = finalvalue;
                    }
                }
                list.Add(entry);
            }
            return list;
        }

        public static Dictionary<string, List<object>> ReadVertical(string file)
        {
            Dictionary<string, List<object>> entry = new();
            TextAsset data = Resources.Load<TextAsset>(file);
            if (data == null) return new();

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return entry;

            // 나중에 값이 안들어 있는 경우에 빈 값을 넣어주도록 바꾸어야한다.
            var headerData = FindHeader(data, "header");
            var header = headerData.Item1;
            var headerIndex = headerData.Item2;

            foreach (string str in header)
                entry.Add(str, new());
            
            for (var i = headerIndex + 1; i < lines.Length - 1; i++)
            {
                var lineValues = Regex.Split(lines[i], SPLIT_RE);
                if (lineValues.Length == 0 || lineValues[0] == "//" || lineValues[0].ToLower() == "header") continue;
                for (var j = 0; j < header.Length && j < lineValues.Length; j++)
                {
                    string value = lineValues[j];
                    if (value.StartsWith(TRIM_CHARS[0]) && value.EndsWith(TRIM_CHARS[0]))
                    {
                        value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                        var values = Regex.Split(value, SPLIT_RE);
                        bool isString = true;
                        bool isFloat = false;
                        foreach (string v in values)
                        {
                            if (int.TryParse(v, out var nn))
                            {
                                isString = false;
                            }
                            else if (float.TryParse(v, out var ff))
                            {
                                isFloat = true;
                                isString = false;
                                break;
                            }
                        }

                        object objectValue = null;
                        if (isString)
                        {
                            objectValue = values;
                        }
                        else if (isFloat)
                        {
                            float[] f = new float[values.Length];
                            for (var index = 0; index < values.Length; index++)
                                float.TryParse(values[index], out f[index]);
                            objectValue = f;
                        }
                        else
                        {
                            int[] n = new int[values.Length];
                            for (var index = 0; index < values.Length; index++)
                                int.TryParse(values[index], out n[index]);
                            objectValue = n;
                        }
                        entry[header[j]].Add(objectValue);
                    }
                    else
                    {
                        value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                        object finalvalue = value;
                        int n;
                        float f;
                        if (int.TryParse(value, out n))
                        {
                            finalvalue = n;
                        }
                        else if (float.TryParse(value, out f))
                        {
                            finalvalue = f;
                        }
                        entry[header[j]].Add(finalvalue);
                    }
                }
            }
            return entry;
        }

        private static Tuple<string[], int> FindHeader(TextAsset data, string headerName)
        {
            var lines = Regex.Split(data.text, LINE_SPLIT_RE);
            for (var i = 0; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "//") continue;
                if (values[0].ToLower() == headerName)
                {
                    return new(Regex.Split(lines[i], SPLIT_RE), i);
                }
            }

            return new(Array.Empty<string>(), -1);
        }
    }
}

#endif