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
        private RubixCube.Move.EMoveAxis _targetAxis = RubixCube.Move.EMoveAxis.X;

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
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Generate Cube")){
                _targetRubixCube.GenerateCube();
            }
            if(GUILayout.Button("Clear Cube")){
                _targetRubixCube.ClearCube();
            }
            GUILayout.EndHorizontal();
            
            EditorGUI.BeginDisabledGroup(_targetRubixCube.CubiesCount() <= 0);
            _targetLayerIndex = (int)EditorGUILayout.Slider("Layer Index", _targetLayerIndex , 0f, _targetRubixCube.CubiesPerSide - 1);

            GUILayout.BeginHorizontal();
            _targetAxis = (RubixCube.Move.EMoveAxis)EditorGUILayout.EnumPopup("Axis", _targetAxis);
            if (GUILayout.Button("<="))
            {
                _targetRubixCube.MoveLayer(_targetLayerIndex, _targetAxis, false);
            }
            if (GUILayout.Button("=>"))
            {
                _targetRubixCube.MoveLayer(_targetLayerIndex, _targetAxis, true);
            }

            GUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();
        }
    }
}