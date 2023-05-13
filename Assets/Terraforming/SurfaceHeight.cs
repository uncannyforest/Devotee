using System;
using UnityEngine;
using Random = UnityEngine.Random;

// YOUR HOME FOR ALL THINGS RELATING TO SURFACE HEIGHT.
// THEY'RE ALL HERE.  IN THIS FILE

// int[] wrapper with helper methods
public class Corners {
    private int[] v;

    public Corners(int a, int b, int c, int d, int e, int f) {
        this.v = new int[] {a, b, c, d, e, f};
    }
    public Corners(int[] v) {
        this.v = v;
    }

    public static implicit operator int[](Corners corners) {
        return corners.v;
    }

    public int Normalize() {
        int diff = Mathf.Min(v[0], v[1], v[2], v[3], v[4], v[5]) - 1;
        this.v = new int[] {v[0] - diff, v[1] - diff, v[2] - diff, v[3] - diff, v[4] - diff, v[5] - diff};
        return diff;
    }

    public int[] heightsWithBase(float theBase) {
        int intBase = Mathf.RoundToInt(theBase);
        return new int[] {v[0] + intBase, v[1] + intBase, v[2] + intBase, v[3] + intBase, v[4] + intBase, v[5] + intBase};
    }
}


public class SurfaceHeight {
    public static int RandomSign() {
        return Random.Range(0, 2) * 2 - 1;
    }

    public static int? GetHeight(HexPos position, int corner) {
        Column column = Terrain.Grid[position];
        if (column == null) return 0;
        int level = column.GetHeight(corner);
        return level >= 0 ? level : (int?)null;
    }

    public static int GetSeafloorHeight(HexPos position, int corner) {
        Column column = Terrain.Grid[position];
        if (column == null) return 0;
        return column.GetHeight(corner);
    }

    public static int GetMaxHeightExcept(HexPos position, int corner) {
        Column column = Terrain.Grid[position];
        if (column == null) return 0;
        int[] heights = column.Heights;
        int result = 0;
        for (int i = 0; i < 6; i++) if (i != corner) result = Mathf.Max(result, heights[i]);
        return result;
    }

    public static int[] GetSeafloorHeightDifferences(HexPos position, HexPos adjacentPosition) {
        int[] corners = (adjacentPosition - position).UnitCorners;
        int diff1 = GetSeafloorHeight(position, corners[0]) - GetSeafloorHeight(adjacentPosition, (corners[0] + 4) % 6);
        int diff2 = GetSeafloorHeight(position, corners[1]) - GetSeafloorHeight(adjacentPosition, (corners[1] + 2) % 6);
        return new int[] {diff1, diff2};
    }

    public static void RaiseCorners(HexPos position, int[] quantities, bool raiseFloorToClamp) {
        if (!Terrain.I.CanModTerrain(position)) return;
        Column column = Terrain.Grid[position];
        int[] corners = new int[6];
        for (int i = 0; i < 6; i++) corners[i] = column.Heights[i] + quantities[i];
        column.SetHeights(corners, raiseFloorToClamp);
    }

    public static void RaiseCorner(HexPos position, int corner, int quantity, bool raiseFloorToClamp) {
        if (!Terrain.I.CanModTerrain(position)) return;
        Column column = Terrain.Grid[position];
        int[] corners = column.Heights;
        corners[corner] += quantity;
        column.SetHeights(corners, raiseFloorToClamp);
    }

    public static int RaiseColumnUnlessOccupied(HexPos position, int quantity, int? groundId) {
        if (!Terrain.I.CanModTerrain(position)) return 0;
        return RaiseColumn(position, quantity, groundId);
    }

    public static int RaiseColumn(HexPos position, int quantity, int? groundId) {
        if (!Terrain.I.CanRaiseTerrain(position)) return 0;
        Transform player = Terrain.I.ContainsPlayer(position);
        if (player != null) {
            player.position += (quantity + .25f) * Vector3.up;
        }
        int yPos = RaiseColumnDangerous(position, quantity, groundId);
        MaybeRaiseOrigin(yPos);
        return yPos;
    }

    private static void MaybeRaiseOrigin(int yPos) {
        if (yPos > Terrain.I.maxHeight) {
            for (int i = 0; i < Terrain.I.originPositions.Length; i++) {
                HexPos origin = Terrain.I.originPositions[i];
                Transform player = Terrain.I.ContainsPlayer(Terrain.I.originPositions[i]);
                if (player != null) {
                    player.position += (yPos - Terrain.I.maxHeight + .25f) * Vector3.up;
                }
                RaiseColumnDangerous(origin, yPos - Terrain.I.maxHeight, i);
            }
            Terrain.I.maxHeight = yPos;
        }
    }

    private static int RaiseColumnDangerous(HexPos Position, int quantity, int? groundId) {
        Column column = Terrain.Grid[Position];
        ExtendColumn(Position, -quantity, groundId);
        column.RigidbodyMove(quantity);
        int yPos = Mathf.RoundToInt(column.Surface.position.y) + quantity;
        CircularIntersectionManager.I.UpdateHex(Position);
        return yPos;
    }

    private static void ExtendColumn(HexPos Position, int level, int? groundId) {
        Column column = Terrain.Grid[Position];
        while (column.DeepestUnderground.position.y > level) {
            float yPosition = column.DeepestUnderground.position.y - Terrain.I.scale;
            if (column.HasUnderground) yPosition -= Terrain.I.scale;
            int defGroundId = groundId ?? Random.Range(0, Terrain.I.randomUnderground.Length);
            Transform underground = column.InstantiateUnderground(
                defGroundId,
                Quaternion.Euler(0, -30, 0) * (Vector3)(Position) * Terrain.I.scale + yPosition * Vector3.up,
                60 * Random.Range(0, 6),
                180 * Random.Range(0, 2),
                RandomSign());
        }
    }

    public static int[] RandomSurface() {
        int[] data;
        int adj;
        float random = Random.value;
        Corners corners;
        int index = Random.Range(0, 16);
        Debug.Log("New random surface: " + index);
        if (index < 2) { // flat, 2 chances
            corners = new Corners(1, 1, 1, 1, 1, 1);
        } else if (index < 6) { // slope/cliff, 4 chances
            int max = Terrain.I.scale / Random.Range(1, 3); // prefer slope to cliff 2:1
            data = new int[] {
                Random.Range(0, max) * 2,
                Random.Range(0, max) * 2};
            corners = random < 1/3f ? new Corners(data[0], data[0], data[0], data[1], data[1], data[1])
                : random < 2/3f ? new Corners(data[0], data[0], data[1], data[1], data[1], data[0])
                : new Corners(data[0], data[1], data[1], data[1], data[0], data[0]);
        } else if (index < 9) { // 3 levels, 3 chances
            if (Random.value < 1/3f) { // cliff end
                int numSeed = Random.Range(0, 4);
                int smallNum = numSeed < 1 ? 9 : numSeed < 3 ? 8 : 7;
                int bigNum = numSeed < 2 ? 5 : numSeed < 3 ? 4 : 3;
                int seed = Random.Range(0, 6);
                data = new int[] {
                    seed < 2 ? 1 : seed < 4 ? smallNum : bigNum,
                    seed < 1 ? smallNum : seed < 3 ? bigNum : seed < 5 ? 1 : smallNum,
                    seed % 3 < 1 ? bigNum : seed % 3 < 2 ? smallNum : 1};
            } else {
                int max = Terrain.I.scale / Random.Range(1, 3); // prefer slope to cliff
                data = new int[] {
                    Random.Range(0, max) * 2,
                    Random.Range(0, max) * 2,
                    Random.Range(0, max) * 2};
            }
            adj = 1 - Mathf.Min(data) * 2;
            corners = random < .5f ? new Corners(data[0], data[0], data[1], data[1], data[2], data[2])
                : new Corners(data[0], data[1], data[1], data[2], data[2], data[0]);
        } else if (index < 10) { // canyon, 1 chance
            int height = Random.Range(0, Terrain.I.scale / 2) * 2 + 11;
            int firstGap = Random.Range(0, 6);
            int secondGap = (firstGap + Random.Range(2, 5)) % 6;
            int maybeAnotherGap = Random.Range(0, 12);
            corners = new Corners(
                firstGap == 0 || secondGap == 0 || maybeAnotherGap == 0 ? 1 : height,
                firstGap == 1 || secondGap == 1 || maybeAnotherGap == 1 ? 1 : height,
                firstGap == 2 || secondGap == 2 || maybeAnotherGap == 2 ? 1 : height,
                firstGap == 3 || secondGap == 3 || maybeAnotherGap == 3 ? 1 : height,
                firstGap == 4 || secondGap == 4 || maybeAnotherGap == 4 ? 1 : height,
                firstGap == 5 || secondGap == 5 || maybeAnotherGap == 5 ? 1 : height
            );
        } else if (index < 13) { // 2 levels, 3 chances
            int max = Random.value < 1/3f ? Terrain.I.scale / 2 : Terrain.I.scale;
            Debug.Log("Max " + max);
            data = new int[] {
                Random.Range(0, max) * 2,
                Random.Range(0, max) * 2,
                Random.Range(0, max) * 2,
                Random.Range(0, max) * 2};
            adj = 1 - Mathf.Min(data) * 2;
            corners = random < 1/3f ? new Corners(data[0], data[0], data[1], data[2], data[2], data[3])
                : random < 2/3f ?  new Corners(data[0], data[1], data[2], data[2], data[3], data[0])
                : new Corners(data[1], data[2], data[2], data[3], data[0], data[0]);
        } else if (index < 14) { // 3 pillars, 1 chance
            bool alt = Random.value < .5;
            corners = new Corners(
                (Random.Range(0, 3) + (alt ? 7 : 0)) * 2,
                (Random.Range(0, 3) + (!alt ? 7 : 0)) * 2,
                (Random.Range(0, 3) + (alt ? 7 : 0)) * 2,
                (Random.Range(0, 3) + (!alt ? 7 : 0)) * 2,
                (Random.Range(0, 3) + (alt ? 7 : 0)) * 2,
                (Random.Range(0, 3) + (!alt ? 7 : 0)) * 2);
        } else { // random, 2 chances
            int max = Terrain.I.scale;
            corners = new Corners(
                Random.Range(0, max) * 2,
                Random.Range(0, max) * 2,
                Random.Range(0, max) * 2,
                Random.Range(0, max) * 2,
                Random.Range(0, max) * 2,
                Random.Range(0, max) * 2);
        }

        corners.Normalize();
        return corners;
    }

    public static int[] MoreRandomSurface() {
        int max = Terrain.I.scale / Random.Range(1, 3);
        Corners corners = new Corners(
            Random.Range(0, max) * 2,
            Random.Range(0, max) * 2,
            Random.Range(0, max) * 2,
            Random.Range(0, max) * 2,
            Random.Range(0, max) * 2,
            Random.Range(0, max) * 2);
        corners.Normalize();
        return corners;
    }

}
