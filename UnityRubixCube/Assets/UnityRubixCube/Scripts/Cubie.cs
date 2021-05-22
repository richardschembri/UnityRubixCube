using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRubixCube {
    public class Cubie : MonoBehaviour
    {
        [SerializeField]
        private GameObject _faceUp;
        [SerializeField]
        private GameObject _faceDown;
        [SerializeField]
        private GameObject _faceLeft;
        [SerializeField]
        private GameObject _faceRight;
        [SerializeField]
        private GameObject _faceFront;
        [SerializeField]
        private GameObject _faceBack;
        public Vector3 Index3D {get; private set;}

        public RubixCube ParentCube {get; private set;}

        public void SetValues(Vector3 index3D, Vector3 localPosition, Vector3 localScale){
            ParentCube = transform.parent.GetComponent<RubixCube>();
            Index3D = index3D;
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            name = $"Cubie(x:{index3D.x},y:{index3D.y},z:{index3D.z})";
            ToggleFaces();
        }

        public void ToggleFaces(){
            _faceBack.SetActive(Index3D.x == 0);
            _faceFront.SetActive(Index3D.x == ParentCube.CubiesPerSide - 1);
            _faceDown.SetActive(Index3D.y == 0);
            _faceUp.SetActive(Index3D.y == ParentCube.CubiesPerSide - 1);
            _faceRight.SetActive(Index3D.z == 0);
            _faceLeft.SetActive(Index3D.z == ParentCube.CubiesPerSide - 1);
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}