using System;
using UnityEngine;
using Unity.Entities;

// ReSharper disable once InconsistentNaming
//[GenerateAuthoringComponent]
[Serializable]
public struct EnemyRotation : IComponentData
{
    
    public Vector3 pos;

}
