using UnityEngine;

namespace Vampire
{
    public class RhythmDash : MonoBehaviour
    {
        Character character;
        Rigidbody2D rb;

        [Header("Dash")]
        public float dashSpeed = 20f;
        public float dashDistance = 5f;
        public float badDashDistance = 2f;
        public float dashDamage = 10f;
        public float invulnerabilityTime = 1.5f;

        bool dashing = false;

        void Start()
        {
            character = GetComponent<Character>();
            rb = character.RB;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K))
            {
                TryDash();
            }
        }

        void TryDash()
        {
            if (dashing) return;

            // 🎯 NUEVO SISTEMA DE RITMO (con notas)
            var result = RhythmNotes.Instance.CheckHit(true); // intenta nota grande

            // si no acertó grande, intenta pequeña
            if (result == RhythmNotes.HitResult.Miss)
            {
                result = RhythmNotes.Instance.CheckHit(false);
            }

            Monster target = GetClosestEnemy();
            if (target == null) return;

            Vector2 dir = (target.transform.position - transform.position).normalized;

            switch (result)
            {
                case RhythmNotes.HitResult.Perfect:
                    if (DashVisualFeedback.Instance != null)
                        DashVisualFeedback.Instance.TriggerEffect(true);
                    StartCoroutine(PerfectDash(dir, target, 1.2f)); // boost
                    break;

                case RhythmNotes.HitResult.Good:
                    if (DashVisualFeedback.Instance != null)
                        DashVisualFeedback.Instance.TriggerEffect(false);
                    StartCoroutine(PerfectDash(dir, target, 1f)); // normal
                    break;

                case RhythmNotes.HitResult.Miss:
                    StartCoroutine(BadDash(dir));
                    break;
            }
        }

        Monster GetClosestEnemy()
        {
            var monsters = character.EntityManager.LivingMonsters;

            if (monsters.Count == 0) return null;

            Monster closest = null;
            float minDist = float.MaxValue;

            foreach (var m in monsters)
            {
                float d = Vector2.Distance(transform.position, m.transform.position);

                if (d < minDist)
                {
                    minDist = d;
                    closest = m;
                }
            }

            return closest;
        }

        System.Collections.IEnumerator PerfectDash(Vector2 dir, Monster target, float multiplier)
        {
            dashing = true;

            character.IsInvulnerable = true;

            float time = dashDistance / dashSpeed;
            float t = 0;

            while (t < time)
            {
                rb.linearVelocity = dir * dashSpeed * multiplier;
                t += Time.deltaTime;
                yield return null;
            }

            if (target != null)
            {
                float finalDamage = dashDamage * multiplier;
                target.TakeDamage(finalDamage, dir * 3f);
            }

            rb.linearVelocity = Vector2.zero;

            yield return new WaitForSeconds(invulnerabilityTime);

            character.IsInvulnerable = false;

            dashing = false;
        }

        System.Collections.IEnumerator BadDash(Vector2 dir)
        {
            dashing = true;

            float time = badDashDistance / dashSpeed;
            float t = 0;

            while (t < time)
            {
                rb.linearVelocity = dir * dashSpeed * 0.6f;
                t += Time.deltaTime;
                yield return null;
            }

            rb.linearVelocity = Vector2.zero;

            dashing = false;
        }
    }
}