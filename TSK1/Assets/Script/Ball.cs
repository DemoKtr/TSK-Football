using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    [SerializeField] Slider ForceSlider;        //zaufaj, tak trzeba bo unity nas nie lubi ~ Przemysław
    [SerializeField] Slider MassSlider;

    [SerializeField] private float mass;
    [SerializeField] private float friction;
    [SerializeField] public float radius;
    private Vector3 EarthAngularVelocity=new Vector3(0.0f,0.0000727f,0f);
    


    [SerializeField] private float initialForce = 0;
    private Vector3 distanceFromAxes = new Vector3(0.0f,0.0f,0.0f);
    public Vector3 linearVelocity = new Vector3(0.0f,0.0f,0.0f);
    public Vector3 angularVelocity = new Vector3(0.0f,0.0f,0.0f);
    public Vector3 linearAcceleration = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 angularAcceleration = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 deviation = new Vector3(0.005f,0.05f,0.05f);
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
    private float interwalsymulacji;
    private DateTime deltatime=new DateTime();
    public float cd = 0.2f;
    
    //stale
    private const float forceTime = 0.01f;

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
            
           
            // Start is called before the first frame update
    void Start()
    {
        
        
        
    }

   private float simulationInterval = 0.01f; // Ustaw interwał symulacji na 0.1 sekundy
   private float ti = 0.0f;

    void Update()
    {
        if (isKicked == true)
        {

            ti += Time.deltaTime; // Zwiększ timer o czas trwania jednej klatki
            if (ti >= simulationInterval)
        {
           
            
            /*
           if (isCollision)
           {
               if (!isCalculatedCollision)
               {
                   calculateColisionForce();
                   ResultForce = collisionForce;
                   isCalculatedCollision = true;
               }
               isCollision = false;
           }
           else
           {
               
               
           }*/
            //resetFroces();

           
            calculateResultForce();
            calculateAcceleration();
            calculatePositions(ti);
            calculateVelocities(ti);
           
            
            
            resetFroces();


            
            ti = 0.0f; 
        }
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
       
        
        
        distanceFromAxes = new Vector3(
            Mathf.Sqrt(force.z * force.z + force.y * force.y),
            Mathf.Sqrt(force.z * force.z + force.x * force.x),
            Mathf.Sqrt(force.x * force.x + force.y * force.y));
        force.x = forceDirection.x / forceDirection.magnitude;
        force.y = forceDirection.y / forceDirection.magnitude;
        force.z = forceDirection.z / forceDirection.magnitude;
        force = force * initialForce;

        angularVelocity += Vector3.Cross(forcePosition, force)* forceTime * 5 / (2 * mass * radius * radius); 
        
       
    } 

    public void calculateColisionForce()
    {
        //x y z to wektor odkształcenia
        
        
        collisionForce.x = damping * linearVelocity.x + stifftness * deviation.x + kd * linearVelocity.x * linearVelocity.x;
        collisionForce.y = damping * linearVelocity.y + stifftness * deviation.y + kd * linearVelocity.x * Mathf.Abs(linearVelocity.x);
        collisionForce.z = damping * linearVelocity.z + stifftness * deviation.z + kd * linearVelocity.z * linearVelocity.z;
        Vector3 l1 = linearVelocity;
        l1.y *= -1;
        l1.x = l1.x / l1.magnitude;
        l1.y = l1.y / l1.magnitude;
        l1.z = l1.z / l1.magnitude;
        collisionForce.x *= l1.x;
        collisionForce.y *= l1.y;
        collisionForce.z *= l1.z;

        linearVelocity= (collisionForce * forceTime) / mass;
        
        
        //linearVelocity += linearVelocityhelper;


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
        Vector3 Rotation = angularVelocity * dt;
        GetComponent<Transform>().Rotate(Rotation);
    }
    public void calcualtelinearAcceleration()
    { 
        // Ten co to robil ma tu zajrzec
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
    
    public void calculateResultForce()
    {
        
        //calculateFrictionForce();
        if (isOnground)
        {
            //this.GetComponent<SphereCollider>().center.Set(this.GetComponent<SphereCollider>().center.x,0,this.GetComponent<SphereCollider>().center.z);
            gravityForce = Vector3.zero;
            coriolisForce = Vector3.zero;
            MagnusForce = Vector3.zero;
            OporForce = Vector3.zero;
            calculateFrictionForce();
           
        }
        else
        {
            calculateMagnusForce();
            calcualteCoriolisFoce();
            calculateGravityForce();
            calculateResistanceForce();
            FrictionForce = Vector3.zero;


        }
        
        ResultForce =   MagnusForce+coriolisForce + gravityForce + OporForce + FrictionForce;
        Debug.Log("magnusFoce "+ MagnusForce);
        Debug.Log("velocity " + linearVelocity);



    }
    public void calculateMagnusForce()
    {
        Vector3 forceDirection = (linearVelocity);
        
        Vector3 w = new Vector3(-forceDirection.y, forceDirection.x, 0);
        w = w.normalized;
        float magnusForceMagnitude = (0.2f*0.1f*1.2f*linearVelocity.magnitude*linearVelocity.magnitude*(3.14f*(2.0f*radius)*(2.0f*radius)/4.0f))/2.0f;
        MagnusForce = w * magnusForceMagnitude;
    }
    public void calculateResistanceForce()
    {
      calcualtecd();
        float value = (cd*0.1f*1.2f*linearVelocity.magnitude*linearVelocity.magnitude*(3.14f*(2.0f*radius)*(2.0f*radius)/4.0f))/2.0f;
        Vector3 forceDirection = -linearVelocity.normalized;

        OporForce =forceDirection* value;
    }

    public void calcualtecd()
    {
        float tester = linearVelocity.magnitude;
        if (tester <= 10)
        {
            cd = 0.21f;
        }
        else if (tester <= 12.5)
        {
            cd = 0.21f;
        }
        else
        {
            cd = 0.2f;
        }
        
    }
    public void calculateFrictionForce()
    {
        //kierunek tej siły
        float FrictionForceMagnitude = mass * g * friction;
        FrictionForce = new Vector3((linearVelocity.x/linearVelocity.magnitude), (linearVelocity.y/linearVelocity.magnitude), (linearVelocity.z/linearVelocity.magnitude));
        FrictionForce = -FrictionForce * FrictionForceMagnitude;
        
    }

    public void resetFroces()
    {
        coriolisForce = Vector3.zero;
        MagnusForce = Vector3.zero;
        FrictionForce = Vector3.zero;
        ResultForce= Vector3.zero;
    }
    public void calculatePosition(float dt)
    {
        Vector3 translations = new Vector3();
          translations.x =  linearVelocity.x * dt + linearAcceleration.x * ((dt*dt)/ 2);
          translations.y =  linearVelocity.y * dt + linearAcceleration.y * ((dt*dt)/ 2);
          translations.z =  linearVelocity.z * dt + linearAcceleration.z * ((dt*dt)/ 2);
        
        
        
        if (this.transform.position.y <= 0.0f && translations.y <= 0.0f) 
        {
            if (isCollision)
            {
                this.transform.position = new Vector3(this.transform.position.x+translations.x, 0, this.transform.position.z+translations.z);
                isOnground = true;
               
            }
            else
            {
                this.transform.position = new Vector3(this.transform.position.x+translations.x, 0, this.transform.position.z+translations.z);
                //calculateColisionForce();
                isCollision = true; 
            }
            
            
        }
        else
        {
            if (isCollision) isCollision = false;
            this.transform.position += translations;
            
        }

    }
   /*
    public void calculateRotation(float dt)
    {
        this.transform.Rotate(
            angularVelocity.x * dt + ((angularAcceleration.x*dt*dt)/2), 
            angularVelocity.y * dt + ((angularAcceleration.y*dt*dt)/2), 
            angularVelocity.z* dt + ((angularAcceleration.z*dt*dt)/2),
            Space.Self);
    }*/
    public void calculateLinearVelocity(float dt)
    {
        linearVelocity = linearVelocity + linearAcceleration * dt;
      
       
       
       
    }
    public void calculateAngularVelocity(float dt)
    {
        angularVelocity += angularAcceleration * dt;
    }
    public void calculateAngularAcceleration()
    {
        if (isOnground)
        {
            
           // angularAcceleration = (Vector3.Cross(FrictionForce, helperRadiusVector))/((5/2)*mass*radius*radius);
            // M/I I =2/5 mr^2
            Vector3 helperRadiusVector = this.GetComponent<SphereCollider>().center - new Vector3(0, -radius, 0);
            angularAcceleration = Vector3.Cross(FrictionForce ,helperRadiusVector)*(5/2)/mass/radius/radius;;
        }
        else
        {
            Vector3 helperRadiusVector = this.GetComponent<SphereCollider>().center - new Vector3(0, -radius, 0);
            angularAcceleration = Vector3.Cross(OporForce ,helperRadiusVector)*(5/2)/mass/radius/radius;
        }
    }
    public void Launch()
    {
        mass = MassSlider.value * 0.001f;
        gravityForce = new Vector3(0, -g, 0) * mass;
        initialForce = ForceSlider.value; //zaufaj ~Przemysław
        forcePosition = markercontroler.InitialPosition;
        calculate_Initial_Velocities();
        isKicked = true;


    }

}
