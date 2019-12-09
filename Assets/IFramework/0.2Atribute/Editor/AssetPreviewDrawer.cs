/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [CustomPropertyDrawer(typeof(AssetPreviewAttribute))]
    internal class AssetPreviewDrawer :  PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position,property, false);
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue != null)
                {
                    Texture2D previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
                    if (previewTexture != null)
                    {
                        AssetPreviewAttribute showAssetPreviewAttribute = this.attribute as AssetPreviewAttribute;
                        int width = Mathf.Clamp(showAssetPreviewAttribute.Width, 0, previewTexture.width);
                        int height = Mathf.Clamp(showAssetPreviewAttribute.Height, 0, previewTexture.height);

                        GUILayout.Label(previewTexture, GUILayout.MaxWidth(width), GUILayout.MaxHeight(height));
                    }
                    else
                    {
                        string warning = property.name + " doesn't have an asset preview";

                        EditorGUILayout.HelpBox(warning,UnityEditor. MessageType.Warning);
                    }
                }
            }
            else
            {
                string warning = property.name + " doesn't have an asset preview";

                EditorGUILayout.HelpBox(warning, UnityEditor.MessageType.Warning);
            }
        }
    }
   

}
