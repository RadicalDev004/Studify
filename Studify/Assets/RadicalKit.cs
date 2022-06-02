using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RadicalKit
{
    public class Change
    {
        //Ttransform
        public static Vector3 X(Vector3 yourTransform, float newX)
        {
            return new Vector3(newX, yourTransform.y, yourTransform.z);
        }
        public static Vector3 Y(Vector3 yourTransform, float newY)
        {
            return new Vector3(yourTransform.x, newY, yourTransform.z);
        }
        public static Vector3 Z(Vector3 yourTransform, float newZ)
        {
            return new Vector3(yourTransform.x, yourTransform.y, newZ);
        }


        //Color
        public static Color32 ColorR(Color32 toChange, byte Value)
        {
            Color32 newv = new Color32((byte)Value, (byte)toChange.g, (byte)toChange.b, (byte)toChange.a);
            return newv;
        }
        public static Color32 ColorG(Color32 toChange, byte Value)
        {
            Color32 newv = new Color32((byte)toChange.r, (byte)Value, (byte)toChange.b, (byte)toChange.a);
            return newv;
        }
        public static Color32 ColorB(Color32 toChange, byte Value)
        {
            Color32 newv = new Color32((byte)toChange.r, (byte)toChange.g, (byte)Value, (byte)toChange.a);
            return newv;
        }
        public static Color32 ColorA(Color32 toChange, byte Value)
        {
            Color32 newv = new Color32((byte)toChange.r, (byte)toChange.g, (byte)toChange.b, (byte)Value);
            return newv;
        }
    }
    public static class RandomChance
    {
        public static bool Percent(int percent)
        {
            int a = Random.Range(1, 101);

            if (a <= percent)
                return true;
            else
                return false;
        }

    }
    public class Prefs
    {
        public static void IncreaseInt(string key, int valueToAdd)
        {
            PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + valueToAdd);
        }

        public static void IncreaseFloat(string key, float valueToAdd)
        {
            PlayerPrefs.SetFloat(key, PlayerPrefs.GetFloat(key) + valueToAdd);
        }

        public static void IncreaseString(string key, string valueToAdd)
        {
            PlayerPrefs.SetString(key, PlayerPrefs.GetString(key) + valueToAdd);
        }



        public static void DecreaseInt(string key, int valueToSubtract)
        {
            PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) - valueToSubtract);
        }

        public static void DecreaseFloat(string key, float valueToSubtract)
        {
            PlayerPrefs.SetFloat(key, PlayerPrefs.GetFloat(key) - valueToSubtract);
        }

        public static void DecreaseString(string key, string valueToSubtract)
        {
            PlayerPrefs.SetString(key, PlayerPrefs.GetString(key).Replace(valueToSubtract, ""));
        }
    }
}
