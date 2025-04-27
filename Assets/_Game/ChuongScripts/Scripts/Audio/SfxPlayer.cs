namespace ChuongCustom
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class SfxPlayer : MonoBehaviour
    {
        [SerializeField] private string _sfxName;
        [SerializeField] private float _delay = 0f;
        [SerializeField] private bool _play3DSound;

        [SerializeField] [ShowIf("_play3DSound", true, true)]
        private Transform _soundTransform;

        [Space] [SerializeField] private bool _multiplePlay = false;

        [ShowIf("_multiplePlay")] [SerializeField]
        private int _timePlay = 2;

        [ShowIf("_multiplePlay")] [SerializeField]
        private float _delayBetween = 0.1f;

        public void PlaySfx()
        {
            PlaySingleSound();
        }

        private void PlaySingleSound()
        {
        }
    }
}