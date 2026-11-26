using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>Shows motion controls only while the alien ImageTarget is tracked.</summary>
public sealed class AlienMotionControls : MonoBehaviour
{
    private SpaceBuddyAnimator animator;
    private Transform canvasTransform;
    private ObserverBehaviour observer;
    private GameObject panel;

    public void Initialize(SpaceBuddyAnimator alienAnimator, Transform parentCanvas)
    {
        animator = alienAnimator;
        canvasTransform = parentCanvas;
    }

    private void Start()
    {
        if (canvasTransform == null)
        {
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null) canvasTransform = canvas.transform;
        }

        if (canvasTransform != null && animator != null)
            CreatePanel();

        observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
            SetTracked(IsTracked(observer.TargetStatus.Status));
        }
    }

    private void OnDestroy()
    {
        if (observer != null)
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        SetTracked(IsTracked(targetStatus.Status));
    }

    private static bool IsTracked(Status status)
    {
        return status == Status.TRACKED || status == Status.EXTENDED_TRACKED;
    }

    private void SetTracked(bool tracked)
    {
        if (panel != null) panel.SetActive(tracked);

        var previous = GameObject.Find("Previo");
        var next = GameObject.Find("Siguiente");
        if (previous != null) previous.SetActive(!tracked);
        if (next != null) next.SetActive(!tracked);

        if (!tracked && animator != null)
            animator.PlayIdle();
    }

    private void CreatePanel()
    {
        panel = new GameObject("ControlesAstroAmigo", typeof(RectTransform));
        panel.transform.SetParent(canvasTransform, false);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0.9f);
        panelRect.sizeDelta = new Vector2(560f, 82f);

        CreateButton("Caminar", -185f, animator.PlayWalk);
        CreateButton("Saltar", 0f, animator.PlayJump);
        CreateButton("Aplaudir", 185f, animator.PlayClap);
        panel.SetActive(false);
    }

    private void CreateButton(string label, float xOffset, UnityEngine.Events.UnityAction action)
    {
        var buttonObject = new GameObject("Alien_" + label, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(Button));
        buttonObject.transform.SetParent(panel.transform, false);
        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(xOffset, 0f);
        rect.sizeDelta = new Vector2(172f, 68f);

        var image = buttonObject.GetComponent<UnityEngine.UI.Image>();
        image.color = new Color(0.08f, 0.32f, 0.58f, 0.96f);
        var button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(action);

        var textObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(buttonObject.transform, false);
        var textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;

        var text = textObject.GetComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 20;
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;
    }
}

