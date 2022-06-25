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

    class CopyPastableStuff : MonoBehaviour
    {
        /// <summary>
        /// Use it like this for a quick shake: ShakeCoroutine(new Vector3(.15f, .15f, .15f), .05f, .5f);
        /// </summary>
        /// <param name="magnitude"></param>
        /// <param name="duration"></param>
        /// <param name="wavelength"></param>
        /// <returns></returns>
        private System.Collections.IEnumerator ShakeCoroutine(Vector3 magnitude, float duration, float wavelength)
        {
            Vector3 startPos = transform.localPosition;
            float endTime = Time.time + duration;
            float currentX = 0;

            while (Time.time < endTime)
            {
                Vector3 shakeAmount = new Vector3(
                    Mathf.PerlinNoise(currentX, 0) - .5f,
                    Mathf.PerlinNoise(currentX, 7) - .5f,
                    Mathf.PerlinNoise(currentX, 19) - .5f
                );

                transform.localPosition = Vector3.Scale(magnitude, shakeAmount) + startPos;
                currentX += wavelength;
                yield return null;
            }

            transform.localPosition = startPos;
        }

        private System.Collections.IEnumerator SpinAndShrink()
        {
            float t = 0.0f;
            float startRotation = transform.eulerAngles.y;
            float endRotation = startRotation + 180.0f;

            bool isDisabling = true; //for use external to the Coroutine

            Vector3 initialScale = transform.localScale;
            float duration = 0.25f;
            while (t < duration)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.zero;
                //float progress = animationCurve.Evaluate(t / duration); //Works great with a animationCurve
                float progress = t / duration;

                transform.localScale = new Vector3(
                    Mathf.LerpUnclamped(initialScale.x, 0, progress),
                    Mathf.LerpUnclamped(initialScale.y, 0, progress),
                    Mathf.LerpUnclamped(initialScale.z, 0, progress));

                transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x,
                    Mathf.LerpUnclamped(startRotation, endRotation, progress) % 360.0f,
                    transform.eulerAngles.z);

                yield return null;
            }
            isDisabling = false;
            //Disable at the end of the anim
            gameObject.SetActive(false);
        }
    }
}