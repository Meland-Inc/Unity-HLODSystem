using System.Collections.Generic;
using Unity.Collections;
using Unity.HLODSystem.Utils;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

namespace Unity.HLODSystem
{
    public class HLODBuilder : IProcessSceneWithReport
    {
        public int callbackOrder
        {
            get { return 0; }
        }
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();
            List<Terrain> terrains = new();
            List<TerrainData> needDestroyDatas = new();
            List<HLOD> hlods = new();
            List<TerrainHLOD> terrainHlods = new();
            for (int oi = 0; oi < rootObjects.Length; ++oi)
            {
                FindHLodComponents(rootObjects[oi], ref hlods, ref terrainHlods, ref terrains);
            }

            for (int hi = 0; hi < hlods.Count; ++hi)
            {
                Object.DestroyImmediate(hlods[hi]);
            }

            for (int hi = 0; hi < terrainHlods.Count; ++hi)
            {
                if (terrainHlods[hi].DestroyTerrain)
                {
                    needDestroyDatas.Add(terrainHlods[hi].TerrainData);
                }
                Object.DestroyImmediate(terrainHlods[hi]);
            }


            for (int ti = 0; ti < terrains.Count; ++ti)
            {
                if (terrains[ti] == null)
                    continue;

                bool needDestroy = needDestroyDatas.Contains(terrains[ti].terrainData);
                if (needDestroy)
                {
                    Object.DestroyImmediate(terrains[ti]);
                }
            }
        }

        private void FindHLodComponents(GameObject target, ref List<HLOD> hLodList, ref List<TerrainHLOD> terrainHLodList, ref List<Terrain> terrainList)
        {

            Component[] components = target.GetComponents<Component>();

            for (int i = components.Length - 1; i >= 0; i--)
            {
                Component comp = components[i];

                //hlod
                if (comp is HLOD)
                {
                    hLodList.Add(comp as HLOD);
                    continue;
                }

                //地形HLod  
                if (comp is TerrainHLOD)
                {
                    terrainHLodList.Add(comp as TerrainHLOD);
                    continue;

                }
                //地形
                if (comp is Terrain)
                {
                    terrainList.Add(comp as Terrain);
                    continue;
                }
            }

            int childCount = target.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform childTransform = target.transform.GetChild(i);
                FindHLodComponents(childTransform.gameObject, ref hLodList, ref terrainHLodList, ref terrainList);
            }
        }
    }
}