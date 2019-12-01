/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework
{
    public static class TextureExtension
	{
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
       
    }


}
