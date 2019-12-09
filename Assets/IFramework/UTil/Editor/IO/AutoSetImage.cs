/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-02-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.IO;
using UnityEditor;
namespace IFramework
{
	public class AutoSetImage : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (Path.GetFileName(Path.GetDirectoryName(assetImporter.assetPath)) == "Sprites")
            {
                TextureImporter texImpoter = assetImporter as TextureImporter;
                texImpoter.textureType = TextureImporterType.Sprite;
                texImpoter.maxTextureSize = 512;
            }
        }
    }
}
