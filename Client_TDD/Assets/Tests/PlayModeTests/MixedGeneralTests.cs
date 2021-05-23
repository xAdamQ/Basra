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
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AppUnits/Camera.Prefab");
            Object.Instantiate(prefab);
        }
        private void LoadCanvas()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AppUnits/Canvas.Prefab");
            Object.Instantiate(prefab);
        }
        private void LoadEventSystem()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AppUnits/EventSystem.Prefab");
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

        [UnityTest]
        public IEnumerator TestBlockingPanel()
        {
            LoadCore();
            //load comp
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/blocking panel.Prefab");
            var p = Object.Instantiate(prefab, Object.FindObjectOfType<Canvas>().transform)
                .GetComponent<BlockingPanel>();
            //load it's deps

            yield return null;

            //test
            p.Show();
            yield return new WaitForSeconds(5);
            p.Hide();
            yield return new WaitForSeconds(3);

            p.Show("a test message");
            yield return new WaitForSeconds(8);
            p.Hide("dismiss button should appear");
            yield return new WaitForSeconds(8);
        }
    }
}