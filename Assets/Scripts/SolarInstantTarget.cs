using UnityEngine;
using Vuforia;

/// <summary>Creates the solar system ImageTarget from the included marker at runtime.</summary>
public sealed class SolarInstantTarget : MonoBehaviour
{
    [SerializeField] private Texture2D image;
    [SerializeField] private Texture2D companionImage;
    [SerializeField] private Transform models;
    private bool created;

    private void Start()
    {
        if (VuforiaApplication.Instance != null)
            VuforiaApplication.Instance.OnVuforiaStarted += CreateTarget;
    }

    private void OnDestroy()
    {
        if (VuforiaApplication.Instance != null)
            VuforiaApplication.Instance.OnVuforiaStarted -= CreateTarget;
    }

    private void CreateTarget()
    {
        if (created || image == null || companionImage == null || models == null || VuforiaBehaviour.Instance == null) return;
        try
        {
            var solarTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(image, 0.3f, "SolarSystem");
            if (solarTarget == null)
            {
                Debug.LogError("Vuforia no pudo crear el ImageTarget SolarSystem.");
                return;
            }

            solarTarget.gameObject.name = "ImageTarget_SolarSystem";
            solarTarget.gameObject.AddComponent<DefaultObserverEventHandler>();
            models.SetParent(solarTarget.transform, false);
            models.localPosition = Vector3.zero;

            var alienTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(companionImage, 0.3f, "AlienMarker");
            if (alienTarget == null)
            {
                Debug.LogError("Vuforia no pudo crear el ImageTarget AlienMarker.");
                return;
            }

            alienTarget.gameObject.name = "ImageTarget_AlienMarker";
            alienTarget.gameObject.AddComponent<DefaultObserverEventHandler>();
            var alienPrefab = Resources.Load<GameObject>("AnimatedAlien");
            if (alienPrefab == null)
            {
                Debug.LogError("No se encontró Assets/Resources/AnimatedAlien.fbx para el segundo marcador.");
                return;
            }
            // The FBX is Z-up while an ImageTarget is Y-up in Unity. Keep the
            // axis correction on a parent so animation clips cannot overwrite it.
            var alienPivot = new GameObject("AstroAmigoPivot").transform;
            alienPivot.SetParent(alienTarget.transform, false);
            alienPivot.localPosition = new Vector3(0f, 0.08f, 0f);
            alienPivot.localRotation = Quaternion.Euler(90f, 0f, 0f);
            alienPivot.localScale = Vector3.one * 0.12f;

            var alien = Instantiate(alienPrefab, alienPivot, false);
            alien.name = "AstroAmigo";
            alien.transform.localPosition = Vector3.zero;
            alien.transform.localRotation = Quaternion.identity;
            alien.transform.localScale = Vector3.one;
            var alienAnimator = alien.AddComponent<SpaceBuddyAnimator>();

            var canvas = FindFirstObjectByType<Canvas>();
            var motionControls = alienTarget.gameObject.AddComponent<AlienMotionControls>();
            motionControls.Initialize(alienAnimator, canvas != null ? canvas.transform : null);

            created = true;
            Debug.Log("Targets AR creados: enfoca SolarSystem.jpg para ver los planetas y AlienMarker.png para ver AstroAmigo.");
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}

