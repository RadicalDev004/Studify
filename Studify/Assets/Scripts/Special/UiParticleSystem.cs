using Pixelplacement;
using System.Collections;
using UnityEngine;

namespace RadicalKit
{
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/Effects/UiParticleSystem", 20)]

    public class UiParticleSystem : MonoBehaviour
    {
        public int ParticeCount;
        public float DurationBetweenSpawn;
        public float AnimationDuration;

        public AnimationType Type;

        public GameObject Origin;
        public RectTransform EndPoint;

        public enum AnimationType { Direct, EaseIn, EaseOut }

        public void Play()
        {
            switch (Type)
            {
                case AnimationType.Direct:
                    StartCoroutine(InternalPlay(Tween.EaseLinear));
                    break;

                case AnimationType.EaseIn:
                    StartCoroutine(InternalPlay(Tween.EaseInStrong));
                    break;

                case AnimationType.EaseOut:
                    StartCoroutine(InternalPlay(Tween.EaseOutStrong));
                    break;
            }
        }

        private IEnumerator InternalPlay(AnimationCurve type)
        {
            float CountLeft =  ParticeCount;

            while (CountLeft > 0)
            {
                StartCoroutine(Spawn(type, AnimationDuration));
                CountLeft --;
                yield return new WaitForSecondsRealtime(DurationBetweenSpawn);
            }
        }

        private IEnumerator Spawn(AnimationCurve type, float TimeBeforeDestroy)
        {
            GameObject obj = Instantiate(Origin, FindObjectOfType<Canvas>().transform);
            Tween.AnchoredPosition(obj.GetComponent<RectTransform>(), EndPoint.anchoredPosition, TimeBeforeDestroy, 0, type);

            yield return new WaitForSecondsRealtime(TimeBeforeDestroy);
            DestroyParticle(obj);
        }

        private void DestroyParticle(GameObject Particle) => Destroy(Particle);
    }
}

