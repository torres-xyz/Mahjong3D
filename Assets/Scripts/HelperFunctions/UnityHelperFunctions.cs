using System.Collections.Generic;
using UnityEngine;

namespace CustomHelperFunctions
{
    public static class UnityHelperFunctions
    {
        /// <summary>
        ///// Generates a 1x1 Transparent Texture from a Base64 String
        /// </summary>
        /// <returns></returns>
        public static Texture2D OneTransparentPixel()
        {
            byte[] b64_bytes = System.Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=");
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(b64_bytes);
            return tex;
        }

        /// <summary>
        /// Take any list of GameObjects and return it with Fischer-Yates shuffle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(List<T> list)
        {
            int i = 0;
            int t = list.Count;
            int r;
            T p;
            List<T> tempList = new();
            tempList.AddRange(list);
            while (i < t)
            {
                r = UnityEngine.Random.Range(i, tempList.Count);
                p = tempList[i];
                tempList[i] = tempList[r];
                tempList[r] = p;
                i++;
            }
            return tempList;
        }
    }
}