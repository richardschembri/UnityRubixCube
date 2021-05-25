using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRubixCube.Utils;

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
        public CubieFace FaceUp { get { return _faceUp; } }
        [SerializeField]
        private CubieFace _faceDown;
        public CubieFace FaceDown { get { return _faceDown; } }
        [SerializeField]
        private CubieFace _faceLeft;
        public CubieFace FaceLeft { get { return _faceLeft; } }
        [SerializeField]
        private CubieFace _faceRight;
        public CubieFace FaceRight { get { return _faceRight; } }
        [SerializeField]
        private CubieFace _faceFront;
        public CubieFace FaceFront { get { return _faceFront; } }
        [SerializeField]
        private CubieFace _faceBack;
        public CubieFace FaceBack { get { return _faceBack; } }
        public CubieIndex Index {get; private set;}

        public RubixCube ParentCube {get; private set;}

        public void SetValues(CubieIndex index, Vector3 localPosition, Vector3 localScale, Quaternion localRotation){
            ParentCube = transform.GetComponentInParent<RubixCube>();
            Index = index;
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            transform.localRotation = localRotation;
            RefreshName();
            // ToggleFaces();
        }

        public void SetValues(RubixSaveUtils.CubieSaveInfo csi){
            SetValues(csi.Index, csi.localPosition, csi.localScale, csi.localRotation);
            FaceUp.gameObject.SetActive(csi.faceUp);
            FaceDown.gameObject.SetActive(csi.faceDown);
            FaceLeft.gameObject.SetActive(csi.faceLeft);
            FaceRight.gameObject.SetActive(csi.faceRight);
            FaceFront.gameObject.SetActive(csi.faceFront);
            FaceBack.gameObject.SetActive(csi.faceBack);
        }
        public void ToggleFaces(){
            _faceBack.gameObject.SetActive(Index.x == 0);
            _faceFront.gameObject.SetActive(Index.x == ParentCube.CubiesPerSide - 1);
            _faceDown.gameObject.SetActive(Index.y == 0);
            _faceUp.gameObject.SetActive(Index.y == ParentCube.CubiesPerSide - 1);
            _faceRight.gameObject.SetActive(Index.z == 0);
            _faceLeft.gameObject.SetActive(Index.z == ParentCube.CubiesPerSide - 1);
        }

        public void ToggleFacesOn(bool on){
            _faceBack.gameObject.SetActive(on);
            _faceFront.gameObject.SetActive(on);
            _faceDown.gameObject.SetActive(on);
            _faceUp.gameObject.SetActive(on);
            _faceRight.gameObject.SetActive(on);
            _faceLeft.gameObject.SetActive(on);
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