using Godot;

namespace marisamod.Scenes.Vfx.SparkProjectile;
public partial class SparkTrail : MeshInstance2D
{
    struct TrailPoint
    {
        public Vector2 Position;
        public float Time;
    }

    [Export] public float Lifetime = 0.4f;
    [Export] public float BaseWidth = 1f;
    [Export] public float MinDistance = 2f;
    [Export] public int InterpolationPerSegment = 3;

    public float LifeTime2 = -1;//大于0时代替Lifetime来影响尾迹点更新
    private readonly List<TrailPoint> _points = [];

    private void UpdatePoints(float second)
    {
        //记录新点
        //Vector2 pos = GetViewport().GetMousePosition();
        Vector2 pos = GetParent<Node2D>().Position;
        if (_points.Count == 0 || pos.DistanceTo(_points[^1].Position) > MinDistance)
        {
            _points.Add(new TrailPoint { Position = pos, Time = second });
        }
        //2. 删除过期点
        float maxTime = Lifetime;
        if (LifeTime2 >0)
            maxTime = LifeTime2;
        _points.RemoveAll(p => second - p.Time > maxTime);

        if (_points.Count < 2)
        {
            Mesh = null;
            return;
        }
    }
    
    public override void _Process(double delta)
    {
        float now = Time.GetTicksMsec() / 1000f;
        UpdatePoints(now);
        //生成插值数组并转换为局部坐标
        List<Vector2> smoothPoints = CatmullRom(_points, InterpolationPerSegment);

        //计算总长度 
        List<float> lengths = [];
        lengths.Add(0);

        for (int i = 1; i < smoothPoints.Count; i++)
        {
            float d = smoothPoints[i].DistanceTo(smoothPoints[i - 1]);
            lengths.Add(lengths[i - 1] + d);
        }

        float totalLength = lengths[^1];
        if (totalLength <= 0.001f) return;
        Mesh =CalculateMesh(smoothPoints, lengths, totalLength);
    }

    //样条插值平滑，并将坐标转化为local坐标
    private List<Vector2> CatmullRom(List<TrailPoint> pts, int subdivisions)
    {
        List<Vector2> result = [];
        if (pts.Count < 2)
            return result;

        for (int i = 0; i < pts.Count - 1; i++)
        {
            Vector2 p0 = i > 0 ? pts[i - 1].Position : pts[i].Position;
            Vector2 p1 = pts[i].Position;
            Vector2 p2 = pts[i + 1].Position;
            Vector2 p3 = i < pts.Count - 2 ? pts[i + 2].Position : p2;

            for (int j = 0; j < subdivisions; j++)
            {
                float t = j / (float)subdivisions;
                result.Add(ToLocal(CatmullRomInterpolate(p0, p1, p2, p3, t)) );
            }
        }

        result.Add( ToLocal(pts[^1].Position) );
        return result;
    }

    private Vector2 CatmullRomInterpolate(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2 * p1) +
            (-p0 + p2) * t +
            (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
            (-p0 + 3 * p1 - 3 * p2 + p3) * t3
        );
    }
    private ArrayMesh CalculateMesh(List<Vector2> smoothPoints,List<float> lengths,float totalLength)
    {
        // 构建mesh
        var verts = new List<Vector2>();
        var uvs = new List<Vector2>();
        var indices = new List<int>();

        for (int i = 0; i < smoothPoints.Count; i++)
        {
            Vector2 p = smoothPoints[i];
            //方向
            Vector2 dir;
            if (i == 0)
                dir = (smoothPoints[i + 1] - p).Normalized();
            else if (i == smoothPoints.Count - 1)
                dir = (p - smoothPoints[i - 1]).Normalized();
            else
                dir = (smoothPoints[i + 1] - smoothPoints[i - 1]).Normalized();

            Vector2 normal = new Vector2(-dir.Y, dir.X);

            //时间映射（用原始点时间）
            float t =i/(float)smoothPoints.Count;

            float width = BaseWidth * Mathf.Pow(t, 0.5f);

            Vector2 left = p - normal * width;
            Vector2 right = p + normal * width;

            verts.Add(left);
            verts.Add(right);

            float u = lengths[i] / totalLength;

            uvs.Add(new Vector2(u, 0));
            uvs.Add(new Vector2(u, 1));
        }

        //索引
        for (int i = 0; i < smoothPoints.Count - 1; i++)
        {
            int idx = i * 2;

            indices.Add(idx);
            indices.Add(idx + 1);
            indices.Add(idx + 2);

            indices.Add(idx + 1);
            indices.Add(idx + 3);
            indices.Add(idx + 2);
        }

        //提交mesh
        ArrayMesh mesh = new ArrayMesh();

        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        arrays[(int)Mesh.ArrayType.Vertex] = verts.ToArray();
        arrays[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
        arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();

        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        return mesh;
    }

}