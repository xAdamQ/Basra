using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;

namespace Basra.Client
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] Mesh Mesh;
        [SerializeField] Material Material;

        private void Start()
        {
            //there's an entity called world
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var archetype = entityManager.CreateArchetype
            (
                typeof(Components.Level),
                typeof(Translation),
                typeof(RenderBounds),
                typeof(RenderMesh),//not a component but a "shared component"
                typeof(LocalToWorld),//to view the entity, calculate how it should be visible
                typeof(Components.MoveSpeed)
            );//the entity archetype constains set of components

            // var entity = entityManager.CreateEntity(archetype);
            // entityManager.SetComponentData(entity, new LevelComponent { Value = 12 });

            //make and set multiple entities
            var nativeArr = new NativeArray<Entity>(1/*size*/, Allocator.Temp/*lifetime*/);
            entityManager.CreateEntity(archetype, nativeArr);//assign mutlipe components(archetypes) to multiple entities(array)

            for (var i = 0; i < nativeArr.Length; i++)
            {
                var entity = nativeArr[i];
                entityManager.SetComponentData(entity, new Components.Level { Value = UnityEngine.Random.Range(0, 100) });
                entityManager.SetComponentData(entity, new Components.MoveSpeed { Value = .1f });
                entityManager.SetComponentData(entity, new Translation { Value = new float3(UnityEngine.Random.Range(-4, 4), UnityEngine.Random.Range(-4, 4), 0) });
                entityManager.SetSharedComponentData(entity, new RenderMesh
                {
                    mesh = Mesh,
                    material = Material,
                });
            }//set values for each entity
            nativeArr.Dispose();
        }
    }
}