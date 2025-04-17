using Cinemachine;
using UnityEngine;

/*
 * This script is used to control the noise of the Cinemachine Virtual Camera. It is used in the Magic Time demo scene.
 * When the Bump() method is calle,d we add a bit to the desired Frequency, up to a maximum value. The frequency falls
 * back to 0 over time.
 */

namespace MagicPigGames.MagicTime.Demo
{
    public class CameraControl : MonoBehaviour
    { 
        public static CameraControl Instance { get; private set; }

        [Header("Setting")] 
        public float maxEffect = 2f;
        public float fallRate = 1f;
        public float riseRate = 0.25f;
        public float startingFrequency = 1f;
        
        [Header("Plumbing")]
        public CinemachineVirtualCamera virtualCamera;
        
        private float _desiredFrequency;

        public void Bump() => _desiredFrequency = Mathf.Min(maxEffect, _desiredFrequency + riseRate);

        private void Update()
        {
            Fall();
            
            var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_FrequencyGain = _desiredFrequency;
        }

        private void Fall() => _desiredFrequency = Mathf.Max(0, _desiredFrequency - fallRate * Time.deltaTime);

        private void Start()
        {
            _desiredFrequency = startingFrequency;
        }

        private void OnEnable()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void OnValidate()
        {
            if (virtualCamera == null)
                virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }
    }

}
