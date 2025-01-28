using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Towers
{
    public class ImpactEffect : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> renderers = new();

        [SerializeField] private float delayTime = 0.5f;

        [SerializeField] private float fadeTime = 0.5f;

        public void FadeProjectile()
        {
            StartCoroutine(FadeAndDestroy());
        }

        protected virtual IEnumerator FadeAndDestroy()
        {
            yield return new WaitForSeconds(delayTime);

            float time = 0;
            float alpha = 1;

            while (time < fadeTime)
            {   
                alpha = Mathf.Lerp(1, 0, time / fadeTime);

                foreach (SpriteRenderer spriteRenderer in renderers)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
                }

                yield return new WaitForFixedUpdate();
            }

            print("destroy");
            Destroy(gameObject);
        }
    }
}