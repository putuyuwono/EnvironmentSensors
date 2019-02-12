using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentSensorProject
{
    public class Utils
    {
        public static String GenerateMsgNo(int msgNo)
        {
            int msg_limit = 9999;
            if (msgNo > msg_limit)
            {
                msgNo = (msgNo % msg_limit) + 1;
            }

            StringBuilder sb = new StringBuilder();
            if (msgNo < 10)
            {
                sb.Append("000");
            }
            else if (msgNo < 100)
            {
                sb.Append("00");
            }
            else if (msgNo < 1000)
            {
                sb.Append("0");
            }
            sb.Append(msgNo);

            return sb.ToString();
        }

        public static PointF GetCentroid(Rectangle rect)
        {
            PointF centroid = new PointF();
            centroid.X = rect.X + rect.Width / 2;
            centroid.Y = rect.Y + rect.Height / 2;
            return centroid;
        }
    }
}
