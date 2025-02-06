using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Util
{
    static class CSVReaderExtension
    {
        public static T DynamicCast<T>(this object objectValue)
        {
            if (objectValue is T value)
                return value;
            return default;
        }

        public static T DynamicCast<T>(this Dictionary<string,object> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out var objectValue))
            {
                if (objectValue is T value)
                    return value;
            }

            if (typeof(T).IsArray)
            {
                var empty = Array.CreateInstance(typeof(T).GetElementType(), 0);
                if(empty is T value)
                    return value;
            }
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
            return default;
        }
        
        public static List<Dictionary<string,object>> GetCSV(this TextAsset csvFile)
        {
            Debug.Assert(csvFile != null, "CSV파일이 없어서 퀘스트 데이터를 셋팅하지 못했습니다.");
            var path = AssetDatabase.GetAssetPath(csvFile);
            
            // Resource 포함 이전 경로 제거
            string[] parts = path.Split('/');
            int index = Array.IndexOf(parts, "Resources");
            if (index != -1 && index + 1 < parts.Length)
                path = string.Join("/", parts.Skip(index + 1));
            
            // 확장자 제거
            path = path.Replace(".csv", "");
            var csv = CSVReader.Read(path);
            return csv;
        }
    }
    
    public class CSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };
        private static readonly char[] ListTrim_Char = { '[', ']' };
        
        public static T DynamicCast<T>(object objectValue) where T : new()
        {
            if (objectValue is T value)
                return value;
            return new T();
        }
        
        public static List<Dictionary<string, object>> Read(string file)
        {
            var list = new List<Dictionary<string, object>>();
            TextAsset data = Resources.Load<TextAsset>(file);
            if (data == null) return new();

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            // 나중에 값이 안들어 있는 경우에 빈 값을 넣어주도록 바꾸어야한다.
            var header = FindHeader(data, "header");
            for (var i = 0; i < lines.Length - 1; i++)
            {
                var lineValues = Regex.Split(lines[i], SPLIT_RE);
                if (lineValues.Length == 0 || lineValues[0] == "//" || lineValues[0].ToLower() == "header") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < lineValues.Length; j++)
                {
                    string value = lineValues[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    if (value.StartsWith(ListTrim_Char[0]) && value.EndsWith(ListTrim_Char[1]))
                    {
                        value = value.TrimStart(ListTrim_Char).TrimEnd(ListTrim_Char).Replace("\\", "");
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

        private static string[] FindHeader(TextAsset data, string headerName)
        {
            var lines = Regex.Split(data.text, LINE_SPLIT_RE);
            for (var i = 0; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "//") continue;
                if (values[0].ToLower() == headerName)
                {
                    return Regex.Split(lines[i], SPLIT_RE);
                }
            }

            return Array.Empty<string>();
        }
    }
}
