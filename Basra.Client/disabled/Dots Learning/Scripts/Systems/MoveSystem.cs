using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace Basra.Client
{
    public class MoveSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Translation translation, ref Components.MoveSpeed moveSpeedComponent) =>
            {
                translation.Value.y += moveSpeedComponent.Value * Time.DeltaTime;
            });
        }
    }
}
