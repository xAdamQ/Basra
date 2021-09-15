using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests
{
    public class MixedGeneralTests
    {
        private void LoadCamera()
        {
            var prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AppUnits/Camera.Prefab");
            Object.Instantiate(prefab);
        }
        private void LoadCanvas()
        {
            var prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AppUnits/Canvas.Prefab");
            Object.Instantiate(prefab);
        }
        private void LoadEventSystem()
        {
            var prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefabs/AppUnits/EventSystem.Prefab");
            Object.Instantiate(prefab);
        }

        /// <summary>
        /// for visual tests
        /// </summary>
        protected void LoadCore()
        {
            LoadCamera();
            LoadCanvas();
            LoadEventSystem();
        }
    }
}