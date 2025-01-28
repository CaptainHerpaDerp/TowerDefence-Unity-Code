using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Core.Character;

namespace Towers
{
    /// <summary>
    /// A visual element created by a tower to display its attack
    /// </summary>
    public abstract class TowerUnit : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected SpriteRenderer spriteRenderer;

        protected string ATTACK_UP = "Attack_U", ATTACK_DOWN = "Attack_D", ATTACK_LEFT = "Attack_L", ATTACK_RIGHT = "Attack_R";

        protected SoundEffectManager soundEffectManager;

        // The attack time, used to determine how long after an attack animation starts should a projectile be fired and the remainder time, the remainding time before the unit should enter the idle state
        public float hitMarkTime, attackAnimationTime;

        private bool doingAttackAnimation;

        private const float fadeSpeed = 0.3f;

        private ViewDirection viewDirection;

        private Dictionary<ViewDirection, string> directionStatePairs, combatStatePairs;

        private void Start()
        {
            soundEffectManager = SoundEffectManager.Instance;

            directionStatePairs = new Dictionary<ViewDirection, string>
        {
            { ViewDirection.Up, "Idle_U" },
            { ViewDirection.Down, "Idle_D" },
            { ViewDirection.Left, "Idle_L" },
            { ViewDirection.Right, "Idle_R" }
        };

            combatStatePairs = new Dictionary<ViewDirection, string>
        {
            { ViewDirection.Up, ATTACK_UP },
            { ViewDirection.Down, ATTACK_DOWN },
            { ViewDirection.Left, ATTACK_LEFT },
            { ViewDirection.Right, ATTACK_RIGHT }
        };

            InitializeAnimationKeys();
        }

        public abstract void InitializeAnimationKeys();

        public void StopAttacking()
        {
            PlayIdle();
        }

        public void HideUnit()
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }

        public void ShowUnit()
        {
            StartCoroutine(FadeInRenderer());
        }

        // Increases the alpha of the spriteRenderer over time
        private IEnumerator FadeInRenderer()
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

            while (spriteRenderer.color.a < 1)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + fadeSpeed);
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }

        public virtual void PlayIdle()
        {
            SetAnimationState(directionStatePairs[viewDirection]);
        }

        public virtual void PlayIdle(ViewDirection direction)
        {
            viewDirection = direction;
            SetAnimationState(directionStatePairs[viewDirection]);
        }

        public virtual void PlayIdle(Transform target)
        {
            if (target == null)
            {
                PlayIdle();
                return;
            }

            Vector3 direction = (target.position - transform.position).normalized;

            // Determine the predominant direction and set the corresponding animation state
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                {
                    viewDirection = ViewDirection.Right;
                }
                else
                {
                    viewDirection = ViewDirection.Left;
                }
            }
            else
            {
                if (direction.y > 0)
                {
                    viewDirection = ViewDirection.Up;
                }
                else
                {
                    viewDirection = ViewDirection.Down;
                }
            }

            SetAnimationState(directionStatePairs[viewDirection]);
        }

        protected virtual IEnumerator DoIdlePostAttack(Transform target)
        {
            yield return new WaitForSeconds(attackAnimationTime);

            PlayIdle(target);

            yield return null;
        }

        public virtual void AttackTowards(Transform target)
        {
            if (animator == null)
            {
                Debug.LogError("Animator is not assigned!");
                return;
            }

            Vector3 direction = (target.position - transform.position).normalized;

            PlayAttackSound();

            // Determine the predominant direction and set the corresponding animation state
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                {
                    viewDirection = ViewDirection.Right;
                }
                else
                {
                    viewDirection = ViewDirection.Left;
                }
            }
            else
            {
                if (direction.y > 0)
                {
                    viewDirection = ViewDirection.Up;
                }
                else
                {
                    viewDirection = ViewDirection.Down;
                }
            }

            SetAnimationState(combatStatePairs[viewDirection]);

            StartCoroutine(DoIdlePostAttack(target));
        }

        protected virtual void PlayAttackSound()
        {

        }

        public virtual void SetAnimationState(string newState)
        {
            animator.Play(newState);
        }
    }
}

