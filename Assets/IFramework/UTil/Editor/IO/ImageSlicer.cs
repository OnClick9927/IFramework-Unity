/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-02-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;

namespace IFramework
{
    public static class ImageSlicer
    {
        [MenuItem("IFramework/Image/Process to Sprites")]
        static void ProcessToSprite()
        {
            Texture2D image = Selection.activeObject as Texture2D;//获取旋转的对象
            string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(image));//获取路径名称
            string path = AssetDatabase.GetAssetPath(image);//图片路径名称
            TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;//获取图片入口
            AssetDatabase.CreateFolder(rootPath, image.name);//创建文件夹
            foreach (SpriteMetaData metaData in texImp.spritesheet)//遍历小图集
            {
                Texture2D myimage = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);
                for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++)//Y轴像素
                {
                    for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                        myimage.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, image.GetPixel(x, y));
                }
                //转换纹理到EncodeToPNG兼容格式
                if (myimage.format != TextureFormat.ARGB32 && myimage.format != TextureFormat.RGB24)
                {
                    Texture2D newTexture = new Texture2D(myimage.width, myimage.height);
                    newTexture.SetPixels(myimage.GetPixels(0), 0);
                    myimage = newTexture;
                }
                var pngData = myimage.EncodeToPNG();
                //AssetDatabase.CreateAsset(myimage, rootPath + "/" + image.name + "/" + metaData.name + ".PNG");
                File.WriteAllBytes(rootPath + "/" + image.name + "/" + metaData.name + ".PNG", pngData);
                AssetDatabase.Refresh();
            }
        }
    }
}
