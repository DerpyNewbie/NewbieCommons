using System;
using UnityEngine;

namespace DerpyNewbie.Common
{
    public static class CsvUtil
    {
        public static Vector3 ParseVector3(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Vector3.zero;

            var actualValue = value.Substring(2, value.Length - 4);
            Debug.Log(actualValue);
            var valueXYZ = actualValue.Split(',');

            float x, y, z;

            float.TryParse(valueXYZ[0], out x);
            float.TryParse(valueXYZ[1], out y);
            float.TryParse(valueXYZ[2], out z);

            return new Vector3(x, y, z);
        }

        public static Quaternion ParseQuaternion(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Quaternion.identity;

            var actualValue = value.Substring(2, value.Length - 4);
            Debug.Log(actualValue);
            var valueXYZW = actualValue.Split(',');

            float x, y, z, w;

            float.TryParse(valueXYZW[0], out x);
            float.TryParse(valueXYZW[1], out y);
            float.TryParse(valueXYZW[2], out z);
            float.TryParse(valueXYZW[3], out w);

            return new Quaternion(x, y, z, w);
        }

        public static float ParseFloat(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return float.NaN;
            float v;
            float.TryParse(value, out v);
            return v;
        }

        public static DateTime ParseDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.MinValue;
            var v = ParseLong(value);
            return v < DateTime.MinValue.Ticks ? DateTime.MinValue :
                v > DateTime.MaxValue.Ticks ? DateTime.MaxValue : new DateTime(v);
        }

        public static long ParseLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return -1L;
            long v;
            long.TryParse(value, out v);
            return v;
        }

        public static string ToCsvString(Vector3 value)
        {
            return $"\"{value.ToString("F3")}\"";
        }

        public static string ToCsvString(Quaternion value)
        {
            return $"\"{value.ToString("F3")}\"";
        }

        public static string[] ParseCsvStringToRecords(string data)
        {
            return data.Split('\n');
        }

        public static string[] ParseCsvRecordToData(string record, int tupleValueLength)
        {
            string[] result = new string[tupleValueLength];

            bool inDoubleQuote = false;
            int currentDataIndex = 0;
            int currentDataBeginIndex = 0;

            for (int i = 0; i < record.Length; i++)
            {
                char c = record[i];
                if (c.Equals('"'))
                {
                    inDoubleQuote = !inDoubleQuote;
                }

                if ((c.Equals(',') || c.Equals('\n') || i == record.Length - 1) && inDoubleQuote == false)
                {
                    int dataLength = i - currentDataBeginIndex;
                    if (i == record.Length - 1)
                        dataLength++;
                    result[currentDataIndex] = record.Substring(currentDataBeginIndex, dataLength);

                    currentDataIndex++;
                    currentDataBeginIndex = i + 1;
                }
            }

            return result;
        }
    }
}