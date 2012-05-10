using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppPerformanceAnalysis.Controllers.ComputationLogic
{
    public abstract class SceneObject
    {
        public Vector scaling, translation;
        public Color ambient, diffuse, specular;
        public float shininess;
        public float reflectivity;

	    // returns the t value of the closest ray-object intersection, or -1 otherwise
        public abstract float Intersect(Vector source, Vector d);

	    // returns the normal at the given point p
	    // if p is not on the object the result is undefined
        public abstract Vector Normal(Vector p);
    }
}