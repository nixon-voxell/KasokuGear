using System.Collections;
using UnityEngine;

namespace GameWorld
{
    using Cinemachine;
    using UX;

    public class Player : MonoBehaviour
    {
        public Transform CameraTransform;
        public CinemachineVirtualCamera CameraVCam;

        [SerializeField] private UpgradeDrop_SO m_UpgradeDropSO;

        [HideInInspector] public PlayerMovement PlayerMovement;
        [HideInInspector] public PlayerAttack PlayerAttack;
        [HideInInspector] public PlayerAttribute PlayerAttribute;
        [HideInInspector] public PlayerEffectsControl PlayerEffectsControl;
        [HideInInspector] public FirstPersonCamera FirstPersonCamera;

        private IEnumerator Start()
        {
            yield return null;
            GameSetup();
        }

        public void GameSetup()
        {
            // TODO: To be called by level manager
            InGameHUD inGameHUD = UXManager.Instance.InGameHUD;
            inGameHUD.SetEnable(true);
            inGameHUD.UpdateCurrentHP(PlayerAttribute.PlayerCurrentHP);
            inGameHUD.UpdateGunAmmo(PlayerAttribute.GunMagazine, PlayerAttribute.GunMagazine);
            inGameHUD.UpdateMaxHP(PlayerAttribute.ArmorMaxHealth);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            PlayerAttack.Initialize();
        }

        public void PlayerDie()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UserInput.Instance.Active = false;

            this.StartCoroutine(this.AdjustTimeScale(0.0f));

            GameOver gameOver = UXManager.Instance.GameOver;
            InGameHUD inGameHUD = UXManager.Instance.InGameHUD;

            inGameHUD.SetEnable(false);
            gameOver.SetEnable(true);

            gameOver.SetWaveCount(LevelManager.Instance.WaveCount);
            gameOver.SetContinueAction(() => { Time.timeScale = 1.0f; });
        }
        
        /// <summary>
        /// Called when boss upgrade orb is taken
        /// 
        /// 
        /// </summary>
        public void TakeUpgradeDrop(Enemy.EnemyType enemyType)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UserInput.Instance.Active = false;
            GameManager.Instance.SoundManager.PlayOneShot("sfx_card_appear", transform);

            StartCoroutine(AdjustTimeScale(0f));
            Upgrade[] upgradeDrops = m_UpgradeDropSO.RollUpgradeList(3, enemyType);

            UXManager.Instance.BuffSelection.DisplayCard(upgradeDrops, SelectUpgradeDrop);
        }

        /// <summary>
        /// Used to adjust time scale on the next frame
        /// 
        /// Due to visual issues when immediately setting timeframe to 0
        /// and stuff are not updated
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator AdjustTimeScale(float time)
        {
            yield return null;
            Time.timeScale = time;
        }

        /// <summary>
        /// Called by BuffSelection when a drop card is selected
        /// 
        /// Function is passed to BuffSelection to be called
        /// </summary>
        /// <param name="upgrade"></param>
        public void SelectUpgradeDrop(Upgrade upgrade)
        {
            PlayerAttribute.AddAttribute(upgrade);
            GameManager.Instance.SoundManager.PlayOneShot("sfx_card_grab", transform);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            UserInput.Instance.Active = true;
            Time.timeScale = 1.0f;
        }
    } 
}
