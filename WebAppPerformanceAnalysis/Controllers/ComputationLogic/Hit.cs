using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppPerformanceAnalysis.Controllers.ComputationLogic
{
    public class Hit
    {
        public Vector source;
        public Vector d;
        public float t;
        public SceneObject scObject;

        public Hit(){}

        public Hit(Vector source, Vector d, float t, SceneObject scObject)
        {
	        this.source = source;
	        this.d = d;
	        this.t = t;
	        this.scObject = scObject;
        }

        public Vector HitPoint()
        {	
	        //x = x0 + t*dx		x0 = source_X, dx = direction of ray (x1 - x0)
	        //y = y0 + t*dy
	        //z = z0 + t*dz
	        float x = source.x + t*d.x;
	        float y = source.y + t*d.y;
	        float z = source.z + t*d.z;


	        Vector hitVect = new Vector(x, y, z);

	        return hitVect;
        }
    }
}