using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Ball : MonoBehaviour
{
    [SerializeField] Slider ForceSlider;        //zaufaj, tak trzeba bo unity nas nie lubi ~ Przemysław
    [SerializeField] Slider MassSlider;
    [SerializeField] LaunchButtonController launchButtonController;

    [SerializeField] private float mass;
    [SerializeField] private float friction;
    [SerializeField] public float radius;
    private Vector3 EarthAngularVelocity=new Vector3(0.0f,0.0000727f,0f);


    Vector3 linearDirection = new Vector3(0, 0, 1);
    [SerializeField] private float initialForce = 0;
    private Vector3 distanceFromAxes = new Vector3(0.0f,0.0f,0.0f);
    public Vector3 linearVelocity = new Vector3(0.0f,0.0f,0.0f);
    public Vector3 angularVelocity = new Vector3(0.0f,0.0f,0.0f);
    public Vector3 linearAcceleration = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 angularAcceleration = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 deviation = new Vector3(0f,0f,0f);
    [SerializeField] private Vector3 forcePosition = new Vector3(0.0f,0.0f,0.0f); // tak naprawde raduius vectorowo
    [SerializeField] private MarkerControler markercontroler;  // tak naprawde raduius vectorowo
    private Vector3 force = new Vector3(0.0f,0.0f,0.0f); // odwrotnosc wektora pozycji
    private Vector3 FrictionForce = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 MagnusForce = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 coriolisForce = new Vector3(0.0f,0.0f,0.0f);
    public Vector3 ResultForce = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 OporForce = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 collisionForce= new Vector3(0.0f,0.0f,0.0f);
    private Vector3 gravityForce= new Vector3(0.0f,0.0f,0.0f);
    private Vector3 Dragmomentum= new Vector3(0.0f,0.0f,0.0f);
    private Vector3 ArchimedesForce= new Vector3(0.0f,0.0f,0.0f);
    private Vector3 frictionForce = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 lastfrictionForce = new Vector3(0.0f, 0.0f, 0.0f);
    private float interwalsymulacji;
    private DateTime deltatime=new DateTime();
    public float cd = 0.2f;
    public float cm = 0.2f;
    public float S = 0.2f;
    
    //stale
    private const float forceTime = 0.012f;

    private float timer = 0.01f;
        //zderzenie
            private const float damping = 15.176f; //c  N*s/m
            private const float stifftness = 36833f; // k N/m
        //wspolczynniki
            private const float kd = 0.00622f;
            private const float km = 0.00622f;
        //grawitacja
            private const float g = 9.81f;
    //boole 
    public bool onAir;
            public bool isKicked = false;
            public bool isCalculatedCollision;
            public bool isGoal;
            public bool isCollision=false;
            public bool isOnground= false;
            public float A;
            
           
    void Start()
    {
        distanceFromAxes = Vector3.zero;
        linearVelocity = Vector3.zero;
        angularVelocity = Vector3.zero;
        linearAcceleration = Vector3.zero;
        angularAcceleration = Vector3.zero;
        forcePosition = Vector3.zero;
        force = Vector3.zero;
        FrictionForce = Vector3.zero;
        MagnusForce = Vector3.zero;
        coriolisForce = Vector3.zero;
        ResultForce = Vector3.zero;
        OporForce = Vector3.zero;
        collisionForce = Vector3.zero;
        gravityForce = Vector3.zero;
        Dragmomentum = Vector3.zero;
        ArchimedesForce = Vector3.zero;
        frictionForce = Vector3.zero;
        isKicked = false;
        isCollision = false;
        isOnground = false;
    }

   private float simulationInterval = 0.02f; 
   private float ti = 0.0f;

    void FixedUpdate()
    {
        if (isKicked == true)
        {
           
           
                
                if (isCollision)
                {
                    
                        calculateResultForce();
                        calculateAcceleration();
                        calculatePositions(simulationInterval);
                        calculateVelocities(simulationInterval);
                        
                        
                        isCollision = false;
                }
                else
                {
               
                    calculateResultForce();
                    calculateAcceleration();
                    calculatePositions(simulationInterval);
                    calculateVelocities(simulationInterval);

                }
                resetFroces();
                
            
        }
    }

  

    
   
    public void calculate_Initial_Velocities()
    {
        
        Vector3 forceDirection = -forcePosition;

        force.x = forceDirection.x / forceDirection.magnitude;
        force.y = forceDirection.y / forceDirection.magnitude;
        force.z = forceDirection.z / forceDirection.magnitude;
        force = force * initialForce;
        linearVelocity = (force * forceTime) / mass;


        angularVelocity = Vector3.Cross(linearVelocity, linearDirection);
        float value = initialForce*radius *forceTime * 5.0f / (2.0f * mass * radius * radius); 
      


    } 

    
    public void calculateArchimedesForce()
    {
        float value = (0.4f * Mathf.PI * radius * radius * radius * 1.2f);
        ArchimedesForce = new Vector3(0, value, 0);

    }

    public void calculateAcceleration()
    {
        calculateAngularAcceleration();
        calcualtelinearAcceleration();
    }
    public void calculateVelocities(float dt)
    {
        calculateAngularVelocity(dt);
        calculateLinearVelocity(dt);
    }

    public void calculatePositions(float dt)
    {
        calculatePosition(dt);
        calculateRotation(dt);
    }
    public void calculateRotation(float dt)
    {
            Vector3 Rotation = (angularVelocity) * dt + (angularAcceleration) * dt * dt / 2;
            GetComponent<Transform>().Rotate(-Rotation);
         
    }
    public void calcualtelinearAcceleration()
    {
        
            linearAcceleration = ResultForce/mass;
        

    }
    public void calcualteCoriolisFoce()
    {
        coriolisForce = 2 * mass * Vector3.Cross(EarthAngularVelocity , linearVelocity);
    }

    public void calculateGravityForce()
    {
        gravityForce  = new Vector3(0, -g, 0) * mass;
    }

    public void calculateMomentumDrag()
    {
        Dragmomentum = (angularVelocity.normalized * ((float)linearVelocity.magnitude) * ((float)linearVelocity.magnitude)*1.2f * A * 0.5f *0.05f);

    }
    
    public void calculateResultForce()
    {
        
        if (isOnground)
        {
            gravityForce = Vector3.zero;
            coriolisForce = Vector3.zero;
            MagnusForce = Vector3.zero;
            
            ArchimedesForce = Vector3.zero;
            calculateFrictionForce();


        }
        else
        {

            calculateMagnusForce();
            calcualteCoriolisFoce();
            calculateGravityForce();
            calculateResistanceForce();
            calculateArchimedesForce();
            FrictionForce = Vector3.zero;
            calculateMomentumDrag();
            

        }

        ResultForce = MagnusForce+coriolisForce + gravityForce + OporForce + FrictionForce + ArchimedesForce;
       


    }
    public void calculateMagnusForce()
    {
        calcualtecm();
        Vector3 forceDirection = Vector3.Cross(angularVelocity,linearVelocity);
        forceDirection=forceDirection.normalized;
        float magnusForceMagnitude = (cm*0.1f*1.2f*linearVelocity.magnitude*linearVelocity.magnitude*A)/2.0f;
        MagnusForce = forceDirection * magnusForceMagnitude;
        MagnusForce.y = 0;

    }
    public void calculateResistanceForce()
    {
      calcualtecd();
        float value = (cd*0.1f*1.2f*linearVelocity.magnitude*linearVelocity.magnitude*(3.14f*(2.0f*radius)*(2.0f*radius)/4.0f))/2.0f;
        Vector3 forceDirection = -linearVelocity.normalized;

        OporForce =forceDirection* value;
    }

    public void calcualtecm()
    {
        float tester = angularVelocity.magnitude*radius/linearVelocity.magnitude;
        if (tester <= 0.1f)
        {
            cm = 0.1f;
        }
        else if (tester <= 0.2f)
        {
            cd = -0.1f;
        }
        else if (tester <= 0.3f)
        {
            cd = 0.15f;
        }
        
    }
    public void calcualtecd()
    {
        float tester = linearVelocity.magnitude;
        if (tester <= 5.0f)
        {
            cd = 0.2f;
        }
        else if (tester <= 10.0f)
        {
            cd = 0.2f;
        }
        else
        {
            cd = 0.2f;
        }
        
    }
    public void calculateFrictionForce()
    {
        float frictionForceMagnitude = mass * g * friction;
        Vector3 frictionForceDirection = -linearVelocity.normalized;
        frictionForceDirection.y = 0;
        frictionForce = frictionForceDirection * frictionForceMagnitude;
    
        linearVelocity *= 0.95f;
        angularVelocity *= 0.95f;

        if (linearVelocity.magnitude <0.000001f)
        {
            linearVelocity = Vector3.zero;
            angularVelocity = Vector3.zero;
            angularAcceleration = Vector3.zero;
        }

       
    }

    public void resetFroces()
    {
        collisionForce = Vector3.zero;
        coriolisForce = Vector3.zero;
        MagnusForce = Vector3.zero;
        FrictionForce = Vector3.zero;
        ResultForce= Vector3.zero;
    }
    public void calculatePosition(float dt)
    {
        Vector3 translations = new Vector3();
        translations.x = linearVelocity.x * dt + linearAcceleration.x * ((dt * dt) / 2);
        translations.y = linearVelocity.y * dt + linearAcceleration.y * ((dt * dt) / 2);
        translations.z = linearVelocity.z * dt + linearAcceleration.z * ((dt * dt) / 2);
        if (linearVelocity.magnitude < 1f)
        {
            linearVelocity = Vector3.zero;
        }
        bool isTouchingGround = ((this.transform.position.y <= 0.01f && translations.y<=0.01f) || (this.transform.position.y <= 0.01f && this.transform.position.y+translations.y <= 0.01f));

        if (isTouchingGround) 
        {
            deviation.y = Mathf.Abs(translations.y * 0.1f);
        
            if (isCollision)
            {
                this.transform.position = new Vector3(this.transform.position.x + translations.x, 0, this.transform.position.z + translations.z);
                isOnground = true;
            }
            else
            {
                isOnground = true; 
                this.transform.position = new Vector3(this.transform.position.x + translations.x, 0, this.transform.position.z + translations.z);

                isCollision = true; 
            }
        }
        else
        {
            if (isCollision) 
            {
                isCollision = false;
            }
            isOnground = false;
            this.transform.position += translations;
        }
    }

    

    public void calculateLinearVelocity(float dt)
    {
        linearVelocity.x = linearVelocity.x+ linearAcceleration.x * dt;
        linearVelocity.z = linearVelocity.z+ linearAcceleration.z * dt;

        
        if(!isCollision)
        {
            linearVelocity.y = linearVelocity.y + linearAcceleration.y * dt;
        }
        else
        {
            if (Mathf.Abs(linearVelocity.y) < 1f)
                {
                    linearVelocity.y = 0;
                
                }
                else
                {
                    linearVelocity.y = -linearVelocity.y * 0.71f;
                    
               
                }
        }
        if (linearVelocity.magnitude < 1f)
        {
            linearVelocity = Vector3.zero;
            angularVelocity = Vector3.zero;
            angularAcceleration = Vector3.zero;
            isKicked = false;
        }

    }
    public void calculateAngularVelocity(float dt)
    {
        angularVelocity += angularAcceleration * dt;
        if (linearVelocity.Equals(Vector3.zero)) angularVelocity = Vector3.zero;
    }
    public void calculateAngularAcceleration()
    {
        
        if (isOnground)
        {
            Dragmomentum = Vector3.zero;
            // M/I I =2/5 mr^2
            Vector3 helperRadiusVector = this.GetComponent<SphereCollider>().center - new Vector3(0, -radius, 0);
            
            angularAcceleration += Vector3.Cross(helperRadiusVector,frictionForce)*(5/2)/mass/radius/radius;
           
        }
        else
        {
            Vector3 helperRadiusVector = this.GetComponent<SphereCollider>().center - new Vector3(0, -radius, 0);
            angularAcceleration = Dragmomentum*(5/2)/mass/radius/radius;
         


        }
    }
    public void Launch()
    {
        mass = MassSlider.value * 0.001f;
        gravityForce = new Vector3(0, -g, 0) * mass;
        initialForce = ForceSlider.value; //zaufaj ~Przemysław
        forcePosition = markercontroler.InitialPosition;
        linearDirection = markercontroler.ForceVector.normalized;

        calculate_Initial_Velocities();
        A = (3.14f * (2.0f * radius) * (2.0f * radius) / 4.0f);
        isKicked = true;
        launchButtonController.Hited = true;

    }

    public void ResetValues()
    {
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        distanceFromAxes = Vector3.zero;
        linearVelocity = Vector3.zero;
        angularVelocity = Vector3.zero;
        linearAcceleration = Vector3.zero;
        angularAcceleration = Vector3.zero;
        forcePosition = Vector3.zero;
        force = Vector3.zero;
        FrictionForce = Vector3.zero;
        MagnusForce = Vector3.zero;
        coriolisForce = Vector3.zero;
        ResultForce = Vector3.zero;
        OporForce = Vector3.zero;
        collisionForce = Vector3.zero;
        gravityForce = Vector3.zero;
        Dragmomentum = Vector3.zero;
        ArchimedesForce = Vector3.zero;
        frictionForce = Vector3.zero;
        isKicked = false;
        isCollision = false;
        isOnground = false;
        launchButtonController.Hited = false;

    }

}
