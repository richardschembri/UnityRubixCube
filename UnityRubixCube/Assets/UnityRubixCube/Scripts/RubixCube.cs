using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit;

namespace UnityRubixCube {
    [RequireComponent(typeof(CubieSpawner))]
    public class RubixCube : RSMonoBehaviour
    {
        [SerializeField]
        private int _cubiesPerSide = 3;
        private CubieSpawner _cubieSpawnerComponent;

        private Transform _rotatingLayer;

        #region RSMonoBehaviour Functions
        protected override void InitComponents()
        {
            base.InitComponents();
            _cubieSpawnerComponent = GetComponent<CubieSpawner>();
        }

        #endregion RSMonoBehaviour Functions

        public void GenerateCube(){
            _cubieSpawnerComponent.GenerateCube(_cubiesPerSide);
        }
        // Start is called before the first frame update
        void Start()
        {
            if(!Initialized){
                return;
            }
            GenerateCube();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

}