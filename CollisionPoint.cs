using System.Numerics;
using YawSafety;

public class CollisionPoint
{

    private Vector3 chairCenter = new Vector3(424, 240, 0);
    private Vector3 relativeCoordinates;
    private Vector3 actualCoordinates;

    public CollisionPoint(float x, float y)
    {
        relativeCoordinates = new Vector3(x, y, 0);
    }

    public Vector3 GetActualCoordinates()
    {
        // Get point relative to center
        Vector3 p = relativeCoordinates - chairCenter;
        // Transform with quaternion
        p = TransformPoint(p);
        // Convert back to other coordinate frame
        actualCoordinates = p + chairCenter;
        return actualCoordinates;
    }

    private Vector3 TransformPoint(Vector3 input)
    {
        Vector3 axis = new Vector3(0, 0, 1);
        double angle = YawController.Instance.ChairYaw * (Math.PI / 180f);
        float qx = axis.X * (float)Math.Sin(angle/2);
        float qy = axis.Y * (float)Math.Sin(angle/2);
        float qz = axis.Z * (float)Math.Sin(angle/2);
        float qw = (float)Math.Cos(angle/2);

        Quaternion q = new Quaternion(qx, qy, qz, qw);
        Quaternion newPos = q * new Quaternion(input.X, input.Y, input.Z, 0) * Quaternion.Inverse(q);
        return new Vector3(newPos.X, newPos.Y, newPos.Z);
    }
}