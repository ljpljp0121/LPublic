using UnityEngine;
using UnityEngine.UI;

/*
 * The UISlider is a simple UI element that allows the user to change the timeScale of a LocalTimeScale. In the
 * demo scene we have one for "Global" and "Red", "Green", and "Blue" LocalTimeScales.
 */

namespace MagicPigGames.MagicTime.Demo
{
    public class UISlider : MonoBehaviour
    {
        [Header("Settings")]
        public LocalTimeScale timeScale;
        public bool automatedChanges = true;
        public float minTimeScale = 0f;
        public float maxTimeScale = 3f;
        public float changeSpeed = 1f; // How long it takes for the change to happen
        public float changeFrequency = 5f; // How often the change happens

        [Header("Plumbing")] 
        public Image image;
        public Sprite playSprite;
        public Sprite pauseSprite;
        public Slider slider;
        
        private float _targetTimeScale;
        private float _timeSinceLastChange;

        private LocalTimeScale LocalTimeScale => MagicTimeManager.Instance.TimeScale(timeScale.name);
        
        private void Start()
        {
            _targetTimeScale = 1f;
            _timeSinceLastChange = changeFrequency;
        }

        private void Update()
        {
            if (!automatedChanges)
                return;
            
            _timeSinceLastChange += Time.deltaTime;
            if (_timeSinceLastChange >= changeFrequency)
            {
                _timeSinceLastChange = 0f;
                _targetTimeScale = Random.Range(minTimeScale, maxTimeScale);
            }

            // We are using Time.deltaTime here because this is external to the MagicTime system, i.e. we always want
            // this to take the same amount of time regardless of the timeScale being used.
            var newValue = Mathf.Lerp(LocalTimeScale.Value, _targetTimeScale, Time.deltaTime * changeSpeed);
            slider.value = newValue; // change the value on the slider so that UI changes, and it will trigger the rest.
        }

        public void UpdateValue(float newValue) => LocalTimeScale.SetValue(newValue);

        public void SetAutomatedChanges(bool value)
        {
            automatedChanges = value;
            image.sprite = automatedChanges ? pauseSprite : playSprite;
        }

        public void ToggleAutomatedChanges() => SetAutomatedChanges(!automatedChanges);
    }

}
