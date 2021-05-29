﻿using UnityEngine;
using RSToolkit;
using System.Collections.Generic;
using UnityRubixCube.Managers;

namespace UnityRubixCube {
    public class CubieFace : RSMonoBehaviour 
    {
        public enum EMouseDirection{
            NONE,
            X, Y
        }

        private EMouseDirection _mouseDirection = EMouseDirection.NONE;
        public Cubie ParentCubie {get; private set;}

        private float _mouseZ;

        private Vector3? _dragStart = null;
        private Vector3 _offset;
        private List<Cubie> _cubieNeighbours = null;
        
        Cubie _closestCubie;
        Vector3 _mouseWorldPoint;

        public bool IsMouseOver {get; private set;}= false;

        float _dragDistance;

        protected override void InitComponents()
        {
            base.InitComponents();
            ParentCubie = transform.parent.GetComponent<Cubie>();
        }

        private Vector3 GetMouseAsWorldPoint(Vector3 mousePosition)
        {

            Vector3 mousePosition3D = mousePosition;

            mousePosition3D.z = _mouseZ;

            return Camera.main.ScreenToWorldPoint(mousePosition3D);

        }

        public override void ResetValues(){
            _dragStart = null;
            _cubieNeighbours = null;
            ParentCubie.DeselectCubie();
        }

        #region Mouse Events

        void OnMouseOver(){
            IsMouseOver = true;
        }
        void OnMouseExit(){
            IsMouseOver = false;
        }

        void OnMouseDown()
        {
            if(!ParentCubie.SelectCubie() || GameManager.Instance.CurrentState != GameManager.EGameStates.IN_GAME){
                return;
            }
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            /*
            // If we didn't hit anything, try again next frame.
            if (!Physics.Raycast(ray, out RaycastHit hit))
                return;
            Debug.Log(hit.normal);
            */

            

            _mouseZ = Camera.main.WorldToScreenPoint( gameObject.transform.position).z;
            _dragStart = Input.mousePosition;
            _offset = gameObject.transform.position - GetMouseAsWorldPoint(_dragStart.Value);

            _cubieNeighbours = ParentCubie.GetNeighbours();
        }
        float _dragModifier = 1;
        void OnMouseDrag()
        {
            if(_dragStart == null){
                return;
            }

            switch(_mouseDirection){
                case EMouseDirection.NONE:
                _mouseDirection = EMouseDirection.Y;
                _dragDistance = Input.mousePosition.y - _dragStart.Value.y;
                if(Mathf.Abs(Input.mousePosition.x - _dragStart.Value.x) > _dragDistance){
                    _mouseDirection = EMouseDirection.X;
                    _dragDistance = Input.mousePosition.x - _dragStart.Value.x;
                }

                if(Mathf.Abs(_dragDistance) < ParentCubie.ParentCube.DragTreshold){
                    _mouseDirection = EMouseDirection.NONE;
                }
                break;
                case EMouseDirection.X:
                    _dragDistance = Input.mousePosition.x - _dragStart.Value.x;
                break;
                case EMouseDirection.Y:
                    _dragDistance = Input.mousePosition.y - _dragStart.Value.y;
                break;
            }
            if( ParentCubie.ParentCube.GetCubeState() != RubixCube.ECubeState.IDLE
                || _mouseDirection == EMouseDirection.NONE
                || _cubieNeighbours == null
                || _cubieNeighbours.Count <= 0){
                    if(ParentCubie.ParentCube.GetCubeState() == RubixCube.ECubeState.MANUAL){
                        ParentCubie.ParentCube.ManualRotate(_dragDistance * _dragModifier); 
                    }
                    return;
            }

            /*
            _closestCubie = _cubieNeighbours[0];
            _mouseWorldPoint = GetMouseAsWorldPoint(Input.mousePosition) + _offset;
            for(int i = 1; i < _cubieNeighbours.Count; i++){
               if(Vector3.Distance(_cubieNeighbours[i].transform.position, _mouseWorldPoint) < 
                   Vector3.Distance(_closestCubie.transform.position, _mouseWorldPoint))
                {
                    _closestCubie = _cubieNeighbours[i];
                    break;
                }
            }
            */
            _closestCubie = null;
            for(int i = 0; i < _cubieNeighbours.Count; i++){
                if(_cubieNeighbours[i].IsMouseOver())
                {
                    _closestCubie = _cubieNeighbours[i];
                    break;
                }
            }
            if(_closestCubie == null){
                return;
            }

            _dragModifier = 1;
            // Have same X and Y
            if(_closestCubie.Index.x == ParentCubie.Index.x && _closestCubie.Index.y == ParentCubie.Index.y){
                if(!ParentCubie.IsEdge(RubixCube.ERubixAxis.Y)){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.y, RubixCube.ERubixAxis.Y, true); //zDiff > 0);
                    if(!IsClockwise(RubixCube.ERubixAxis.Y, RubixCube.ERubixAxis.Y)){
                        _dragModifier = -1;
                    }
                }else{
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.x, RubixCube.ERubixAxis.X, true); //zDiff > 0);
                }
            }
            // Have same Y and Z
            else if(_closestCubie.Index.y == ParentCubie.Index.y && _closestCubie.Index.z == ParentCubie.Index.z){
                if(!ParentCubie.IsEdge(RubixCube.ERubixAxis.Y)){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.y, RubixCube.ERubixAxis.Y, true); //xDiff > 0);
                    if(!IsClockwise(RubixCube.ERubixAxis.Y, RubixCube.ERubixAxis.Y)){
                        _dragModifier = -1;
                    }
                }else{
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.z, RubixCube.ERubixAxis.Z, true); //-xDiff > 0);
                }
            }
            // Have same X and Z
            else if(_closestCubie.Index.x == ParentCubie.Index.x && _closestCubie.Index.z == ParentCubie.Index.z){
                if(ParentCubie.IsEdge(RubixCube.ERubixAxis.X)){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.z, RubixCube.ERubixAxis.Z, true); //-yDiff  > 0);
                    if(!IsClockwise(RubixCube.ERubixAxis.Z, RubixCube.ERubixAxis.X)){
                        _dragModifier = -1;
                    }
                }else if(ParentCubie.IsEdge(RubixCube.ERubixAxis.Z)){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.x, RubixCube.ERubixAxis.X, true); //(ParentCubie.Index.z == 0 ? -yDiff : yDiff) > 0);
                }
            }
                ParentCubie.ParentCube.ManualRotate(0f);

        }
        private bool IsClockwise(RubixCube.ERubixAxis axis, int selfIndex, int neighbourIndex, RubixCube.ERubixAxis edgeAxis){
            bool result = true;
            switch(edgeAxis){
                case RubixCube.ERubixAxis.X:
                    if(axis == RubixCube.ERubixAxis.Y){
                        // y:1 -> y:2  (clockwise)
                        result = neighbourIndex - selfIndex > 0;
                    }else if (axis == RubixCube.ERubixAxis.Z){
                        // z:1 -> z:2  (anticlockwise)
                        result = neighbourIndex - selfIndex < 0;
                    }
                    break;
                case RubixCube.ERubixAxis.Y:
                    if(axis == RubixCube.ERubixAxis.X){
                        // x:1 -> x:2  (anticlockwise)
                        result = neighbourIndex - selfIndex < 0;
                    }else if (axis == RubixCube.ERubixAxis.Z){
                        // z:1 -> z:2  (clockwise)
                        result = neighbourIndex - selfIndex > 0;
                    }
                    break;
                case RubixCube.ERubixAxis.Z:
                    if(axis == RubixCube.ERubixAxis.X){
                        // x:1 -> x:2  (clockwise)
                        result = neighbourIndex - selfIndex > 0;
                    }else if (axis == RubixCube.ERubixAxis.Y){
                        // y:1 -> y:2  (anticlockwise)
                        result = neighbourIndex - selfIndex < 0;
                    }
                    break;

            }
            if(ParentCubie.IsTop(edgeAxis)){
                return result;
            }

            return !result;
        }
        private bool IsClockwise(RubixCube.ERubixAxis axis, RubixCube.ERubixAxis edgeAxis){
            return IsClockwise(axis, ParentCubie.Index.x, _closestCubie.Index.x, edgeAxis);
        }

        void OnMouseUp(){
            ResetValues();
            if(ParentCubie.ParentCube.GetCubeState() == RubixCube.ECubeState.MANUAL){
                ParentCubie.ParentCube.TriggerAutoRotate();
            }
        }

        Vector2 ScreenDirection(Vector2 screenPoint, Vector3 worldPoint, Vector3 worldDirection) {
            Vector2 shifted = Camera.main.WorldToScreenPoint(worldPoint + worldDirection);

            return (shifted - screenPoint).normalized;
        }

        float GetDragDistance(Vector2 dragStart, Vector2 dragCurrent, Vector2 dragDirection) {
            return Vector2.Dot(dragCurrent - dragStart, dragDirection);
        }

        /*
        public RubixCube.ECubeFace GetCubeFace(){

        }
        */
        
        #endregion Mouse Events
        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if(!ParentCubie.IsCubieSelected() || _dragStart == null){
                return;
            }

            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawLine(Camera.main.ScreenToWorldPoint(_dragStart.Value),
                                            Camera.main.ScreenToWorldPoint(Input.mousePosition));
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}