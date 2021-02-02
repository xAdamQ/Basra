using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Basra.Client.Components
{
    public struct MoveSpeed : IComponentData
    {
        public float Value;
    }
}
