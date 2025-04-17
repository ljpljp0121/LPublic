using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This will change the color of the zone based on the colors of the objects within it, as well as the lights and opacity.
 */

namespace MagicPigGames.MagicTime.Demo
{
    public class ZoneColorChanger : MonoBehaviour
    {
        [Header("Settings")] 
        public Color startColor = new Color(1f, 1f, 1f, 0.25f);
        public float maximumObjectsForEffects = 5;
        public float transitionDuration = 0.2f; // Transition duration exposed
        
        [Header("Opacity Settings")]
        public float minimumOpacity = 0.25f;
        public float maximumOpacity = 0.75f;
        public float emissionIntensityModifier = 0.5f;
        
        [Header("Light Settings")]
        public float minLightIntensity = 0.5f;  // Minimum light intensity
        public float maxLightIntensity = 2.0f;  // Maximum light intensity

        [Header("Plumbing")]
        public MeshRenderer meshRenderer;
        public TimeZone timeZone;
        public Light pointLight;

        private GameObject[] SubscriberGameObjects => timeZone.TimeScale.SubscriberGameObjects;

        private readonly Dictionary<GameObject, Color> _objectColors = new Dictionary<GameObject, Color>();
        private Coroutine _colorTransitionCoroutine;
        
        private float Opacity => Mathf.Lerp(minimumOpacity, maximumOpacity, Mathf.Clamp01(_objectColors.Count / maximumObjectsForEffects));
        private float LightIntensity => Mathf.Lerp(minLightIntensity, maxLightIntensity, Mathf.Clamp01(_objectColors.Count / maximumObjectsForEffects));

        private void UpdateColors()
        {
            if (_objectColors.Count == 0)
            {
                if (_colorTransitionCoroutine != null)
                {
                    StopCoroutine(_colorTransitionCoroutine);
                }
                
                // Sometimes the object is deactivated when play stops.
                if (!gameObject.activeSelf)
                    return;
                
                _colorTransitionCoroutine = StartCoroutine(TransitionColor(startColor));
                return;
            }

            var red = 0f;
            var green = 0f;
            var blue = 0f;

            foreach (var objectColor in _objectColors)
            {
                red += objectColor.Value.r;
                green += objectColor.Value.g;
                blue += objectColor.Value.b;
            }

            var count = _objectColors.Count;
            var averageColor = new Color(red / count, green / count, blue / count, Opacity); // Apply Opacity here

            if (_colorTransitionCoroutine != null)
                StopCoroutine(_colorTransitionCoroutine);

            if (gameObject.activeSelf == false)
                return;
            
            _colorTransitionCoroutine = StartCoroutine(TransitionColor(averageColor));
        }

        private IEnumerator TransitionColor(Color targetColor)
        {
            var currentColor = meshRenderer.material.color;
            var currentEmissionColor = meshRenderer.material.GetColor("_EmissionColor");
            var elapsedTime = 0f;
            var initialLightIntensity = pointLight.intensity;

            while (elapsedTime < transitionDuration)
            {
                // Lerp between the current and target colors for both base color and emission color
                var lerpedColor = Color.Lerp(currentColor, targetColor, elapsedTime / transitionDuration);
                var lerpedEmissionColor = Color.Lerp(currentEmissionColor, targetColor * emissionIntensityModifier, elapsedTime / transitionDuration);

                // Set the base color
                meshRenderer.material.color = lerpedColor;

                // Set the emission color using the "_EmissionColor" property, modified by emissionIntensityModifier
                meshRenderer.material.SetColor("_EmissionColor", lerpedEmissionColor);
                
                // Lerp the light intensity
                pointLight.color = lerpedColor;
                pointLight.intensity = Mathf.Lerp(initialLightIntensity, LightIntensity, elapsedTime / transitionDuration);

                // Update elapsed time
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure it ends exactly at the target color and the final light intensity
            meshRenderer.material.color = targetColor;
            meshRenderer.material.SetColor("_EmissionColor", targetColor * emissionIntensityModifier); 
            pointLight.intensity = LightIntensity;
        }

        private void AddColor(GameObject newObject)
        {
            if (newObject == null) return;

            if (_objectColors.TryGetValue(newObject, out var color))
                return;

            _objectColors.Add(newObject, newObject.GetComponent<MeshRenderer>().material.color);
            UpdateColors();
        }

        private void RemoveColor(GameObject newColor)
        {
            if (newColor == null) return;

            if (!_objectColors.Remove(newColor))
                return;

            UpdateColors();
        }

        private void Start()
        {
            if (timeZone == null) timeZone = GetComponent<TimeZone>();
            if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();

            if (meshRenderer == null)
            {
                Debug.LogError("No MeshRenderer found in the ZoneColorChanger. Disabling script.");
                enabled = false;
                return;
            }

            meshRenderer.material.color = startColor;
        }

        private void OnEnable()
        {
            StartCoroutine(SubscribeToTimeScale());
        }

        private void OnDisable()
        {
            timeZone.TimeScale.OnSubscribed -= OnSubscribed;
            timeZone.TimeScale.OnUnsubscribed -= OnUnsubscribed;
        }

        private IEnumerator SubscribeToTimeScale()
        {
            yield return new WaitUntil(() => timeZone.TimeScale != null);
            timeZone.TimeScale.OnSubscribed += OnSubscribed;
            timeZone.TimeScale.OnUnsubscribed += OnUnsubscribed;
        }

        private void OnSubscribed(object go)
        {
            var newObject = (IHaveLocalTime)go;
            if (newObject == null) return;

            var newGameObject = newObject.GameObject;
            if (newGameObject == null) return;

            var sphere = (Sphere)go;
            if (sphere != null)
                sphere.Enter();

            Debug.Log($"ENTER: {newGameObject.name} has entered zone {gameObject.name}");
            AddColor(newGameObject);
        }

        private void OnUnsubscribed(object go)
        {
            var newObject = (IHaveLocalTime)go;
            if (newObject == null) return;

            var newGameObject = newObject.GameObject;
            if (newGameObject == null) return;
            
            var sphere = (Sphere)go;
            if (sphere != null)
                sphere.Exit();

            Debug.Log($"EXIT: {newGameObject.name} has exited zone {gameObject.name}");
            RemoveColor(newGameObject);
        }

        private void OnValidate()
        {
            if (timeZone == null) timeZone = GetComponent<TimeZone>();
            if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
            if (pointLight == null) pointLight = GetComponentInChildren<Light>();
        }
    }
}