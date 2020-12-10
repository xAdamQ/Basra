using Unity.Entities;
using UnityEngine;

namespace Basra.Client
{
    public class LevelUpSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            //"on update" we will iterate throw this system entitites of type LevelComponent and act on them
            //1- whats "Entities" object and "EntityQueryBuilder" type and how we did what I said
            //the system worked interacting with the scene!!!!!!
            //you can use this here World.
            Entities.ForEach((ref Components.Level lc) =>
            {
                lc.Value += .001f;
            });
        }
    }
}