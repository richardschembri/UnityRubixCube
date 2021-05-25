using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRubixCube {
    public class Cubie : MonoBehaviour
    {
        [Serializable]
        public struct CubieIndex{
            public int x;
            public int y; 
            public int z;
            public CubieIndex(int x, int y, int z){
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
        [SerializeField]
        private CubieFace _faceUp;
        [SerializeField]
        private CubieFace _faceDown;
        [SerializeField]
        private CubieFace _faceLeft;
        [SerializeField]
        private CubieFace _faceRight;
        [SerializeField]
        private CubieFace _faceFront;
        [SerializeField]
        private CubieFace _faceBack;
        public CubieIndex Index {get; private set;}

        public RubixCube ParentCube {get; private set;}

        public void SetValues(CubieIndex index, Vector3 localPosition, Vector3 localScale, Quaternion localRotation){
            ParentCube = transform.GetComponentInParent<RubixCube>();
            Index = index;
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            transform.localRotation = localRotation;
            RefreshName();
            ToggleFaces();
        }

        public void ToggleFaces(){
            _faceBack.gameObject.SetActive(Index.x == 0);
            _faceFront.gameObject.SetActive(Index.x == ParentCube.CubiesPerSide - 1);
            _faceDown.gameObject.SetActive(Index.y == 0);
            _faceUp.gameObject.SetActive(Index.y == ParentCube.CubiesPerSide - 1);
            _faceRight.gameObject.SetActive(Index.z == 0);
            _faceLeft.gameObject.SetActive(Index.z == ParentCube.CubiesPerSide - 1);
        }

        private void RefreshName(){
            name = $"Cubie(x:{Index.x},y:{Index.y},z:{Index.z})";
        }

        public void RefreshIndex(){
            Index = ParentCube.GetLocalPositionIndex(transform.localPosition);
            RefreshName();
        }

        public bool SelectCubie(){
            return ParentCube.SelectCubie(this);
        }

        public bool DeselectCubie(){
            return ParentCube.DeselectCubie(this);
        }

        public bool IsCubieSelected(){
            return ParentCube.IsCubieSelected(this);
        }
        
        public List<Cubie> GetNeighbours(){
            return ParentCube.GetNeighbours(this);
        }
       private bool IsEdge(int index1D){
           return index1D == 0 || index1D == ParentCube.CubiesPerSide - 1;
       } 
        public bool IsEdge(RubixCube.ERubixAxis axis){
            switch (axis){
                case RubixCube.ERubixAxis.X:
                    return IsEdge(Index.x); 
                case RubixCube.ERubixAxis.Y:
                    return IsEdge(Index.y); 
                case RubixCube.ERubixAxis.Z:
                    return IsEdge(Index.z); 
            }
            return false;
        }

        public bool IsMouseOver(){
            return _faceUp.IsMouseOver ||
            _faceDown.IsMouseOver ||
            _faceLeft.IsMouseOver ||
            _faceRight.IsMouseOver ||
            _faceFront.IsMouseOver ||
            _faceBack.IsMouseOver; 
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