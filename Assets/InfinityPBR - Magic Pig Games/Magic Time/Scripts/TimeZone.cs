using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MagicPigGames.MagicTime
{
    public class TimeZone : MonoBehaviour
    {
        [Tooltip("The LocalTimeScale applied to objects in this zone. If none is assigned, a new one will be created.")]
        public LocalTimeScale timeScaleToUse;

        [Tooltip("If a new LocalTimeScale is created, this value will be used as its initial value.")]
        [SerializeField]
        protected float timeScaleValue = 1f;
        
        protected LocalTimeScale timeScale;
        public LocalTimeScale TimeScale => timeScale;
        public float TimeScaleValue => timeScaleValue;
        
        [HideInInspector] public bool createLocalTime = true;
        
        public void SetTimeScaleStartingValue(float value) => timeScaleValue = value;

        protected virtual void Awake()
        {
            // Ensure the Collider is set as a trigger
            var col = GetComponent<Collider>();
            if (col != null && !col.isTrigger)
            {
                col.isTrigger = true;
                Debug.LogWarning($"{name}'s Collider was not set as a trigger. It has been set to trigger mode.");
            }

            // If we are creating a new LocalTimeScale, do so now
            if (createLocalTime || timeScaleToUse == null)
            {
                CreateLocalTimeScale();
                return;
            }
            
            // Otherwise create a new instance of the assigned timeScaleToUse Scriptable Object
            timeScale = Instantiate(timeScaleToUse);
            timeScale.Initialize($"{name}_{timeScaleToUse.name}_TimeZoneTimeScale", timeScaleToUse.Value);
        }

        private void CreateLocalTimeScale()
        {
            timeScale = ScriptableObject.CreateInstance<LocalTimeScale>();
            timeScale.Initialize($"{name}_TimeZoneTimeScale", timeScaleValue);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger) return;
            
            var localTimeUser = other.GetComponentInParent<IHaveLocalTime>();
            if (localTimeUser == null) return;
            
            OnEnter(localTimeUser);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.isTrigger) return;
            
            var localTimeUser = other.GetComponentInParent<IHaveLocalTime>();
            if (localTimeUser == null) return;
            
            OnExit(localTimeUser);
        }

        // For 2D physics
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger) return;
            
            var localTimeUser = other.GetComponentInParent<IHaveLocalTime>();
            if (localTimeUser == null) return;
            
            OnEnter(localTimeUser);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.isTrigger) return;
            
            var localTimeUser = other.GetComponentInParent<IHaveLocalTime>();
            if (localTimeUser == null) return;
            
            OnExit(localTimeUser);
        }

        /// <summary>
        /// Called when an IHaveLocalTime object enters the zone.
        /// </summary>
        /// <param name="localTimeUser">The object that entered the zone.</param>
        protected virtual void OnEnter(IHaveLocalTime localTimeUser) => localTimeUser.SubscribeToLocalTimeScale(timeScale);

        /// <summary>
        /// Called when an IHaveLocalTime object exits the zone.
        /// </summary>
        /// <param name="localTimeUser">The object that exited the zone.</param>
        protected virtual void OnExit(IHaveLocalTime localTimeUser) => localTimeUser.UnsubscribeFromLocalTimeScale(timeScale);

        protected virtual void OnDestroy()
        {
            if (timeScale == null) return;
         
            // Dispose of the timeScale if it was created by this zone
            timeScale.Dispose();
            timeScale = null;
        }

        protected virtual void OnDisable()
        {
            if (timeScale == null) return;

            TimeScale.UnsubscribeAll();
        }
    }
}