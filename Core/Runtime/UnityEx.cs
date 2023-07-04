/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.IO;
using UnityEngine;

namespace IFramework
{
    public static class UnityEx
    {
        public static string ToAbsPath(this string self)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            assetRootPath = assetRootPath.Substring(0, assetRootPath.Length - 6) + self;
            return assetRootPath.ToRegularPath();
        }
        public static string ToAssetsPath(this string self)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            return "Assets" + Path.GetFullPath(self).Substring(assetRootPath.Length).Replace("\\", "/");
        }
        public static T MakeComponentExist<T>(this T selfComponent) where T : Component
        {
            T t;
            t = selfComponent.GetComponent<T>();
            return t == null ? selfComponent.gameObject.AddComponent<T>() : t;
        }
        public static void RmoveComponent<T>(this T selfComponent) where T : Component
        {
            T t;
            t = selfComponent.GetComponent<T>();
            if (t!=null)
            {
                UnityEngine.Object.Destroy(t);
            }
        }
        public static T LocalIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localPosition = Vector3.zero;
            selfComponent.transform.localRotation = Quaternion.identity;
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }
        public static string GetPath(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();
            var t = transform;
            while (true)
            {
                sb.Insert(0, t.name);
                t = t.parent;
                if (t)
                {
                    sb.Insert(0, "/");
                }
                else
                {
                    return sb.ToString();
                }
            }
        }

        public static Sprite CreateSprite(this Texture2D self)
        {
            return Sprite.Create(self,
                                new Rect(0, 0, self.width, self.height),
                                new Vector2(0.5f, 0.5f));
        }
        public static Texture2D CreateReadableTexture(this Texture self)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                   self.width,
                   self.height,
                   0,
                   RenderTextureFormat.Default,
                   RenderTextureReadWrite.Linear);
            RenderTexture previous = RenderTexture.active;

            Graphics.Blit(self, tmp);
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(self.width, self.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);
            return myTexture2D;
        }


        public static Vector2 GetPosInRootTrans(this RectTransform self, Transform rootTrans)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, self).center;
        }

        public static RectTransform AnchorPosX(this RectTransform self, float anchorPosX)
        {
            var anchorPos = self.anchoredPosition;
            anchorPos.x = anchorPosX;
            self.anchoredPosition = anchorPos;
            return self;
        }

        public static RectTransform SetSizeWidth(this RectTransform self, float sizeWidth)
        {
            var sizeDelta = self.sizeDelta;
            sizeDelta.x = sizeWidth;
            self.sizeDelta = sizeDelta;
            return self;
        }

        public static RectTransform SetSizeHeight(this RectTransform self, float sizeHeight)
        {
            var sizeDelta = self.sizeDelta;
            sizeDelta.y = sizeHeight;
            self.sizeDelta = sizeDelta;
            return self;
        }

        public static Vector2 GetWorldSize(this RectTransform self)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(self).size;
        }

        public static Texture2D Screenshot(this Camera camera, Rect rect)
        {
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;

            var screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.Destroy(renderTexture);

            return screenShot;
        }

    }
}