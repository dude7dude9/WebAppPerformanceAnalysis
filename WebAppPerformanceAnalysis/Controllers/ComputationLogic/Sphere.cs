using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppPerformanceAnalysis.Controllers.ComputationLogic
{
    public class Sphere : SceneObject
    {
        //public Vector scaling, translation;
        //public Color ambient, diffuse, specular;
        //public float shininess;
        //public float reflectivity;

        public Sphere(){}

        public override float Intersect(Vector source, Vector d)
        {
	        float A = Vector.Dot(d, d);
            float B = Vector.Dot(2*source, d); 
	        //NB C determines that this spere is positioned at the origin and has radius 1
            float C = Vector.Dot(source, source) - 1; 
	        if(B*B - 4*A*C <= 0) return -1;  // no hit 

	        float t1; 
	        if(B>0)   // for numerical precision 
		        t1 = (-B - Convert.ToSingle( Math.Sqrt(B*B - 4*A*C)) ) / (2*A); 
	        else 
		        t1 = (-B + Convert.ToSingle( Math.Sqrt(B*B - 4*A*C)) ) / (2*A); 
	
	        float t2 = C/(A*t1); // easier way to get t2 
	
	        if(t1<t2) 
		        return t1;  // need only closer t 
	        else 
		        return t2; 
        }

        public override Vector Normal(Vector p)
        {
	        //Assuming sphere at origin as (norm(P-C)==norm(P))
	        return p.Normalize();

	        //return Vector();
        }
    }
}