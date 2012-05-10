using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppPerformanceAnalysis.Controllers.ComputationLogic
{
    public class Plane : SceneObject
    {
        //public Vector scaling, translation;
        //public Color ambient, diffuse, specular;
        //public float shininess;
        //public float reflectivity;

        public Vector n;
        public float a;  // distance from origin


        public override float Intersect(Vector source, Vector d)
        {
	        float t = (a - Vector.Dot(source, n))/Vector.Dot(d, n);

	        if( (t >= 0) && (Vector.Dot(d, n) != 0) )
		        return t;

	        return -1;
        }

        public override Vector Normal(Vector p)
        {	
	        return n;
        }

        public double Distance(Vector p)
        {
	        return Vector.Dot(n, p) - a;
        }
    }
}