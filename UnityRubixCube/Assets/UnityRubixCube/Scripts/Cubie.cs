﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRubixCube {
    public class Cubie : MonoBehaviour
    {
        public struct CubieIndex{
            public readonly int x;
            public readonly int y; 
            public readonly int z;
            public CubieIndex(int x, int y, int z){
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
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
        public CubieIndex Index {get; private set;}

        public RubixCube ParentCube {get; private set;}

        public void SetValues(CubieIndex index, Vector3 localPosition, Vector3 localScale){
            ParentCube = transform.parent.GetComponent<RubixCube>();
            Index = index;
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            RefreshName();
            ToggleFaces();
        }

        public void ToggleFaces(){
            _faceBack.SetActive(Index.x == 0);
            _faceFront.SetActive(Index.x == ParentCube.CubiesPerSide - 1);
            _faceDown.SetActive(Index.y == 0);
            _faceUp.SetActive(Index.y == ParentCube.CubiesPerSide - 1);
            _faceRight.SetActive(Index.z == 0);
            _faceLeft.SetActive(Index.z == ParentCube.CubiesPerSide - 1);
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