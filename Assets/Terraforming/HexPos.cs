using UnityEngine;

public struct HexPos {
    public static float SQRT3 = Mathf.Sqrt(3);

    public HexPos(int x, int y) {
        this.x = x;
        this.y = y;
    }

    // internally, x is ESE, y is ENE
    public int x;
    public int y;

    public static implicit operator Vector2(HexPos pos)
        => new Vector2((pos.x + pos.y) * 3f / 2, (pos.y - pos.x) * SQRT3 / 2);
    public static implicit operator Vector3(HexPos pos)
        => new Vector3((pos.x + pos.y) * 3f / 2, 0, (pos.y - pos.x) * SQRT3 / 2);

    public static HexPos E => new HexPos(0, 1);
    public static HexPos W => new HexPos(-1, 1);
    public static HexPos Q => new HexPos(-1, 0);
    public static HexPos A => new HexPos(0, -1);
    public static HexPos S => new HexPos(1, -1);
    public static HexPos D => new HexPos(1, 0);

    public static HexPos operator +(HexPos a, HexPos b) => new HexPos(a.x + b.x, a.y + b.y);
    public static HexPos operator *(HexPos a, int n) => new HexPos(a.x * n, a.y * n);
    public static HexPos operator *(int n, HexPos a) => new HexPos(a.x * n, a.y * n);

    public static bool operator ==(HexPos a, HexPos b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(HexPos a, HexPos b) => a.x != b.x || a.y != b.y;
    public override bool Equals(object obj) => obj is HexPos a && this == a;
    public override int GetHashCode() => x.GetHashCode() + (y * SQRT3).GetHashCode();

    public HexPos rotate(float angle) {
        int rotations = Mathf.RoundToInt(angle / 60);
        while (rotations < 0) rotations += 600;
        switch (rotations % 6) {
            case 1: return new HexPos(-y, x + y);
            case 2: return new HexPos(-x - y, x);
            case 3: return new HexPos(-x, -y);
            case 4: return new HexPos(y, -x - y);
            case 5: return new HexPos(x + y, -x);
            default: return this;
        }
    }

    override public string ToString() => "(" + x + ", " + y + ")";
}
