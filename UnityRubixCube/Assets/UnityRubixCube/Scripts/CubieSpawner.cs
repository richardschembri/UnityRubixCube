using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Controls;
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
                                            localScale);
                        xIndexPositions[x] = newCubie.transform.localPosition.x;
                        yIndexPositions[y] = newCubie.transform.localPosition.y;
                        zIndexPositions[z] = newCubie.transform.localPosition.z;
                    }
                }
            }

            return true;
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