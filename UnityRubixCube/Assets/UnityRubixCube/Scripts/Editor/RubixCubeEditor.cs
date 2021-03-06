using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityRubixCube {
    [CustomEditor(typeof(RubixCube))]
    public class RubixCubeEditor : Editor
    {
        private RubixCube _targetRubixCube;
        private int _targetLayerIndex = 0;
        private RubixCube.ERubixAxis _targetAxis = RubixCube.ERubixAxis.X;

        void OnEnable()
        {
            _targetRubixCube = (RubixCube)target;
        }

        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            
            if (!_targetRubixCube.DebugMode || !Application.isPlaying)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug:", EditorStyles.largeLabel);
            EditorGUI.BeginDisabledGroup(_targetRubixCube.GetCubeState() != RubixCube.ECubeState.IDLE);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Generate Cube")){
                _targetLayerIndex = 0;
                _targetRubixCube.GenerateCube();
            }
            if(GUILayout.Button("Clear Cube")){
                _targetLayerIndex = 0;
                _targetRubixCube.ClearCube();
            }
            GUILayout.EndHorizontal();
            
            EditorGUI.BeginDisabledGroup(_targetRubixCube.CubiesCount() <= 0);
            _targetLayerIndex = (int)EditorGUILayout.Slider("Layer Index", _targetLayerIndex , 0f, _targetRubixCube.CubiesPerSide - 1);

            GUILayout.BeginHorizontal();
            _targetAxis = (RubixCube.ERubixAxis)EditorGUILayout.EnumPopup("Axis", _targetAxis);
            if (GUILayout.Button("<="))
            {
                if(_targetRubixCube.SetLayerMove(_targetLayerIndex, _targetAxis, false)){
                    _targetRubixCube.TriggerAutoRotate();
                }
            }
            if (GUILayout.Button("=>"))
            {
                if(_targetRubixCube.SetLayerMove(_targetLayerIndex, _targetAxis, true)){
                    _targetRubixCube.TriggerAutoRotate();
                }
            }

            GUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(!_targetRubixCube.HasMoves());

            if (GUILayout.Button("Undo Move"))
            {
                _targetRubixCube.UndoPlayerMove();
            }
            EditorGUI.EndDisabledGroup(); // HasMoves
            EditorGUI.EndDisabledGroup(); // Cube is Generated
            EditorGUI.EndDisabledGroup(); // Cube is IDLE
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(_targetRubixCube.GetCubeState());
            EditorGUI.EndDisabledGroup();
        }
    }
}