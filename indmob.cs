using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Random = Unity.Mathematics.Random;
using System;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Physics;
using System.Data.SqlTypes;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]


public class indmob : MonoBehaviour
{

    public GameObject playerobject;
    public GameObject playerobject2;

    public GameObject prefab;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;


    Vector3 movement1;
    //Vector3 targetposition;

    Random random;

    public ParticleSystem ps1;

    public float moveZ = 0.0f, moveY = 0.0f;
    
    public float rotationspeed, rotationfriction, rotationsmoothness;
    
    private float resultingvalue;
    
    private Quaternion rotatefrom, rotateto;

    private float resultingvalue1, resultingvalue2;

    Vector2 v2;
    Vector2 v2i;
    
    float calcu, calcu1;
    float calcu2, calcu3;

private CharacterController cc;

    float xx = 0.0f, yy = 0.0f;

    public AudioClip[] ac;

    private AudioSource asour;

    EntityManager entityManager;

    Entity entity, entity2, entity3, entity4;

    NativeArray<Entity> e2;
    NativeArray<Entity> e3;

    float aa;
    float bb;

    private void Awake()
    {
        
        Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        //Screen.SetResolution(800, 500, true);
        //Application.targetFrameRate = 60;
        //ps1.Stop();
        
        //BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        //buildPlayerOptions.options = BuildOptions.BuildScriptsOnly;
        //buildPlayerOptions.scenes = new[] { "Assets/scene.unity" };
        //buildPlayerOptions.locationPathName = "scriptBuilds";
        //buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        // use these options for the first build
        //buildPlayerOptions.options = BuildOptions.Development;
        //BuildPipeline.BuildPlayer(buildPlayerOptions);
    }


    protected void OnDestroy()
    {
        try
        {

            e2.Dispose();
            e3.Dispose();

        }
        catch (Exception ex)
        {

           
        }
    }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        asour = GetComponent<UnityEngine.AudioSource>();

        asour.clip = ac[0];
        asour.Play();

        var settings = GameObjectConversionSettings.FromWorld
(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
        
entity = GameObjectConversionUtility.ConvertGameObjectHierarchy
            (prefab, settings);

        entity2 = 
            GameObjectConversionUtility.ConvertGameObjectHierarchy
            (prefab2, settings);

        entity3 = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab3, settings);
        
        entity4 = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab4, settings);

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        entity = entityManager.Instantiate(entity);

        e2 = new NativeArray<Entity>(5000, Allocator.Persistent);
        
        e3 = new NativeArray<Entity>(5000, Allocator.Persistent);

        entityManager.Instantiate(entity2, e2); 
        entityManager.Instantiate(entity3, e3);

        entityManager.SetComponentData
            (entity, new Translation { Value = playerobject.transform.position });
        entityManager.SetComponentData
            (entity, new Rotation { Value = playerobject.transform.rotation});

        random = new Random(1);

        //er.pos = cam.transform.position;

        for (int i = 0; i < e3.Length; i++)
        {
            entityManager.SetComponentData(e3[i],

                new Translation
                {
                    Value = new Vector3(random.NextFloat(-12000, 12000),
                          random.NextFloat(-12000, 12000),
                          random.NextFloat(-12000, 12000))
                });

            entityManager.AddComponentData(e3[i],

                            new Scale
                            {

                                Value = random.NextFloat(10, 50)
                            
                            });



        }


        for (int i = 0; i < e2.Length; i++)
        {
            
            entityManager.SetComponentData(e2[i],
                      new Translation
                      {
                          Value = new Vector3(random.NextFloat(-500,500), random.NextFloat(10,40)
                      , random.NextFloat(-500, 500))
                      });


            entityManager.AddComponentData(e2[i],
                              new EnemyRotation
                              {
                                  pos = 
                                  new Vector3
                                  (
                                  playerobject2.transform.position.x, 
                                  playerobject2.transform.position.y,
                                  playerobject2.transform.position.z
                                  ), 
                                  
                              });
            //var data = new RotationSpeed_IJobChunk { };
        }
    }

    [BurstCompile]
    public void Update()
    {

        calcu = 0;
        calcu2 = 0;

        xx = Input.acceleration.x;
        yy = Input.acceleration.y;

        if (xx > 0.1f || Input.GetAxis("Horizontal") > 0)
        {

            resultingvalue1 += (0.5f) * rotationspeed * rotationfriction;

            if (resultingvalue2 > -45)
            {
                resultingvalue2 -= (5.0f) * rotationspeed * rotationfriction;
            }
        }
        if (xx < -0.1f || Input.GetAxis("Horizontal") < 0)
        {
            
            resultingvalue1 -= (0.5f) * rotationspeed * rotationfriction;

            if (resultingvalue2 < 45)
            {

            resultingvalue2 += (5.0f) * rotationspeed * rotationfriction;

            }
         }

        if (yy < -0.50f && yy!=0 || Input.GetAxis("Vertical") > 0)
        {
        
            if (resultingvalue < 45.0f) {
                resultingvalue += (0.5f) * rotationspeed * rotationfriction;
            }
        
        }
        
        if (yy > -0.50f && yy != 0 || Input.GetAxis("Vertical") < 0)
        {

            if (resultingvalue > -45.0f)
            {

                resultingvalue -= (0.5f) * rotationspeed * rotationfriction;
            
            }
        
        }
        
        if (resultingvalue2 > 0f)
        {
            resultingvalue2 -= 0.1f;
        }

        if (resultingvalue2 < 0f)
        {
            resultingvalue2 += 0.1f;
        }





        if (Input.touchCount > 0 && Input.touchCount < 2)
        {
            v2 = Input.GetTouch(0).position;
            calcu1 = v2.y; 
            calcu = v2.x;

        }

        if (Input.touchCount > 1 && Input.touchCount < 3)
        {


            v2i = Input.GetTouch(1).position;

            calcu2 = v2i.x; calcu3 = v2i.y;

        }



        if (calcu > (Screen.width / 2) && calcu1 > (Screen.height / 2)
            && calcu != 0 || calcu2 > (Screen.width / 2) && calcu3 >
            (Screen.height / 2 )
            && calcu2 != 0 || Input.GetKey("c"))
        {
            if (moveZ > 0 ) {

                moveZ -= 0.1f;

            }

            if (moveZ < 0)
            {

                moveZ += 0.1f;

            }




        }

        if (calcu > (Screen.width / 2) && calcu1 > (Screen.height / 2)
            && calcu != 0 || calcu2 > (Screen.width / 2) && calcu3 >
            (Screen.height / 2)
            && calcu2 != 0 || Input.GetKey("v"))
        {
            if (moveZ > -2)
            {
                moveZ -= 0.01f;
            }
        }

        if (calcu > (Screen.width / 2) && calcu1 < (Screen.height / 2)
            && calcu != 0 || calcu2 > (Screen.width / 2) && calcu3 <
            (Screen.height / 2) && calcu2 != 0 || Input.GetKey("x"))
        {

            moveY = 0.0f;

            if (moveZ < 5) {
                moveZ += 0.01f;
            }

           
        }

        if (calcu < (Screen.width / 2) && calcu != 0 &&
            calcu1 < (Screen.height / 2) || calcu2 < (Screen.width / 2) && calcu2 != 0 &&
            calcu3 < (Screen.height / 2) || Input.GetKey("b"))
        {
            resultingvalue2 = 0f;
        }


        aa += Time.deltaTime;


        if (aa > 0.25)
        {

            bb = 1;

        }

        
        movement1 = Vector3.zero;

        movement1.x = 0.0f;
        movement1.y = moveY;
        movement1.z = moveZ;

        movement1 = transform.TransformDirection(movement1);


        //moveZ = movement1.z;
        //transform.localPosition = movement1;

        cc.Move(new Vector3(movement1.x, movement1.y, movement1.z)*10);

        rotatefrom = transform.rotation;

        rotateto = Quaternion.Euler(resultingvalue, 
            resultingvalue1, resultingvalue2);
        
        cc.transform.localRotation = Quaternion.Lerp(rotatefrom, rotateto, Time.deltaTime * rotationsmoothness);

        entityManager.SetComponentData(entity, new Translation { Value = playerobject.transform.position });
        entityManager.SetComponentData(entity, new Rotation { Value = playerobject.transform.rotation });

        for (int i = 0; i < e2.Length; i++)
        {
            entityManager.SetComponentData(e2[i],
                              new EnemyRotation
                              {
                                  pos = new Vector3(playerobject2.transform.position.x,
                                  playerobject2.transform.position.y,
                                  playerobject2.transform.position.z)
                                   
                              });
        }




        if (calcu < (Screen.width / 2) && calcu != 0 &&
       calcu1 > (Screen.height / 2)
       || calcu2 < (Screen.width / 2) && calcu2 != 0 &&
       calcu3 > (Screen.height / 2) || Input.GetKey("z"))
        {

            var ep = entityManager.GetComponentData<LocalToWorld>(entity);

            var ep2 = entityManager.GetComponentData<Translation>(entity);

            //Vector3 ep1 = 

            if (bb == 1)
            {

                entityManager.Instantiate(entity4);

                entityManager.SetComponentData(entity4,new Translation { Value = 
                    new float3(playerobject.transform.position)+ 
                new float3(0,0,0.1f)});

                entityManager.AddComponentData(entity4,
                                  new PhysicsVelocity
                                  {
                                      Linear = playerobject.transform.TransformDirection(new Vector3(0, 0, 1000f)*10)
                                  });
                

                //Debug.Log(playerobject.transform.TransformDirection(new Vector3(0, 0, 0.2f)));

                /*-
            
                var rr = Quaternion.Slerp
                    (rot.Value, Quaternion.LookRotation(difftargetent), 1.0f);

                */
                aa = 0;
                bb = 0;

            }
            //entityManager.SetComponentData(entity, new Rotation { Value = playerobject.transform.rotation });
            
        }
        


        //entityManager.Instantiate(entity5);
        //entityManager.SetComponentData(entity5, 
        //  new Translation { 
        //    Value = playerobject.transform.position+ 
        // playerobject.transform.TransformDirection(new Vector3(0, 0, 10f))});


        var e = Raycaster(playerobject.transform.position +
                playerobject.transform.TransformDirection(new Vector3(0, 0, 1f)),
                playerobject.transform.position +
                playerobject.transform.TransformDirection(new Vector3(0, 0, 2f)));


        if (transform.position.z < -2000 ||
            transform.position.z > 2000 ||
            transform.position.x > 2000 ||
            transform.position.x < -2000)
        {


            Debug.Log(e);
            ps1.Play();

            //transform.rotation = new Quaternion(0,0,0,0);

            //StartCoroutine(waiter());


        }
        if (e > 0 && e < 4990 || e < 0 && e > -4990)
        {

            //Debug.Log(e);
            ps1.Play();

            //transform.rotation = new Quaternion(0,0,0,0);
          
            StartCoroutine(waiter());
        

        }


    }


    void OnTriggerEnter(UnityEngine.Collider col)
    {

        if (col.tag == "ementerr")

        {

        
        }

    }
    
    
    public float Raycaster(float3 RayFrom, float3 RayTo)
    {
        //World.DefaultGameObjectInjectionWorld


        var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.
            GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
        
        RaycastInput input = new RaycastInput()
        {
            Start = RayFrom,
            End = RayTo,
            Filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u, 
                // all 1s, so all layers, collide with everything
                GroupIndex = 0
            }
        };

        Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();
        bool haveHit = collisionWorld.CastRay(input, out hit);
        if (haveHit)
        {
            // hit.Position
             // hit.SurfaceNormal
            //Entity ef = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
            
            float e = hit.RigidBodyIndex;
            return e;
            //Debug.Log(e);
        
        
        }
        
        return 0;
    }

    IEnumerator waiter()
    {

        yield return new WaitForSecondsRealtime(3f);
                
        ps1.Stop();
        
        transform.position= new Vector3(0, 100, -1000);
        
        //transform.rotation = new Quaternion(0,0,0);
       // rotateto = Quaternion.Euler(0,0,0);
       // cc.transform.localRotation = Quaternion.Lerp(rotatefrom, rotateto, Time.deltaTime * rotationsmoothness);
    
    }

}

