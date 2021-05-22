using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Controls;

namespace UnityRubixCube {
    public class CubieSpawner : Spawner<Cubie>
    {
        private bool isEdge(int axisIndex, int cubiesPerSide){
            return axisIndex > 0 && axisIndex < cubiesPerSide - 1;
        }
        public bool GenerateCube(int cubiesPerSide){
            if(GameObjectToSpawn == null) return false;
            DestroyAllSpawns();
            Vector3 localScale = Vector3.one / cubiesPerSide;
            Vector3 offset = Vector3.one * ((1f / cubiesPerSide * 0.5f) - 0.5f);
            Cubie newCubie;
            for(int x = 0; x < cubiesPerSide; x++){
                for(int y = 0; y < cubiesPerSide; y++){
                    for(int z = 0; z < cubiesPerSide; z++){
                        if(isEdge(x, cubiesPerSide) && isEdge(y, cubiesPerSide) && isEdge(z, cubiesPerSide)){
                            // Do not spawn inner cubies
                            continue;
                        }
                        newCubie = SpawnAndGetGameObject();
                        newCubie.SetValues( new Vector3(x,y,z), // 3D Index
                                            (new Vector3(x,y,z) / cubiesPerSide) + offset, // Local Position
                                            localScale);
                    }
                }
            }

            return true;
        }
    }
}