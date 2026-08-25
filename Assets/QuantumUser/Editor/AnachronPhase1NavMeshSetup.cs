namespace Anachron.Editor
{
    using Quantum;
    using Unity.AI.Navigation;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public static class AnachronPhase1NavMeshSetup
    {
        private const string GroundName = "AnachronNavMeshGround";
        private const string QuantumMapPath = "Assets/QuantumUser/Resources/QuantumMap.asset";
        private const string QuantumNavMeshPath = "Assets/QuantumUser/Resources/QuantumMap_AnachronQuantumNavMesh.asset";
        private const string QuantumNavMeshDataPath = "Assets/QuantumUser/Resources/QuantumMap_AnachronQuantumNavMesh_data.asset";

        [MenuItem("Tools/Anachron/Setup Phase 1 NavMesh Surface")]
        public static void SetupPhase1NavMeshSurface()
        {
            QuantumMapData mapData = Object.FindAnyObjectByType<QuantumMapData>();
            if (mapData == null)
            {
                Debug.LogError("No QuantumMapData found in the open scene.");
                return;
            }

            GameObject ground = GetOrCreateGround();
            ground.transform.SetParent(mapData.transform, true);

            QuantumMapNavMeshUnity quantumNavMesh = ground.GetComponent<QuantumMapNavMeshUnity>();
            if (quantumNavMesh == null)
            {
                quantumNavMesh = ground.AddComponent<QuantumMapNavMeshUnity>();
            }

            NavMeshSurface surface = ground.GetComponent<NavMeshSurface>();
            if (surface == null)
            {
                surface = ground.AddComponent<NavMeshSurface>();
            }

            surface.collectObjects = CollectObjects.All;
            surface.defaultArea = 0;
            quantumNavMesh.NavMeshSurfaces = new[] { ground };

            EditorUtility.SetDirty(ground);
            EditorUtility.SetDirty(quantumNavMesh);
            EditorUtility.SetDirty(mapData);
            EditorSceneManager.MarkSceneDirty(mapData.gameObject.scene);

            Debug.Log("Anachron Phase 1 NavMesh surface is ready. Next run Tools > Quantum > Bake > MapData with Unity NavMesh Import.");
        }

        [MenuItem("Tools/Anachron/Repair Phase 1 Quantum NavMesh Link")]
        public static void RepairPhase1QuantumNavMeshLink()
        {
            Map map = AssetDatabase.LoadAssetAtPath<Map>(QuantumMapPath);
            if (map == null)
            {
                Debug.LogError($"Could not load Quantum map at {QuantumMapPath}.");
                return;
            }

            NavMesh navMesh = AssetDatabase.LoadAssetAtPath<NavMesh>(QuantumNavMeshPath);
            if (navMesh == null)
            {
                Debug.LogError($"Could not load Quantum navmesh at {QuantumNavMeshPath}. Run Tools > Quantum > Bake > MapData with Unity NavMesh Import first.");
                return;
            }

            BinaryData navMeshData = AssetDatabase.LoadAssetAtPath<BinaryData>(QuantumNavMeshDataPath);
            if (navMeshData == null)
            {
                Debug.LogError($"Could not load Quantum navmesh data at {QuantumNavMeshDataPath}. Run Tools > Quantum > Bake > MapData with Unity NavMesh Import first.");
                return;
            }

            navMesh.Name = "AnachronQuantumNavMesh";
            navMesh.DataAsset = new AssetRef<BinaryData>(navMeshData.Identifier.Guid);
            map.NavMeshLinks = new AssetRef<NavMesh>[] { new AssetRef<NavMesh>(navMesh.Identifier.Guid) };
            map.Regions = new[] { "MainArea" };

            EditorUtility.SetDirty(navMesh);
            EditorUtility.SetDirty(map);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Anachron Phase 1 Quantum navmesh link repaired. Press Play again to verify runtime loading.");
        }

        private static GameObject GetOrCreateGround()
        {
            GameObject ground = GameObject.Find(GroundName);
            if (ground != null)
            {
                return ground;
            }

            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = GroundName;
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(8.0f, 1.0f, 8.0f);
            return ground;
        }

    }
}
