#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CodEvadareUiMenuVisuals
{
    private const string RuntimeUiRoot = "Assets/_Project/Art/Sprites/RuntimeTransparent/UI";
    private const string ThumbnailRoot = "Assets/_Project/Art/Thumbnails";
    private const string SceneRoot = "Assets/_Project/Scenes";
    private const string UiPrefabRoot = "Assets/_Project/Prefabs/UI";

    [MenuItem("Tools/Cod Evadare/Art/Phase 8/Apply UI Menu HUD Visuals")]
    public static void ApplyUiMenuHudVisuals()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("UI visual integration must be run from Edit Mode, not Play Mode.");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("UI visual integration cancelled because the current scene has unsaved changes.");
            return;
        }

        string activeScenePath = SceneManager.GetActiveScene().path;

        CodEvadareArtImportSettings.ApplyVisualImportSettings();
        ApplyUiRuntimeImportSettings();
        ApplyThumbnailImportSettings();

        SpriteLibrary sprites = SpriteLibrary.Load();
        int changedPrefabs = CreateOrUpdateUiPrefabs(sprites);
        int changedScenes = ApplySceneVisuals(sprites);

        RestoreActiveScene(activeScenePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Applied UI, menu, HUD, button, card and level thumbnail visuals. UI prefabs changed: {changedPrefabs}. Scenes changed: {changedScenes}.");
    }

    [MenuItem("Tools/Cod Evadare/Art/Repair Broken Visual Layouts")]
    public static void RepairBrokenVisualLayouts()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("Visual layout repair must be run from Edit Mode, not Play Mode.");
            return;
        }

        ApplyUiMenuHudVisuals();
        CodEvadarePickupPropHazardEnvironmentVisuals.ApplyPickupPropHazardEnvironmentVisuals();
    }

    private static int CreateOrUpdateUiPrefabs(SpriteLibrary sprites)
    {
        EnsureFolder(UiPrefabRoot);
        EnsureFolder($"{UiPrefabRoot}/HUD");
        EnsureFolder($"{UiPrefabRoot}/Menus");
        EnsureFolder($"{UiPrefabRoot}/Cards");

        int changedCount = 0;
        changedCount += CreateOrUpdateImagePrefab($"{UiPrefabRoot}/HUD/HUDPanelFrame.prefab", "HUDPanelFrame", sprites.AmmoPanel, new Vector2(360f, 112f)) ? 1 : 0;
        changedCount += CreateOrUpdateButtonPrefab($"{UiPrefabRoot}/Menus/MenuButton.prefab", sprites) ? 1 : 0;
        changedCount += CreateOrUpdateImagePrefab($"{UiPrefabRoot}/Cards/ShopItemCardFrame.prefab", "ShopItemCardFrame", sprites.ShopItemCard, new Vector2(320f, 190f)) ? 1 : 0;
        changedCount += CreateOrUpdateImagePrefab($"{UiPrefabRoot}/Cards/BuffChoiceCardFrame.prefab", "BuffChoiceCardFrame", sprites.BuffChoiceCard, new Vector2(340f, 210f)) ? 1 : 0;
        changedCount += CreateOrUpdateLevelCardPrefab($"{UiPrefabRoot}/Cards/LevelThumbnailCard.prefab", sprites) ? 1 : 0;
        return changedCount;
    }

    private static bool CreateOrUpdateImagePrefab(string prefabPath, string objectName, Sprite sprite, Vector2 size)
    {
        GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        bool created = root == null;
        GameObject instance = created ? new GameObject(objectName) : PrefabUtility.LoadPrefabContents(prefabPath);
        bool changed = false;

        try
        {
            RectTransform rectTransform = EnsureRectTransform(instance, ref changed);
            changed |= SetRectTransform(rectTransform, size);

            Image image = instance.GetComponent<Image>();

            if (image == null)
            {
                image = instance.AddComponent<Image>();
                changed = true;
            }

            changed |= AssignImageSprite(image, sprite, Image.Type.Sliced, true);
            image.raycastTarget = false;

            if (created || changed)
            {
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                return true;
            }
        }
        finally
        {
            if (created)
            {
                Object.DestroyImmediate(instance);
            }
            else
            {
                PrefabUtility.UnloadPrefabContents(instance);
            }
        }

        return false;
    }

    private static bool CreateOrUpdateButtonPrefab(string prefabPath, SpriteLibrary sprites)
    {
        GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        bool created = root == null;
        GameObject instance = created ? new GameObject("MenuButton") : PrefabUtility.LoadPrefabContents(prefabPath);
        bool changed = false;

        try
        {
            RectTransform rectTransform = EnsureRectTransform(instance, ref changed);
            changed |= SetRectTransform(rectTransform, new Vector2(280f, 76f));

            Image image = instance.GetComponent<Image>();

            if (image == null)
            {
                image = instance.AddComponent<Image>();
                changed = true;
            }

            changed |= AssignImageSprite(image, sprites.ButtonNormal, Image.Type.Sliced, true);

            Button button = instance.GetComponent<Button>();

            if (button == null)
            {
                button = instance.AddComponent<Button>();
                changed = true;
            }

            changed |= StyleButton(button, sprites, sprites.ButtonNormal);

            Text text = instance.GetComponentInChildren<Text>(true);

            if (text == null)
            {
                GameObject textObject = new GameObject("Text");
                textObject.transform.SetParent(instance.transform, false);
                RectTransform textTransform = textObject.AddComponent<RectTransform>();
                textTransform.anchorMin = Vector2.zero;
                textTransform.anchorMax = Vector2.one;
                textTransform.offsetMin = new Vector2(16f, 8f);
                textTransform.offsetMax = new Vector2(-16f, -8f);
                text = textObject.AddComponent<Text>();
                text.text = "Button";
                text.alignment = TextAnchor.MiddleCenter;
                text.fontSize = 22;
                text.raycastTarget = false;
                changed = true;
            }

            if (text.font == null)
            {
                text.font = GetBuiltinFont();
                changed = true;
            }

            if (text.color != Color.white)
            {
                text.color = Color.white;
                changed = true;
            }

            if (created || changed)
            {
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                return true;
            }
        }
        finally
        {
            if (created)
            {
                Object.DestroyImmediate(instance);
            }
            else
            {
                PrefabUtility.UnloadPrefabContents(instance);
            }
        }

        return false;
    }

    private static bool CreateOrUpdateLevelCardPrefab(string prefabPath, SpriteLibrary sprites)
    {
        GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        bool created = root == null;
        GameObject instance = created ? new GameObject("LevelThumbnailCard") : PrefabUtility.LoadPrefabContents(prefabPath);
        bool changed = false;

        try
        {
            RectTransform rectTransform = EnsureRectTransform(instance, ref changed);
            changed |= SetRectTransform(rectTransform, new Vector2(340f, 220f));

            Image image = instance.GetComponent<Image>();

            if (image == null)
            {
                image = instance.AddComponent<Image>();
                changed = true;
            }

            changed |= AssignImageSprite(image, sprites.UnlockedLevelCard, Image.Type.Sliced, true);
            changed |= EnsureChildImage(instance.transform, "ThumbnailPreview", sprites.LaboratoryThumbnail, new Vector2(0.5f, 0.5f), new Vector2(300f, 150f), new Vector2(0f, 20f), false);

            if (created || changed)
            {
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                return true;
            }
        }
        finally
        {
            if (created)
            {
                Object.DestroyImmediate(instance);
            }
            else
            {
                PrefabUtility.UnloadPrefabContents(instance);
            }
        }

        return false;
    }

    private static int ApplySceneVisuals(SpriteLibrary sprites)
    {
        int changedSceneCount = 0;

        foreach (string scenePath in FindScenePaths())
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            bool changed = false;

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Canvas[] canvases = root.GetComponentsInChildren<Canvas>(true);

                foreach (Canvas canvas in canvases)
                {
                    changed |= ApplyCanvasVisuals(canvas, sprites);
                }
            }

            if (changed)
            {
                EditorSceneManager.SaveScene(scene);
                changedSceneCount++;
            }
        }

        return changedSceneCount;
    }

    private static bool ApplyCanvasVisuals(Canvas canvas, SpriteLibrary sprites)
    {
        bool changed = false;

        foreach (Image image in canvas.GetComponentsInChildren<Image>(true))
        {
            changed |= StyleImage(image, sprites);
        }

        foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
        {
            Sprite buttonSprite = ChooseButtonSprite(button, sprites);
            changed |= StyleButton(button, sprites, buttonSprite);
        }

        foreach (Text text in canvas.GetComponentsInChildren<Text>(true))
        {
            changed |= StyleText(text);
        }

        changed |= RemoveGeneratedPanelIcons(canvas.transform);
        changed |= RemoveGeneratedLevelThumbnails(canvas.transform);
        changed |= RepairLevelSelectButtons(canvas, sprites);
        return changed;
    }

    private static bool StyleImage(Image image, SpriteLibrary sprites)
    {
        if (image == null)
        {
            return false;
        }

        string lowerName = image.name.ToLowerInvariant();
        string lowerParentName = image.transform.parent != null ? image.transform.parent.name.ToLowerInvariant() : string.Empty;
        bool isButton = image.GetComponent<Button>() != null || image.GetComponentInParent<Button>() != null;

        if (lowerName.Contains("thumbnailpreview") || lowerName.StartsWith("uiicon"))
        {
            return false;
        }

        if (ShouldClearBrokenGeneratedPanel(lowerName))
        {
            return ClearGeneratedPanelImage(image, GetFallbackPanelColor(lowerName));
        }

        if (lowerName.Contains("boss") && lowerName.Contains("fill"))
        {
            return AssignImageSprite(image, sprites.BossHealthFill, Image.Type.Filled, false);
        }

        if (lowerName.Contains("boss") && (lowerName.Contains("background") || lowerName.Contains("panel") || lowerName.Contains("bar")))
        {
            return AssignImageSprite(image, sprites.BossHealthFrame, Image.Type.Sliced, true);
        }

        if (lowerName.Contains("health") && lowerName.Contains("fill"))
        {
            return AssignImageSprite(image, sprites.HealthFill, Image.Type.Filled, false);
        }

        if (lowerName.Contains("health") && (lowerName.Contains("panel") || lowerName.Contains("background") || lowerParentName.Contains("healthpanel")))
        {
            return AssignImageSprite(image, sprites.HealthFrame, Image.Type.Sliced, true);
        }

        if (lowerName.Contains("buff") && (lowerName.Contains("choice") || lowerName.Contains("card")))
        {
            return AssignImageSprite(image, lowerName.Contains("status") ? sprites.BuffStatusPanel : sprites.BuffChoiceCard, Image.Type.Sliced, true);
        }

        if (lowerName.Contains("shop") && (lowerName.Contains("card") || lowerName.Contains("item")))
        {
            return AssignImageSprite(image, sprites.ShopItemCard, Image.Type.Sliced, true);
        }

        if (lowerName.Contains("levelbutton"))
        {
            return AssignImageSprite(image, sprites.ButtonNormal, Image.Type.Sliced, true);
        }

        if (isButton)
        {
            return AssignImageSprite(image, ChooseButtonSprite(image.GetComponentInParent<Button>(), sprites), Image.Type.Sliced, true);
        }

        return false;
    }

    private static bool StyleButton(Button button, SpriteLibrary sprites, Sprite normalSprite)
    {
        if (button == null)
        {
            return false;
        }

        bool changed = false;
        Image targetImage = button.targetGraphic as Image;

        if (targetImage == null)
        {
            targetImage = button.GetComponent<Image>();

            if (targetImage != null && button.targetGraphic != targetImage)
            {
                button.targetGraphic = targetImage;
                changed = true;
            }
        }

        if (targetImage != null)
        {
            changed |= AssignImageSprite(targetImage, normalSprite, Image.Type.Sliced, true);
        }

        if (button.transition != Selectable.Transition.SpriteSwap)
        {
            button.transition = Selectable.Transition.SpriteSwap;
            changed = true;
        }

        SpriteState spriteState = button.spriteState;

        if (spriteState.highlightedSprite != sprites.ButtonHover)
        {
            spriteState.highlightedSprite = sprites.ButtonHover;
            changed = true;
        }

        if (spriteState.pressedSprite != sprites.ButtonPressed)
        {
            spriteState.pressedSprite = sprites.ButtonPressed;
            changed = true;
        }

        if (spriteState.selectedSprite != sprites.ButtonHover)
        {
            spriteState.selectedSprite = sprites.ButtonHover;
            changed = true;
        }

        button.spriteState = spriteState;

        ColorBlock colors = button.colors;
        Color disabledColor = new Color(1f, 1f, 1f, 0.42f);

        if (colors.normalColor != Color.white || colors.highlightedColor != Color.white || colors.pressedColor != Color.white || colors.selectedColor != Color.white || colors.disabledColor != disabledColor)
        {
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = Color.white;
            colors.selectedColor = Color.white;
            colors.disabledColor = disabledColor;
            button.colors = colors;
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(button);
        }

        return changed;
    }

    private static bool StyleText(Text text)
    {
        if (text == null)
        {
            return false;
        }

        bool changed = false;

        if (text.font == null)
        {
            text.font = GetBuiltinFont();
            changed = true;
        }

        Color targetColor = ChooseTextColor(text);

        if (text.color != targetColor)
        {
            text.color = targetColor;
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(text);
        }

        return changed;
    }

    private static bool RemoveGeneratedPanelIcons(Transform root)
    {
        if (root == null)
        {
            return false;
        }

        bool changed = false;

        List<GameObject> generatedIcons = new List<GameObject>();
        Transform[] transforms = root.GetComponentsInChildren<Transform>(true);

        foreach (Transform transform in transforms)
        {
            if (transform != null && transform.name.StartsWith("UIIcon_"))
            {
                generatedIcons.Add(transform.gameObject);
            }
        }

        foreach (GameObject generatedIcon in generatedIcons)
        {
            Object.DestroyImmediate(generatedIcon);
            changed = true;
        }

        return changed;
    }

    private static bool RepairLevelSelectButtons(Canvas canvas, SpriteLibrary sprites)
    {
        bool changed = false;
        LevelSelectController2D[] controllers = canvas.GetComponentsInChildren<LevelSelectController2D>(true);

        foreach (LevelSelectController2D controller in controllers)
        {
            Button[] buttons = GetSerializedObjectReferenceArray<Button>(controller, "levelButtons");

            for (int i = 0; i < buttons.Length; i++)
            {
                Button button = buttons[i];

                if (button == null)
                {
                    continue;
                }

                changed |= StyleLevelSelectButton(button, i + 1, sprites);
            }
        }

        return changed;
    }

    private static bool StyleLevelSelectButton(Button button, int levelIndex, SpriteLibrary sprites)
    {
        bool changed = false;
        RectTransform rectTransform = button.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            Vector2 targetSize = new Vector2(360f, 58f);

            if (rectTransform.sizeDelta != targetSize)
            {
                rectTransform.sizeDelta = targetSize;
                changed = true;
            }
        }

        changed |= StyleButton(button, sprites, sprites.ButtonNormal);

        Text text = button.GetComponentInChildren<Text>(true);

        if (text != null)
        {
            RectTransform textTransform = text.GetComponent<RectTransform>();

            if (textTransform != null)
            {
                changed |= SetFullButtonText(textTransform);
            }
        }

        if (changed)
        {
            EditorUtility.SetDirty(button.gameObject);
        }

        return changed;
    }

    private static bool RemoveGeneratedLevelThumbnails(Transform root)
    {
        if (root == null)
        {
            return false;
        }

        bool changed = false;
        List<GameObject> generatedThumbnails = new List<GameObject>();
        Transform[] transforms = root.GetComponentsInChildren<Transform>(true);

        foreach (Transform transform in transforms)
        {
            if (transform != null && transform.name == "ThumbnailPreview")
            {
                generatedThumbnails.Add(transform.gameObject);
            }
        }

        foreach (GameObject generatedThumbnail in generatedThumbnails)
        {
            Object.DestroyImmediate(generatedThumbnail);
            changed = true;
        }

        return changed;
    }

    private static bool EnsureNamedPanelIcon(Transform root, string panelName, string iconName, Sprite sprite, Vector2 anchor, Vector2 size, Vector2 anchoredPosition)
    {
        bool changed = false;
        Transform[] transforms = root.GetComponentsInChildren<Transform>(true);

        foreach (Transform transform in transforms)
        {
            if (transform.name != panelName)
            {
                continue;
            }

            changed |= EnsureChildImage(transform, iconName, sprite, anchor, size, anchoredPosition, false);
        }

        return changed;
    }

    private static bool EnsureChildImage(Transform parent, string childName, Sprite sprite, Vector2 anchor, Vector2 size, Vector2 anchoredPosition, bool stretch)
    {
        if (parent == null || sprite == null)
        {
            return false;
        }

        bool changed = false;
        Transform child = parent.Find(childName);

        if (child == null)
        {
            GameObject childObject = new GameObject(childName);
            childObject.transform.SetParent(parent, false);
            child = childObject.transform;
            changed = true;
        }

        RectTransform rectTransform = child.GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            rectTransform = child.gameObject.AddComponent<RectTransform>();
            child = rectTransform.transform;
            changed = true;
        }

        if (stretch)
        {
            if (rectTransform.anchorMin != Vector2.zero || rectTransform.anchorMax != Vector2.one || rectTransform.offsetMin != Vector2.zero || rectTransform.offsetMax != Vector2.zero)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                changed = true;
            }
        }
        else
        {
            if (rectTransform.anchorMin != anchor || rectTransform.anchorMax != anchor || rectTransform.pivot != new Vector2(0.5f, 0.5f) || rectTransform.anchoredPosition != anchoredPosition || rectTransform.sizeDelta != size)
            {
                rectTransform.anchorMin = anchor;
                rectTransform.anchorMax = anchor;
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = size;
                changed = true;
            }
        }

        Image image = rectTransform.GetComponent<Image>();

        if (image == null)
        {
            image = rectTransform.gameObject.AddComponent<Image>();
            changed = true;
        }

        changed |= AssignImageSprite(image, sprite, Image.Type.Simple, true);

        if (image.raycastTarget)
        {
            image.raycastTarget = false;
            changed = true;
        }

        if (!image.preserveAspect)
        {
            image.preserveAspect = true;
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(rectTransform.gameObject);
        }

        return changed;
    }

    private static bool AssignImageSprite(Image image, Sprite sprite, Image.Type type, bool allowTypeChange)
    {
        if (image == null || sprite == null)
        {
            return false;
        }

        bool changed = false;

        if (image.sprite != sprite)
        {
            image.sprite = sprite;
            changed = true;
        }

        if (allowTypeChange && image.type != Image.Type.Filled && image.type != type)
        {
            image.type = type;
            changed = true;
        }

        if (!allowTypeChange && image.type != Image.Type.Filled)
        {
            image.type = type;
            changed = true;
        }

        if (image.color != Color.white)
        {
            image.color = Color.white;
            changed = true;
        }

        if (image.preserveAspect)
        {
            image.preserveAspect = false;
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(image);
        }

        return changed;
    }

    private static bool ShouldClearBrokenGeneratedPanel(string lowerName)
    {
        return lowerName.Contains("mainpanel") ||
               lowerName.Contains("levelselectpanel") ||
               lowerName.Contains("controlspanel") ||
               lowerName.Contains("resourcepanel") ||
               lowerName.Contains("objectivepanel") ||
               lowerName.Contains("pausepanel") ||
               lowerName.Contains("gameoverpanel") ||
               lowerName.Contains("victorypanel") ||
               lowerName.Contains("shoppanel") ||
               lowerName.Contains("backdrop");
    }

    private static Color GetFallbackPanelColor(string lowerName)
    {
        if (lowerName.Contains("mainpanel") || lowerName.Contains("levelselectpanel") || lowerName.Contains("controlspanel"))
        {
            return new Color(0f, 0f, 0f, 0f);
        }

        return new Color(0f, 0f, 0f, 0.72f);
    }

    private static bool ClearGeneratedPanelImage(Image image, Color color)
    {
        if (image == null)
        {
            return false;
        }

        bool changed = false;

        if (image.sprite != null)
        {
            image.sprite = null;
            changed = true;
        }

        if (image.type != Image.Type.Simple)
        {
            image.type = Image.Type.Simple;
            changed = true;
        }

        if (image.color != color)
        {
            image.color = color;
            changed = true;
        }

        if (image.preserveAspect)
        {
            image.preserveAspect = false;
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(image);
        }

        return changed;
    }

    private static Sprite ChooseButtonSprite(Button button, SpriteLibrary sprites)
    {
        if (button == null)
        {
            return sprites.ButtonNormal;
        }

        string lowerName = button.name.ToLowerInvariant();

        if (lowerName.Contains("quit") || lowerName.Contains("delete") || lowerName.Contains("danger"))
        {
            return sprites.ButtonDanger;
        }

        if (lowerName.Contains("new") || lowerName.Contains("continue") || lowerName.Contains("play") || lowerName.Contains("next") || lowerName.Contains("resume") || lowerName.Contains("confirm"))
        {
            return sprites.ButtonConfirm;
        }

        return sprites.ButtonNormal;
    }

    private static Color ChooseTextColor(Text text)
    {
        string lowerName = text.name.ToLowerInvariant();
        string lowerText = text.text != null ? text.text.ToLowerInvariant() : string.Empty;

        if (lowerName.Contains("title") || lowerText.Contains("cod: evadare"))
        {
            return new Color(0.62f, 0.95f, 1f, 1f);
        }

        if (lowerText.Contains("game over") || lowerName.Contains("gameover"))
        {
            return new Color(1f, 0.35f, 0.36f, 1f);
        }

        if (lowerText.Contains("cleared") || lowerText.Contains("victory") || lowerName.Contains("victory"))
        {
            return new Color(0.55f, 1f, 0.7f, 1f);
        }

        if (lowerName.Contains("hint") || lowerName.Contains("objective"))
        {
            return new Color(0.78f, 0.96f, 1f, 1f);
        }

        return Color.white;
    }

    private static bool SetAnchoredTextBand(RectTransform textTransform)
    {
        bool changed = false;
        Vector2 anchorMin = new Vector2(0f, 0f);
        Vector2 anchorMax = new Vector2(1f, 0f);
        Vector2 offsetMin = new Vector2(14f, 10f);
        Vector2 offsetMax = new Vector2(-14f, 54f);

        if (textTransform.anchorMin != anchorMin)
        {
            textTransform.anchorMin = anchorMin;
            changed = true;
        }

        if (textTransform.anchorMax != anchorMax)
        {
            textTransform.anchorMax = anchorMax;
            changed = true;
        }

        if (textTransform.offsetMin != offsetMin)
        {
            textTransform.offsetMin = offsetMin;
            changed = true;
        }

        if (textTransform.offsetMax != offsetMax)
        {
            textTransform.offsetMax = offsetMax;
            changed = true;
        }

        return changed;
    }

    private static bool SetFullButtonText(RectTransform textTransform)
    {
        bool changed = false;

        if (textTransform.anchorMin != Vector2.zero)
        {
            textTransform.anchorMin = Vector2.zero;
            changed = true;
        }

        if (textTransform.anchorMax != Vector2.one)
        {
            textTransform.anchorMax = Vector2.one;
            changed = true;
        }

        Vector2 offsetMin = new Vector2(18f, 8f);
        Vector2 offsetMax = new Vector2(-18f, -8f);

        if (textTransform.offsetMin != offsetMin)
        {
            textTransform.offsetMin = offsetMin;
            changed = true;
        }

        if (textTransform.offsetMax != offsetMax)
        {
            textTransform.offsetMax = offsetMax;
            changed = true;
        }

        return changed;
    }

    private static RectTransform EnsureRectTransform(GameObject owner, ref bool changed)
    {
        RectTransform rectTransform = owner.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            return rectTransform;
        }

        rectTransform = owner.AddComponent<RectTransform>();
        changed = true;
        return rectTransform;
    }

    private static bool SetRectTransform(RectTransform rectTransform, Vector2 size)
    {
        if (rectTransform.sizeDelta == size)
        {
            return false;
        }

        rectTransform.sizeDelta = size;
        return true;
    }

    private static T[] GetSerializedObjectReferenceArray<T>(Object target, string propertyName) where T : Object
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property == null || !property.isArray)
        {
            return new T[0];
        }

        List<T> values = new List<T>(property.arraySize);

        for (int i = 0; i < property.arraySize; i++)
        {
            values.Add(property.GetArrayElementAtIndex(i).objectReferenceValue as T);
        }

        return values.ToArray();
    }

    private static void ApplyThumbnailImportSettings()
    {
        if (!AssetDatabase.IsValidFolder(ThumbnailRoot))
        {
            return;
        }

        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { ThumbnailRoot });

        foreach (string textureGuid in textureGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(textureGuid);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer == null)
            {
                continue;
            }

            bool changed = false;

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }

            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }

            if (!Mathf.Approximately(importer.spritePixelsPerUnit, 100f))
            {
                importer.spritePixelsPerUnit = 100f;
                changed = true;
            }

            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }

            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }

            if (changed)
            {
                importer.SaveAndReimport();
            }
        }
    }

    private static void ApplyUiRuntimeImportSettings()
    {
        if (!AssetDatabase.IsValidFolder(RuntimeUiRoot))
        {
            return;
        }

        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { RuntimeUiRoot });

        foreach (string textureGuid in textureGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(textureGuid);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer == null)
            {
                continue;
            }

            Vector4 border = GetUiSpriteBorder(assetPath);
            bool changed = false;

            if (importer.spriteBorder != border)
            {
                importer.spriteBorder = border;
                changed = true;
            }

            if (changed)
            {
                importer.SaveAndReimport();
            }
        }
    }

    private static Vector4 GetUiSpriteBorder(string assetPath)
    {
        if (assetPath.Contains("/Buttons/"))
        {
            return new Vector4(260f, 160f, 260f, 160f);
        }

        if (assetPath.Contains("/Panels/"))
        {
            return new Vector4(220f, 220f, 220f, 220f);
        }

        return Vector4.zero;
    }

    private static void RestoreActiveScene(string activeScenePath)
    {
        if (!string.IsNullOrEmpty(activeScenePath))
        {
            EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
        }
    }

    private static IEnumerable<string> FindScenePaths()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Scene", new[] { SceneRoot }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (path.EndsWith("/UI_Showcase.unity"))
            {
                continue;
            }

            yield return path;
        }
    }

    private static void EnsureFolder(string folder)
    {
        if (AssetDatabase.IsValidFolder(folder))
        {
            return;
        }

        string parent = Path.GetDirectoryName(folder)?.Replace("\\", "/");
        string name = Path.GetFileName(folder);

        if (!string.IsNullOrEmpty(parent))
        {
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }

    private static Font GetBuiltinFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private sealed class SpriteLibrary
    {
        public Sprite ButtonNormal { get; private set; }
        public Sprite ButtonHover { get; private set; }
        public Sprite ButtonPressed { get; private set; }
        public Sprite ButtonDanger { get; private set; }
        public Sprite ButtonConfirm { get; private set; }

        public Sprite HealthFrame { get; private set; }
        public Sprite HealthFill { get; private set; }
        public Sprite AmmoPanel { get; private set; }
        public Sprite MoneyPanel { get; private set; }
        public Sprite WeaponPanel { get; private set; }
        public Sprite KeycardPanel { get; private set; }
        public Sprite BuffStatusPanel { get; private set; }
        public Sprite BuffChoiceCard { get; private set; }
        public Sprite ShopItemCard { get; private set; }
        public Sprite ObjectivePanel { get; private set; }
        public Sprite PauseMenuPanel { get; private set; }
        public Sprite BossHealthFrame { get; private set; }
        public Sprite BossHealthFill { get; private set; }
        public Sprite UnlockedLevelCard { get; private set; }
        public Sprite LockedLevelCard { get; private set; }

        public Sprite HealthIcon { get; private set; }
        public Sprite AmmoIcon { get; private set; }
        public Sprite MoneyIcon { get; private set; }
        public Sprite WeaponIcon { get; private set; }
        public Sprite KeycardIcon { get; private set; }
        public Sprite BuffIcon { get; private set; }
        public Sprite BossIcon { get; private set; }
        public Sprite ObjectiveIcon { get; private set; }
        public Sprite WarningIcon { get; private set; }
        public Sprite ReloadIcon { get; private set; }

        public Sprite LaboratoryThumbnail { get; private set; }
        public Sprite PrisonThumbnail { get; private set; }
        public Sprite ZombieCityThumbnail { get; private set; }
        public Sprite SciFiBaseThumbnail { get; private set; }
        public Sprite HorrorHospitalThumbnail { get; private set; }

        public static SpriteLibrary Load()
        {
            return new SpriteLibrary
            {
                ButtonNormal = LoadRuntimeSprite("Buttons/UI_Button_Normal.png"),
                ButtonHover = LoadRuntimeSprite("Buttons/UI_Button_Hover.png"),
                ButtonPressed = LoadRuntimeSprite("Buttons/UI_Button_Pressed.png"),
                ButtonDanger = LoadRuntimeSprite("Buttons/UI_Button_Normal.png"),
                ButtonConfirm = LoadRuntimeSprite("Buttons/UI_Button_Normal.png"),

                HealthFrame = LoadRuntimeSprite("Panels/UI_HealthBar_Frame.png"),
                HealthFill = LoadRuntimeSprite("Panels/UI_HealthBar_Fill.png"),
                AmmoPanel = LoadRuntimeSprite("Panels/UI_AmmoPanel.png"),
                MoneyPanel = LoadRuntimeSprite("Panels/UI_MoneyPanel.png"),
                WeaponPanel = LoadRuntimeSprite("Panels/UI_WeaponPanel.png"),
                KeycardPanel = LoadRuntimeSprite("Panels/UI_KeycardPanel.png"),
                BuffStatusPanel = LoadRuntimeSprite("Panels/UI_BuffStatusPanel.png"),
                BuffChoiceCard = LoadRuntimeSprite("Panels/UI_BuffChoiceCard.png"),
                ShopItemCard = LoadRuntimeSprite("Panels/UI_ShopItemCard.png"),
                ObjectivePanel = LoadRuntimeSprite("Panels/UI_ObjectivePanel.png"),
                PauseMenuPanel = LoadRuntimeSprite("Panels/UI_PauseMenuPanel.png"),
                BossHealthFrame = LoadRuntimeSprite("Panels/UI_BossHealthBar_Frame.png"),
                BossHealthFill = LoadRuntimeSprite("Panels/UI_BossHealthBar_Fill.png"),
                UnlockedLevelCard = LoadRuntimeSprite("Panels/UI_UnlockedLevelCard.png"),
                LockedLevelCard = LoadRuntimeSprite("Panels/UI_LockedLevelCard.png"),

                HealthIcon = LoadRuntimeSprite("Icons/UI_Icon_Health.png"),
                AmmoIcon = LoadRuntimeSprite("Icons/UI_Icon_Ammo.png"),
                MoneyIcon = LoadRuntimeSprite("Icons/UI_Icon_Money.png"),
                WeaponIcon = LoadRuntimeSprite("Icons/UI_Icon_Weapon.png"),
                KeycardIcon = LoadRuntimeSprite("Icons/UI_Icon_Keycard.png"),
                BuffIcon = LoadRuntimeSprite("Icons/UI_Icon_Buff.png"),
                BossIcon = LoadRuntimeSprite("Icons/UI_Icon_Boss.png"),
                ObjectiveIcon = LoadRuntimeSprite("Icons/UI_Icon_Objective.png"),
                WarningIcon = LoadRuntimeSprite("Icons/UI_Icon_Warning.png"),
                ReloadIcon = LoadRuntimeSprite("Icons/UI_Icon_Reload.png"),

                LaboratoryThumbnail = LoadThumbnail("Thumbnail_Laboratory.png"),
                PrisonThumbnail = LoadThumbnail("Thumbnail_Prison.png"),
                ZombieCityThumbnail = LoadThumbnail("Thumbnail_ZombieCity.png"),
                SciFiBaseThumbnail = LoadThumbnail("Thumbnail_SciFiBase.png"),
                HorrorHospitalThumbnail = LoadThumbnail("Thumbnail_HorrorHospital.png")
            };
        }

        public Sprite GetLevelThumbnail(int levelIndex)
        {
            switch (levelIndex)
            {
                case 1:
                    return LaboratoryThumbnail;
                case 2:
                    return PrisonThumbnail;
                case 3:
                    return ZombieCityThumbnail;
                case 4:
                    return SciFiBaseThumbnail;
                case 5:
                    return HorrorHospitalThumbnail;
                default:
                    return LaboratoryThumbnail;
            }
        }

        private static Sprite LoadRuntimeSprite(string relativePath)
        {
            return LoadSprite($"{RuntimeUiRoot}/{relativePath}");
        }

        private static Sprite LoadThumbnail(string fileName)
        {
            return LoadSprite($"{ThumbnailRoot}/{fileName}");
        }

        private static Sprite LoadSprite(string path)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null)
            {
                Debug.LogWarning($"Missing UI visual sprite: {path}");
            }

            return sprite;
        }
    }
}
#endif
