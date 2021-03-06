using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crotty.Helpers {
    public static class StaticHelpers {
        /// <summary>
        /// Returns distance between Vector3s a & b as a float
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Vector3Distance(Vector3 a, Vector3 b) {
            return Mathf.Sqrt((Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2) + Mathf.Pow(a.z - b.z, 2)));
        }

        /// <summary>
        /// Gets the distance between 2 Vector3s as a float on the XZ plane
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Vector2DistanceXZ(Vector3 a, Vector3 b) {
            return Mathf.Sqrt((Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.z - b.z, 2)));
        }

        public static Vector3 RandomVector3XZ(float v) {
            Vector2 xz = UnityEngine.Random.insideUnitCircle * v;
            return new Vector3(xz.x,0, xz.y);
        }
        /// <summary>
        /// Gets the distance between 2 floats
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float FloatDistance(float a, float b) {
            return Mathf.Abs(Mathf.Abs(a) - Mathf.Abs(b));
        }

        /// <summary>
        /// Gets the perpendicular distance a point is to a line in XZ space
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Vector2PerpendicularXZ(Vector3 lineStart, Vector3 lineEnd, Vector3 point, float bounds) {
            float x1 = lineStart.x, y1 = lineStart.z, x2 = lineEnd.x, y2 = lineEnd.z, x0 = point.x, y0 = point.z;
            float top, bot;
            //https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line

            if(y1 <= y2) {
                if (y0 < y1 - bounds || y0 > y2 + bounds) {
                    return 9999f;
                }
            }
            else {
                if (y0 > y1 + bounds || y0 < y2 - bounds) {
                    return 9999f;
                }
            }

            if (x1 <= x2) {
                if (x0 < x1 - bounds || x0 > x2 + bounds) {
                    return 9999f;
                }
            }
            else {
                if (x0 > x1 + bounds || x0 < x2 - bounds) {
                    return 9999f;
                }
            }

            top = Mathf.Abs((x2 - x1) * (y1 - y0) - (x1 - x0) * (y2 - y1));
            bot = Mathf.Sqrt(Mathf.Pow(x2 - x1,2) + Mathf.Pow(y2 - y1, 2));
            return top / bot;
        }


        /// <summary>
        /// Returns a point on a line that is no more than maxDistance from a in the direction of b, if the distance between b and a is less than max distance it returns b else it returns a
        /// point along the line a b at the point of max distance from in b's direction from a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// /// <param name="maxDistance"></param>
        /// <returns></returns>
        public static Vector3 Vector3PointAlongLine(Vector3 a, Vector3 b, float maxDistance) {
            float dist = Vector3Distance(a, b);
            if (dist <= maxDistance) {
                return b;
            }
            //Difference between x y z and multiply by maxdist/dist added to point a
            Vector3 difference = new Vector3(b.x - a.x, b.y - a.y, b.z - a.z);

            return a + difference * (maxDistance/dist);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 Vector3Follow(Vector3 a, Vector3 b, float percent) {
            float dist = Vector3Distance(a, b);
            //Difference between x y z and multiply by maxdist/dist added to point a
            Vector3 difference = new Vector3(b.x - a.x, b.y - a.y, b.z - a.z);

            return a + difference * percent;
        }
        /// <summary>
        /// Round int z to the nearest int v with offset o
        /// </summary>
        /// <param name="z"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int Round(int z, int v, int o) { // 83   50    25   => 75 or 50 + 25    
            if (o < 0)
                o *= -1;
            if (v < 0)
                v *= -1;
            bool neg = false;
            if (z < 0) {
                neg = true;
                z *= -1;
            }
            int zwo = z - o; // Z without offset                83 -25 => 58                    
            int zwom = zwo % v; // Is it closer last or next    58 % 50 => 8                    
            int ret;
            if(zwom >= v / 2) {
                ret =  z + v - zwom;                            //125
            }
            else {
                ret =  z - zwom;                                //75
            }

            if (neg)
                ret *= -1;
            return ret;
        }

        public static float Vector2Distance(Vector2 a, Vector2 b) {
            return Mathf.Sqrt((Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2)));
        }

        public static Vector3 Vector3Direction(Vector3 centre, Vector3 directionObject) {
            return (directionObject - centre).normalized;
        }

        public static Vector3 Vector3Clamp(Vector3 var, float min, float max) {
            return new Vector3(Mathf.Clamp(var.x, min, max), Mathf.Clamp(var.y, min, max), Mathf.Clamp(var.z, min, max));
        }
    }
}
