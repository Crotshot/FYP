using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crotty.Helpers {
    public static class StaticHelpers {
        /// <summary>
        /// Gets the distance between 2 Vector3s as a float
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
    }
}
