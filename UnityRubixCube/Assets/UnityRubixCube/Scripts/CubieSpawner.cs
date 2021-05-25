using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Controls;
using UnityRubixCube.Utils;
using System.Collections.ObjectModel;

namespace UnityRubixCube {
    public class CubieSpawner : Spawner<Cubie>
    {
        float[] xIndexPositions;
        float[] yIndexPositions;
        float[] zIndexPositions;

        public RubixCube ParentCube {get; private set;}
        protected override void InitComponents()
        {
            base.InitComponents();
            ParentCube = GetComponent<RubixCube>();
        }

        private bool isInner(int axisIndex){
            return axisIndex > 0 && axisIndex < ParentCube.CubiesPerSide - 1;
        }


        
        public bool GenerateCube(){
            if(GameObjectToSpawn == null) return false;

            xIndexPositions = new float[ParentCube.CubiesPerSide];
            yIndexPositions = new float[ParentCube.CubiesPerSide];
            zIndexPositions = new float[ParentCube.CubiesPerSide];
            DestroyAllSpawns();
            Vector3 localScale = Vector3.one / ParentCube.CubiesPerSide;
            Vector3 offset = Vector3.one * ((1f / ParentCube.CubiesPerSide * 0.5f) - 0.5f);
            Cubie newCubie;
            for(int x = 0; x < ParentCube.CubiesPerSide; x++){
                for(int y = 0; y < ParentCube.CubiesPerSide; y++){
                    for(int z = 0; z < ParentCube.CubiesPerSide; z++){
                        if(isInner(x) && isInner(y) && isInner(z)){
                            // Do not spawn inner cubies
                            continue;
                        }
                        newCubie = SpawnAndGetGameObject();
                        newCubie.SetValues( new Cubie.CubieIndex(x,y,z),
                                            (new Vector3(x,y,z) / ParentCube.CubiesPerSide) + offset, // Local Position
                                            localScale, Quaternion.Euler(0f,0f,0f));
                        xIndexPositions[x] = newCubie.transform.localPosition.x;
                        yIndexPositions[y] = newCubie.transform.localPosition.y;
                        zIndexPositions[z] = newCubie.transform.localPosition.z;
                    }
                }
            }

            return true;
        }
        public bool SaveCube(){
            if(SpawnedGameObjects.Count <= 0){
                return false;
            }
            RubixSaveUtils.SaveCubies(SpawnedGameObjects);
            return true;
        }
        public bool RestoreCube(ref int cubiesPerSide){
            cubiesPerSide = 0;
            if(GameObjectToSpawn == null) 
                return false;

            var cubieSaveInfos = RubixSaveUtils.LoadCubieSaveInfos();
            if(cubieSaveInfos == null || cubieSaveInfos.Length <= 0)
                return false;

            DestroyAllSpawns();

            cubiesPerSide = Mathf.CeilToInt(Mathf.Pow(cubieSaveInfos.Length, 1f / 3f));
            Debug.Log($"Cubies per side:{cubiesPerSide}");
            xIndexPositions = new float[cubiesPerSide];
            yIndexPositions = new float[cubiesPerSide];
            zIndexPositions = new float[cubiesPerSide];
            Cubie newCubie;
            for(int i = 0; i < cubieSaveInfos.Length; i++){
                newCubie = SpawnAndGetGameObject();
                newCubie.LoadFromCubieSaveInfo(cubieSaveInfos[i]);

                xIndexPositions[newCubie.Index.x] = newCubie.transform.localPosition.x;
                yIndexPositions[newCubie.Index.y] = newCubie.transform.localPosition.y;
                zIndexPositions[newCubie.Index.z] = newCubie.transform.localPosition.z;
                // Displaying all faces due to weird bug
                newCubie.ToggleFacesOn(true);
            }

            return false;
        }
        private int GetAxisIndex(float[] indexList, float axis){
            float treshold = ParentCube.GetTreshold();
            for(int i = 0; i < indexList.Length; i++){
                if(Mathf.Abs(indexList[i] - axis) < treshold){
                    return i;
                }
            }
            return -1;
        }

        public Cubie.CubieIndex GetLocalPositionIndex(Vector3 localPosition){
            int x = GetAxisIndex(xIndexPositions, localPosition.x);
            int y = GetAxisIndex(yIndexPositions, localPosition.y);
            int z = GetAxisIndex(zIndexPositions, localPosition.z);
            return new Cubie.CubieIndex(x, y, z);
        }
    }
}