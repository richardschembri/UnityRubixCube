using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityRubixCube.Managers;

namespace UnityRubixCube {
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        private GameManager _targetGameManager;
        void OnEnable()
        {
            _targetGameManager = (GameManager)target;
        }


        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            
            if (!_targetGameManager.DebugMode || !Application.isPlaying)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug:", EditorStyles.largeLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(_targetGameManager.CurrentState);
            EditorGUI.EndDisabledGroup();
        }
    }
}