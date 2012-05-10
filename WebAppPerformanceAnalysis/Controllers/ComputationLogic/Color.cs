using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppPerformanceAnalysis.Controllers.ComputationLogic
{
    public class Color
    {
        public float r, g, b;

        public Color() { }

        public Color(float r, float g, float b)
        {
	        this.r = r;
	        this.g = g;
	        this.b = b;
        }

        public static Color operator +(Color r, Color c)
        {
	        return new Color(r.r + c.r, r.g + c.g, r.b + c.b);
        }

        public static Color operator *(Color r, Color c)
        {
	        return new Color(r.r * c.r, r.g * c.g, r.b * c.b);
        }

        public static Color operator *(Color r, float s)
                    {return new Color(r.r*s, r.g*s, r.b*s);}
        public static Color operator *(float s, Color r)
                    {return new Color(r.r * s, r.g * s, r.b * s);}

        public static Color operator /(Color r, float f)
                    {return new Color(r.r/f, r.g/f, r.b/f);}
        public static Color operator /(float f, Color r)
                    {return new Color(r.r/f, r.g/f, r.b/f);}
    }
}