using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppPerformanceAnalysis.Controllers.ComputationLogic
{
    public class Vector
    {
        public float x;
        public float y; 
        public float z;

        public Vector()
        {
        }

        public Vector(float x, float y, float z)
        {
	        this.x = x;
	        this.y = y;
	        this.z = z;
        }


        public static float Dot(Vector f, Vector v)
        {
	        /** dot product *************************************/

	        float ret = 0;

	        ret += v.x * f.x;
	        ret += v.y * f.y;
	        ret += v.z * f.z;

	        return ret;
        }

        public static Vector Cross(Vector f, Vector v)
        {
	        /** cross product ***********************************/

	        float a=0, b=0, c=0;

	        a = f.y*v.z - f.z*v.y;
	        b = f.z*v.x - f.x*v.z;
	        c = f.x*v.y - f.y*v.x;  
            
            return new Vector(a, b, c);
        }

        public static Vector operator +(Vector f, Vector v)
        {
	        return new Vector(f.x + v.x, f.y + v.y, f.z + v.z);
        }

        public static Vector operator -(Vector f, Vector v)
        {
	        return new Vector(f.x - v.x, f.y - v.y, f.z - v.z);
        }


        public static Vector operator *(float s, Vector v)
                    {return new Vector(v.x * s, v.y * s, v.z * s);}

        public static Vector operator *(Vector v, float s)
                    {return new Vector(v.x * s, v.y * s, v.z * s);}


        public static Vector operator /(float f, Vector v)
                    { return new Vector(v.x / f, v.y / f, v.z / f); }

        public static Vector operator /(Vector v, float f)
                    {return new Vector(v.x/f, v.y/f, v.z/f);}


        public Vector Scale(float sx, float sy, float sz)
        {
	        return new Vector(x * sx, y * sy, z * sz);    
        }

        public Vector Normalize()
        {
	        float l = Convert.ToSingle( Math.Sqrt(Dot(this, this)) );
	        return new Vector(x / l, y / l, z / l);
        }

        public void NormalizeDestructive() 
        {
	        float l = Convert.ToSingle( Math.Sqrt(Dot(this, this)) );
	        x /= l;
	        y /= l;
	        z /= l;
        }

        public void Set(Vector v)
        {
	        x = v.x;
	        y = v.y;
	        z = v.z;
        }
    }
}