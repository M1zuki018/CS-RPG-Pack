using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// CustomUIオブジェクトを追加する処理をメニューに追加
/// </summary>
public class CustomUIMenuItems
{
    #region Constants

    private const string CANVAS_NAME = "Canvas";
    private const string CUSTOM_TEXT_NAME = "CustomText";
    private const string CUSTOM_BUTTON_NAME = "CustomButton";
    private const string CUSTOM_IMAGE_NAME = "CustomImage";

    #endregion
    
    #region Custom Text

    [MenuItem("GameObject/UI/Custom UI/Custom Text", false, 10)]
    private static void CreateCustomText(MenuCommand menuCommand)
    {
        var canvas = CreateCanvas();

        // CustomTextオブジェクトを作成
        GameObject customTextGO = new GameObject(CUSTOM_TEXT_NAME);
        customTextGO.transform.SetParent(canvas.transform, false);
        
        // 必要なコンポーネントを追加
        RectTransform rectTransform = customTextGO.AddComponent<RectTransform>();
        customTextGO.AddComponent<CanvasRenderer>();
        customTextGO.AddComponent<CustomText>();
        
        // RectTransformの初期設定
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        // 作成したオブジェクトを選択状態にする
        Selection.activeGameObject = customTextGO;
        
        // Undoに登録
        Undo.RegisterCreatedObjectUndo(customTextGO, "Create Custom Text");
    }

    #endregion

    #region Custom Button

    [MenuItem("GameObject/UI/Custom UI/Custom Button", false, 11)]
    static void CreateCustomButton(MenuCommand menuCommand)
    {
        var canvas = CreateCanvas();

        GameObject customButtonGO = new GameObject(CUSTOM_BUTTON_NAME);
        customButtonGO.transform.SetParent(canvas.transform, false);
        
        RectTransform rectTransform = customButtonGO.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160, 30);
        
        customButtonGO.AddComponent<CanvasRenderer>();
        customButtonGO.AddComponent<CustomImage>();
        customButtonGO.AddComponent<CustomButton>();
        
        GameObject childGO = new GameObject("Text");
        childGO.transform.SetParent(customButtonGO.transform, false);
        childGO.AddComponent<CanvasRenderer>();
        childGO.AddComponent<CustomText>();
        
        // 作成したオブジェクトを選択状態にする
        Selection.activeGameObject = customButtonGO;
        
        Undo.RegisterCreatedObjectUndo(customButtonGO, "Create Custom Button");
    }

    #endregion
    
    #region Custom Image

    [MenuItem("GameObject/UI/Custom UI/Custom Image", false, 10)]
    private static void CreateCustomImage(MenuCommand menuCommand)
    {
        var canvas = CreateCanvas();

        // CustomTextオブジェクトを作成
        GameObject customTextGO = new GameObject(CUSTOM_IMAGE_NAME);
        customTextGO.transform.SetParent(canvas.transform, false);
        
        // 必要なコンポーネントを追加
        RectTransform rectTransform = customTextGO.AddComponent<RectTransform>();
        customTextGO.AddComponent<CanvasRenderer>();
        customTextGO.AddComponent<CustomImage>();
        
        // RectTransformの初期設定
        rectTransform.sizeDelta = new Vector2(100, 100);
        rectTransform.anchoredPosition = Vector2.zero;

        // 作成したオブジェクトを選択状態にする
        Selection.activeGameObject = customTextGO;
        
        // Undoに登録
        Undo.RegisterCreatedObjectUndo(customTextGO, "Create Custom Image");
    }

    #endregion
    
    /// <summary>
    /// Canvasが存在しない場合、Canvasを作成
    /// </summary>
    private static Canvas CreateCanvas()
    {
        // 親となるCanvasを取得または作成
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            // Canvasが存在しない場合は作成
            GameObject canvasGO = new GameObject(CANVAS_NAME);
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        return canvas;
    }
}