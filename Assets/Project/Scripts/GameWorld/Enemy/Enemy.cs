using GameWorld.Util;
using System;
using UnityEngine;

namespace GameWorld
{
    using AI;

    [RequireComponent(typeof(BoidEntity))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        public enum EnemyType { NORMAL, ELITE, BOSS }

        [SerializeField] private EnemyType m_EnemyType;
        [SerializeField] private GameObject m_UpgradeOrb;

        [Header("Stats")]
        [SerializeField] private int m_HealthMax;
        [SerializeField] private int m_StartingSpeed;
        [SerializeField] private int m_StartingDamage;
        [SerializeField] private int m_StartingAtkCooldown;

        [Header("Debug")]
        [SerializeField] private bool m_EnemyUnableToDie;

        private BoidEntity m_BoidEntity;

        // STATS
        private int m_CurrentHealth;
        private int m_CurrentSpeed;
        private int m_CurrentDamage;
        private int m_CurrentAtkCooldown;

        /// <summary>
        /// TODO: SPAWN VIA SPAWNMANAGER
        /// </summary>
        private void Awake()
        {
            this.m_BoidEntity = this.GetComponent<BoidEntity>();
        }

        private void OnEnable()
        {
            // Set on enable for now. To allow enemy reset their current health due to boidmanager reusing killed enemies object
            InitializeEnemy(1.0f);
        }

        public void OnDamage(int damage)
        {
            m_CurrentHealth -= damage;

            // m_DamagePopupPool.GetNextObject().OnPopup(damage.ToString());

            if (m_CurrentHealth <= 0)
            {
                m_CurrentHealth = 0;
                if (!m_EnemyUnableToDie)
                {
                    this.OnDie();
                }
            }
        }

        private void OnDie()
        {
            if (this.m_UpgradeOrb != null)
            {
                Instantiate(m_UpgradeOrb, transform.position, Quaternion.identity);
            }

            LevelManager.Instance.DespawnEnemy(this.m_BoidEntity);
        }

        private void InitializeEnemy(float statsMultiplier)
        {

            // Set new stats based on time
            m_CurrentHealth = (int)Math.Round(m_HealthMax * statsMultiplier);
            m_CurrentSpeed = (int)Math.Round(m_StartingSpeed * statsMultiplier);
            m_CurrentDamage = (int)Math.Round(m_StartingDamage * statsMultiplier);
            m_CurrentAtkCooldown = (int)Math.Round(m_StartingAtkCooldown * statsMultiplier);
        }
    }
}
