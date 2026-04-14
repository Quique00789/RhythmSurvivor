using UnityEngine;
using System.Linq;

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

        [Header("Rhythm")]
        public float bpm = 270f;
        public float beatWindow = 0.07f;

        float beatInterval;
        float lastBeatTime;

        bool dashing = false;

        void Start()
        {
            character = GetComponent<Character>();
            rb = character.RB;

            beatInterval = 60f / bpm;
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

            bool perfectTiming = CheckBeat();

            Monster target = GetClosestEnemy();

            if (target == null) return;

            Vector2 dir = (target.transform.position - transform.position).normalized;

            if (perfectTiming)
                StartCoroutine(PerfectDash(dir, target));
            else
                StartCoroutine(BadDash(dir));
        }

        bool CheckBeat()
        {
            float songTime = Time.time;

            float mod = songTime % beatInterval;

            return mod < beatWindow || mod > beatInterval - beatWindow;
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

        System.Collections.IEnumerator PerfectDash(Vector2 dir, Monster target)
        {
            dashing = true;

            character.IsInvulnerable = true;

            float time = dashDistance / dashSpeed;
            float t = 0;

            while (t < time)
            {
                rb.linearVelocity = dir * dashSpeed;
                t += Time.deltaTime;
                yield return null;
            }

            if (target != null)
                target.TakeDamage(dashDamage, dir * 3f);

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