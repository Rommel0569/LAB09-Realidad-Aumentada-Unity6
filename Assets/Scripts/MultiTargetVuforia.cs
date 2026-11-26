using UnityEngine;
using UnityEngine.UI;

/// <summary>Controls planet navigation and the AR companion mode.</summary>
public sealed class MultiTargetVuforia : MonoBehaviour
{
    [SerializeField] private Transform modelRoot;
    [SerializeField] private int startingPlanetIndex = 2;

    private static readonly string[] PlanetOrder =
    {
        "Sun", "Mercury", "Earth", "Venus", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune"
    };

    private int currentIndex;
    private Button previousButton;
    private Button nextButton;

    private void Awake()
    {
        if (modelRoot == null) modelRoot = transform;
        currentIndex = Mathf.Clamp(startingPlanetIndex, 0, PlanetOrder.Length - 1);
        ShowCurrent();
    }

    private void Start()
    {
        previousButton = GameObject.Find("Previo")?.GetComponent<Button>();
        nextButton = GameObject.Find("Siguiente")?.GetComponent<Button>();
        if (previousButton != null) previousButton.onClick.AddListener(Previo);
        if (nextButton != null) nextButton.onClick.AddListener(Siguiente);

    }

    public void Siguiente() => ChangePlanet(1);
    public void Previo() => ChangePlanet(-1);

    public void ChangePlanet(int direction)
    {
        currentIndex = (currentIndex + direction % PlanetOrder.Length + PlanetOrder.Length) % PlanetOrder.Length;
        ShowCurrent();
    }

    private void ShowCurrent()
    {
        if (modelRoot == null) return;
        for (int i = 0; i < modelRoot.childCount; i++)
        {
            Transform child = modelRoot.GetChild(i);
            bool shouldShow = child.name == PlanetOrder[currentIndex];
            if (child.gameObject.activeSelf != shouldShow) child.gameObject.SetActive(shouldShow);
        }
    }
}
