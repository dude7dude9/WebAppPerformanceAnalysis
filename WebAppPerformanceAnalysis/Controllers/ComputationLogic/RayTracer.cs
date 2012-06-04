using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Threading;

namespace WebAppPerformanceAnalysis.Controllers.ComputationLogic
{
    public class RayTracer
    {
        const int windowWidth = 1300;
        const int windowHeight = 1300;

        int workWidth;
        int workHeight;
        private static ManualResetEvent[] resetEvents;
        private const int NumThreads = 4;
        bool parallel;
        bool caching;

        // objects
        const int numObjects = 2;
        SceneObject[] sceneObjects = new SceneObject[numObjects];

        Color background = new Color(0.5f, 0.8f, 1f);

        // lights
        const int numLights = 2;
        Light[] lights = new Light[numLights];

        const int maxReflection = 10;

        // camera
        Vector eye =   new Vector(0, 0, 4);
        Vector u =     new Vector(1, 0, 0);
        Vector v =     new Vector(0, 1, 0);
        Vector n =     new Vector(0, 0, 1);
        float N = 1f, W = 0.5f, H = 0.5f;

        // Returning Pixel array
        int[][] pixelArray = new int[10][];

        public RayTracer()
        {
            //Scene object and light intialization

            for(int i=0; i<pixelArray.Length; i++){
                pixelArray[i] = new int[(windowHeight * windowWidth * 3) / 10];
            }

	        Sphere s = new Sphere();
	        s.ambient =    new Color(0.1f, 0.1f, 0.1f);
	        s.diffuse =    new Color(1.0f, 0.2f, 0.2f);
	        s.specular =   new Color(0.7f, 0.7f, 0.7f);
	        s.shininess =  50;
	        s.reflectivity = 0.4f;
	        sceneObjects[0] = (SceneObject)s;

	        Plane p = new Plane();
	        p.n = new Vector(0, 1, 0);
	        p.a = -1;
	        p.ambient =    new Color(0.1f, 0.1f, 0.1f);
	        p.diffuse =    new Color(0.2f, 0.5f, 0.2f);
	        p.specular =   new Color(0.7f, 0.7f, 0.7f);
	        p.shininess =  50;
	        p.reflectivity = 0.4f;
	        sceneObjects[1] = (SceneObject)p;

	        Light l = new Light();
	        l.position =   new Vector(-2, 2, 2);
	        l.ambient =    new Color(0.1f, 0.1f, 0.1f);
	        l.diffuse =    new Color(0.5f, 0.5f, 0.5f);
	        l.specular =   new Color(0.5f, 0.5f, 0.5f);
	        lights[0] = l;

	        l = new Light();
	        l.position =   new Vector(3, 1, 3);
	        l.ambient =    new Color(0.1f, 0.1f, 0.1f);
	        l.diffuse =    new Color(0.4f, 0.4f, 0.6f);
	        l.specular =   new Color(0.3f, 0.3f, 0.5f);
	        lights[1] = l;
        }


        Hit intersect(Vector source, Vector d)
        {
	        // initially hit scObject==NULL (no hit)
	        Hit hit = new Hit(source, d, -1f, null);

	        // for every scObject, check if ray hits it
	        for(int i=0; i<numObjects; i++) {
		        float t = sceneObjects[i].Intersect(source, d);

		        // 1. only use hits visible for the camera
		        // 2. only overwrite hit if either there is 
		        //     no hit yet, or the hit is closer
		        if(t>0.00001 && (hit.scObject==null || t<hit.t))
			        hit = new Hit(source, d, t, sceneObjects[i]);
	        }
	        return hit;
        }

        Color shade(Hit hit)
        {
	        // if no scObject was hit, return background color
	        if (hit.scObject == null)
		        return background;

	        Color color = new Color(0,0,0); 
            for(int i=0; i<numLights; i++){
		        // convenience for cleaner code
		        Vector liPos = lights[i].position;

                // ambient reflection 
                color = color + hit.scObject.ambient * lights[i].ambient; 
                Vector p = hit.HitPoint(); 
                Vector v = eye - p; 
        
		        Vector s = liPos - p;    
		        Vector m = hit.scObject.Normal(p); 
        
		        // make sure light hits the front face 
                if (Vector.Dot(s, m) < 0)  continue;

		        // cast a "shadow feeler" and find its closest hit position to light source
		        Hit feeler = intersect(liPos, p - liPos);
		        Vector feelerHit = feeler.HitPoint();

		        // check that point is less 90 degrees therefore it is on the same side of light source as displayed point
		        float dotLiFeel_LiPoint = Vector.Dot((liPos - feelerHit), liPos - p);
		        bool correctLightSide = dotLiFeel_LiPoint >= 0;

		        // check that the point is not the same point as the displayed point 
		        // and if it is closer to light source or further away
		        float squaredLen_LiFeel = Vector.Dot((liPos - feelerHit), liPos - feelerHit);
		        bool beforeObj  = Math.Abs(dotLiFeel_LiPoint - squaredLen_LiFeel) < 0.0001; //0.0001 to stop float round error for <=

		        //if there is a collision and it is closer than the displayed scObject and it is on the same side of light as scObject
		        if( (feeler.t<=0) || ((beforeObj) && (correctLightSide)) ){
			
			        //diffuse reflection
			        color = color + ( (hit.scObject.diffuse * lights[i].diffuse) * ( Vector.Dot(s, m) / Convert.ToSingle( (Math.Sqrt(Vector.Dot(s, s))*Math.Sqrt(Vector.Dot(m, m))) ) ));

			        //specular reflection
			        //h = normalized(normalized(s) + normalized(v))
			        Vector h = s.Normalize() + v.Normalize();	
			        h = h.Normalize();
			        color = color + ( (hit.scObject.specular * lights[i].specular) * Convert.ToSingle( (Math.Pow(( Vector.Dot(h, m) / (Math.Sqrt(Vector.Dot(h, h))*Math.Sqrt(Vector.Dot(m, m))) ), hit.scObject.shininess) ) ));
		        }
	        }
	        return color;
        }


        public int[][] RayTraceScene(bool parallel, bool caching)
        {
            this.parallel = parallel;
            this.caching = caching;
            if (parallel)
            {
                workHeight = windowHeight / 4;
                workWidth = windowWidth / 4;

                //Begin threads performing work
                resetEvents = new ManualResetEvent[NumThreads];
                for (int t = 0; t < NumThreads; t++)
                {
                    resetEvents[t] = new ManualResetEvent(false);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DoWork), (object)t);
                }

                //Wait for the threads to all finish
                WaitHandle.WaitAll(resetEvents);
            }
            else
            {
                workHeight = windowHeight;
                workWidth = windowWidth;

                RayTracingLoop(0, 0);
            }
            
            return pixelArray;
        }

        private void RayTracingLoop(int R, int C){
            int increaseCount = 0;

            int len = (windowWidth * windowHeight * 3) / 10;

            int modelCount = ((R * windowWidth * 3) + C)/ len;
            
            if ((modelCount == 0)||(modelCount==5))
            {
                modelCount--;
            }
            
            //Begin loop at correct Row and Column so that different threads calculate different areas
            for (int r=R; r < R + workWidth; r++)
            {
                for (int c=0; c < windowWidth; c++)
                {
                    // construct ray through (c, r) using axis u,v,n and window constraints H,W
                    Vector d = new Vector(0, 0, 0);

                    //d = pixelPos - eye
                    //d = -Nn +W(2c/ncols -1)u +H(2r/nrows -1)v
                    d = (n * (-N) +
                        u * (W * ((2 * (float)c / windowWidth) - 1)) +
                        v * (H * ((2 * (float)r / windowHeight) - 1)));

                    // intersect ray with scene objects
                    Hit hit = intersect(eye, d);

                    // shade pixel accordingly
                    Color color = shade(hit);

                    // add a purple haze to objects in the distance
                    float hazeDistance = 8.0f;
                    if (hit.scObject != null && hit.t > hazeDistance)
                    {
                        float logDist;
                        if (this.caching)
                        {
                            // Caching for haze intensity depending upon distance from the camera
                            if (HttpRuntime.Cache.Get("colorForHitT" + hit.t.ToString()) != null)
                            {
                                color = (Color)HttpRuntime.Cache.Get("colorForHitT" + hit.t.ToString());
                            }
                            else
                            {
                                logDist = Convert.ToSingle(Math.Log((hit.t - hazeDistance) + 1));
                                if (logDist > 20)
                                    logDist = 20;
                                color = color + (new Color(0.05f, 0.01f, 0.05f)) * logDist;
                                HttpRuntime.Cache.Insert("colorForHitT" + hit.t.ToString(), color);
                            }
                        }
                        else
                        {
                            logDist = Convert.ToSingle(Math.Log((hit.t - hazeDistance) + 1));
                            if (logDist > 20)
                                logDist = 20;
                            color = color + (new Color(0.05f, 0.01f, 0.05f)) * logDist;
                        }
                    }

                    if (((r * windowWidth * 3) + c) % len == 0)
                    {
                        modelCount++;                
                    }

                    try
                    {
                        //Store double array of RGB values
                        pixelArray[modelCount][((r * windowWidth) * 3) + (c * 3) + 0 - modelCount * len] = AddColor(color.r);    //Red
                        pixelArray[modelCount][((r * windowWidth) * 3) + (c * 3) + 1 - modelCount * len] = AddColor(color.g);    //Green
                        pixelArray[modelCount][((r * windowWidth) * 3) + (c * 3) + 2 - modelCount * len] = AddColor(color.b);    //Blue

                        increaseCount++;
                        if ((increaseCount == (len*2.5)) && (parallel))
                        {
                            return;
                        }
                    }
                    catch (IndexOutOfRangeException rangeEx)
                    {
                        Debug.WriteLine(((r * windowWidth) * 3) + (c * 3) + 0 - modelCount * len, "Position 1");
                        Debug.WriteLine(modelCount, "ModelCount");
                        Debug.WriteLine(rangeEx.StackTrace, "STACK TRACE");
                    }
                }
            }
        }

        int AddColor(float color){
            return Convert.ToInt16(color * 255);
        }

        private void DoWork(object num)
        {
            RayTracingLoop(workHeight*(int)num, workWidth*(int)num); 
            
            //Event handler notifications for when thread completed
            resetEvents[(int)num].Set();
        }
    }
}