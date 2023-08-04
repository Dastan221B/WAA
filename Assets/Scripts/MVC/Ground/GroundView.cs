using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MVC.Ground
{
    public class GroundView 
    {
        [System.Serializable]
        public class SplatHeights
        {
            public int TextureIndex;
            public TerrainTypes TerrainTypes;
        }

        public Terrain TerrainMain;
        private GroundModel _groundModel;

        //public SplatHeights[] splatHeights;

        public void StartPaint()
        {
            //TerrainData terrainData = TerrainMain.terrainData;

            //var splatHeights = new SplatHeights[]
            //{
            //    new SplatHeights() { TextureIndex = 0,TerrainTypes = TerrainTypes.DIRT_ROAD},
            //    new SplatHeights() { TextureIndex = 1,TerrainTypes =  TerrainTypes.GRASS},
            //    new SplatHeights() { TextureIndex = 2,TerrainTypes = TerrainTypes.RIVER},
            //};

            //float[,,] splatmapData = new float[terrainData.alphamapWidth,
            //                                   terrainData.alphamapHeight,
            //                                   terrainData.alphamapLayers];

            //for (int y = 0; y < terrainData.alphamapHeight; y++)
            //{
            //    for (int x = 0; x < terrainData.alphamapWidth; x++)
            //    {
            //        float terrainHeight = terrainData.GetHeight(y, x);

            //        float[] splat = new float[splatHeights.Length];

            //        if(_groundModel.TryGetCellsByCoordinates(x,y,out Cell cell))
            //        {

            //            for (int i = 0; i < splatHeights.Length; i++)
            //            {
            //                if (i == splatHeights.Length - 1 && terrainHeight >= splatHeights[i].startingHeight)
            //                {
            //                    splat[i] = 1;
            //                }
            //                else if (terrainHeight >= splatHeights[i].startingHeight &&
            //                    terrainHeight <= splatHeights[i + 1].startingHeight)
            //                {
            //                    splat[i] = 1;
            //                }
            //            }
            //        }

            //        for (int j = 0; j < splatHeights.Length; j++)
            //        {
            //            splatmapData[x, y, j] = splat[j];
            //        }
            //    }
            //}

            //terrainData.SetAlphamaps(0, 0, splatmapData);
        }
    }

}