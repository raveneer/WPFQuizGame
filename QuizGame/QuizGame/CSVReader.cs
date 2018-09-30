using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace QuizGame
{
    public class CSVReader
    {
        private static readonly string _SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        private static readonly string _LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        private static readonly char[] _TRIM_CHARS = { '\"' };

        public string ReadText(string fileName)
        {
            var filePath = $"{fileName}.csv";
            return File.ReadAllText(filePath);
        }

        public Dictionary<string, Dictionary<string, object>> ReadToDic(string file, bool keyAsRowercase = false)
        {
            var dic = new Dictionary<string, Dictionary<string, object>>();
            //validation
            if (ReadText(file) == null)
            {
                throw new Exception("그런 CSV파일은 없습니다." + file);
            }

            var lines = Regex.Split(ReadText(file), _LINE_SPLIT_RE);

            if (lines.Length <= 1) return dic;

            var header = Regex.Split(lines[0], _SPLIT_RE);
            for (var i = 1; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], _SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    var value = values[j];
                    value = value.TrimStart(_TRIM_CHARS).TrimEnd(_TRIM_CHARS).Replace("\\", "");
                    object finalvalue = value;
                    int n;
                    float f;
                    if (int.TryParse(value, out n))
                        finalvalue = n;
                    else if (float.TryParse(value, out f))
                        finalvalue = f;
                    entry[header[j]] = finalvalue;
                }

                if (dic.ContainsKey(entry["title"].ToString().ToLower()))
                {
                    throw new Exception($"same key in file. key : {entry["title"].ToString()} in {file}");
                }

                if (keyAsRowercase)
                {
                    dic.Add(entry["title"].ToString().ToLower(), entry);
                }
                else
                {
                    dic.Add(entry["title"].ToString(), entry);
                }
            }
            return dic;
        }
    }
}