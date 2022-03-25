using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using UnityEngine.Experimental.GlobalIllumination;

using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]

// This system updates all entities in the scene with both a RotationSpeed_IJobChunk and Rotation component.

// ReSharper disable once InconsistentNaming
public class EnemyRotationSystem : SystemBase
{
    EntityQuery m_Group;

    protected override void OnCreate()
    {
        // Cached access to a set of ComponentData based on a specific query
    
        m_Group = GetEntityQuery(
            
            typeof(Rotation),
            typeof(Translation), 
            typeof(PhysicsVelocity),
            typeof(LocalToWorld), 
            ComponentType.ReadOnly<EnemyRotation>());
    
    }

    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    [BurstCompile]
    struct RotationSpeedJob : IJobChunk
    {
        public float DeltaTime;
        public ComponentTypeHandle<Rotation> rt;
        public ComponentTypeHandle<PhysicsVelocity> pv;
        public ComponentTypeHandle<Translation> tr;
        public ComponentTypeHandle<LocalToWorld> ltw;

        [ReadOnly] public ComponentTypeHandle<EnemyRotation> major;
        
        [WriteOnly] public EntityCommandBuffer.ParallelWriter cb;

        public void Execute(ArchetypeChunk chunk, 
            int chunkIndex, int firstEntityIndex)
        {
            var chunkRotations = chunk.GetNativeArray(rt);
            var chunkVel = chunk.GetNativeArray(pv);
            var chunkTr = chunk.GetNativeArray(tr);
            var chunkLtw = chunk.GetNativeArray(ltw);
            var majorvariables = chunk.GetNativeArray(major);
            
            for (var i = 0; i < chunk.Count; i++)
            {
                var rotation = chunkRotations[i];
                var velo = chunkVel[i];
                var trans = chunkTr[i];
                var lo=chunkLtw[i];
                var frommajor = majorvariables[i];

                //var diff = frommajor.pos - new Vector3(trans.Value.x, trans.Value.y, trans.Value.z);
                var diff = frommajor.pos - 
                    new Vector3(trans.Value.x, trans.Value.y, trans.Value.z);
                //float3 fl = lo.Position;

                //var diff2 = frommajor.pos - new Vector3(fl.x, fl.y, fl.z);


                var rr = Quaternion.Slerp
                    (rotation.Value, Quaternion.LookRotation(frommajor.pos - new Vector3
                    (trans.Value.x, trans.Value.y, trans.Value.z)), 1.0f);

                var yy = new float3(0,0,0);

                //float3 diffenemrot = frommajor.centerforrot - new Vector3(trans.Value.x,
                //                  trans.Value.y, trans.Value.z);
                
                float3 diffenemrot = frommajor.pos- 
                    new Vector3(trans.Value.x,
                                  trans.Value.y, trans.Value.z);

                float3 velpre = velo.Linear;

                float distSqrd = math.lengthsq(diffenemrot);

                    var xx= (diffenemrot / math.sqrt(distSqrd));

                    yy =  velpre + xx;
                //yy = yy / 2;
                

                // Rotate something about its up vector at the speed given by RotationSpeed_IJobChunk.

                //if (diff.x > 0 && diff.x < 10 || diff.x < 0 && diff.x > -10 || 
                 //   diff.z > 0 && diff.z<10 || diff.z < 0 && diff.x > -10) {
                    chunkRotations[i] = new Rotation
                    {

                        Value = rr

                    };
                //}

                chunkVel[i] = new PhysicsVelocity
                {

                    Linear = yy

                };

                
              
            
            }
        }
    }

    // OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        // Explicitly declare:
        // - Read-Write access to Rotation
        // - Read-Only access to RotationSpeed_IJobChunk
        var rotationType = GetComponentTypeHandle<Rotation>();
        var physicsvel= GetComponentTypeHandle<PhysicsVelocity>();

        var frommajor = GetComponentTypeHandle<EnemyRotation>();
        var trans = GetComponentTypeHandle<Translation>();
        var ltw2 = GetComponentTypeHandle<LocalToWorld>();

        var job = new RotationSpeedJob()
        {
            rt = rotationType,
            pv = physicsvel,
            tr = trans,
            major = frommajor,
            ltw = ltw2,
            DeltaTime = Time.DeltaTime
        };

        Dependency = job.Schedule(m_Group, Dependency);
    }
}
