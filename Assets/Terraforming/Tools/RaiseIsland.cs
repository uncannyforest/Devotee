using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaiseIsland : Tool {
    public int islandIterations = 3;

    override public void Load() {
        selector.transform.localScale = new Vector3(3 * selector.defaultSize, selector.transform.localScale.y, 3 * selector.defaultSize);
    }

    override public void Unload() {
        selector.transform.localScale = new Vector3(selector.defaultSize, selector.transform.localScale.y, selector.defaultSize);
    }

    private HexPos Dir(int i) {
        return new HexPos[] { HexPos.E, HexPos.W, HexPos.Q, HexPos.A, HexPos.S, HexPos.D }[i % 6];
    }
    private void IncrementCompass(HexPos[] axes) {
        for (int i = 0; i < 6; i++) axes[i] += Dir(i);
    }

    public void CreateNewGround(HexPos position) {
        if (!Terrain.I.CanModTerrain(position) || Terrain.Grid[position] != null) return;
        Column column = Column.Instantiate(position, 0, SurfaceHeight.MoreRandomSurface());
    }

    public override void UpdatePos(HexPos pos) {
        selector.Position = pos;
        if (Terrain.I.CanRaiseTerrain(pos)) selector.Color = terraformer.selectorReady;
        else selector.Color = terraformer.selectorInvalid;
    }

    override public bool Use() {
        RaiseGround.Use(Position);

        HexPos[] compass = new HexPos[] { Position, Position, Position, Position, Position, Position };
        IncrementCompass(compass);

        foreach (HexPos axis in compass)
            if (Terrain.Grid[axis] == null) CreateNewGround(axis);

        for (int i = 0; i < 6; i++) {
            HexPos axis = compass[i];
            int[] diffs = SurfaceHeight.GetSeafloorHeightDifferences(axis, Position);
            int max = Mathf.Max(diffs);
            if (max < 0)
                 SurfaceHeight.RaiseColumnUnlessOccupied(axis, Mathf.Min(-max, 2), null);
            if (Mathf.Min(diffs) < 0) {
                for (int d = 0; d < 2; d++) {
                    if (diffs[d] < max) SurfaceHeight.RaiseCorner(axis, (Position - axis).UnitCorners[d], 1, true);
                }
            }
        }
        for (int i = 0; i < 6; i++) {
            HexPos cur = compass[i];
            HexPos adj = compass[(i + 1) % 6];
            int diff = SurfaceHeight.GetSeafloorHeightDifferences(cur, adj)[0];
            if (diff < 0) SurfaceHeight.RaiseCorner(cur, (adj - cur).UnitCorners[0], 1, true);
            else if (diff > 0) SurfaceHeight.RaiseCorner(adj, (cur - adj).UnitCorners[1], 1, true);
        }

        for (int i = 1; i <= islandIterations; i++) {
            bool changed = false;
            for (int d = 0; d < 6; d++)
                changed |= FlattenRow(compass[d] + Dir(d + 1), Dir(d + 2), Dir(d + 3), Dir(d + 4), i);
            if (!changed) break;
            IncrementCompass(compass);
            for (int d = 0; d < 6; d++)
                FlattenAxis(compass[d], Dir(d + 2), Dir(d + 3), Dir(d + 4), i);
        }

        return true;
    }

    private bool FlattenRow(HexPos start, HexPos direction, HexPos comparison0, HexPos comparison1, int times) {
        bool changed = false;
        HexPos cur = start;
        for (int i = 0; i < times; i++) {
            if (Terrain.Grid[cur] == null) CreateNewGround(cur);
            Terrain.Grid[cur].debugInfo = "";
            int[] diffs = SurfaceHeight.GetSeafloorHeightDifferences(cur, cur + comparison0).Concat(
                SurfaceHeight.GetSeafloorHeightDifferences(cur, cur + comparison1)).ToArray();
            Terrain.Grid[cur].debugInfo += "diffs " + diffs[0] + " " + diffs[1] + " " + diffs[2] + " " + diffs[3];
            int max = Mathf.Max(diffs);
            Terrain.Grid[cur].debugInfo += " max " + max;
            if (max < 0) {
                Terrain.Grid[cur].debugInfo += " all " + Mathf.Min(-max, 1 + times * 2);
                if (Terrain.Grid[cur] == null) CreateNewGround(cur);
                SurfaceHeight.RaiseColumnUnlessOccupied(cur, Mathf.Min(-max, 1 + times * 2), null);
                changed = true;
            }
            if (Mathf.Min(diffs) < 0) {
                max = Mathf.Min(max, 0);
                int[] cornersToRaise = new int[] {0, 0, 0, 0, 0, 0};
                if (diffs[0] < max) cornersToRaise[comparison0.UnitCorners[0]] = Mathf.Min(2, max - diffs[0]);
                if (diffs[1] < max || diffs[2] < max) cornersToRaise[comparison0.UnitCorners[1]] = Mathf.Min(2, max - Mathf.Min(diffs[1], diffs[2]));
                if (diffs[3] < max) cornersToRaise[comparison1.UnitCorners[1]] = Mathf.Min(2, max - diffs[3]);
                Terrain.Grid[cur].debugInfo += " corners " + cornersToRaise[0] + " " + cornersToRaise[1] + " " + cornersToRaise[2] + " " + cornersToRaise[3] + " " + cornersToRaise[4] + " " + cornersToRaise[5];
                RaiseCorners(cur, cornersToRaise, true);
            }
            cur = cur + direction;
        }
        cur = start;
        for (int i = 0; i < times - 1; i++) {
            HexPos adj = cur + direction;
            int diff = SurfaceHeight.GetSeafloorHeightDifferences(cur, adj)[0];
            if (diff < 0) RaiseCorner(cur, direction.UnitCorners[0], Mathf.Min(2, -diff), true);
            else if (diff > 0) RaiseCorner(adj, direction.UnitCorners[1], Mathf.Min(2, -diff), true);
            cur = adj;
        }

        return changed;
    }

    private void FlattenAxis(HexPos cur, HexPos comparison0, HexPos comparison1, HexPos comparison2, int recentTimes) {
        if (Terrain.Grid[cur] == null) CreateNewGround(cur);
        Terrain.Grid[cur].debugInfo = "";
        int[] diffs = SurfaceHeight.GetSeafloorHeightDifferences(cur, cur + comparison0).Concat(
            SurfaceHeight.GetSeafloorHeightDifferences(cur, cur + comparison1)).Concat(
            SurfaceHeight.GetSeafloorHeightDifferences(cur, cur + comparison2)).ToArray();
        Terrain.Grid[cur].debugInfo += "diffs " + diffs[0] + " " + diffs[1] + " " + diffs[2] + " " + diffs[3] + " " + diffs[4] + " " + diffs[5];
        int max = Mathf.Max(diffs);
        Terrain.Grid[cur].debugInfo += " max " + max;
        if (max < 0) {
            Terrain.Grid[cur].debugInfo += " all " + Mathf.Min(-max, 2 + recentTimes * 2);
            if (Terrain.Grid[cur] == null) CreateNewGround(cur);
            SurfaceHeight.RaiseColumnUnlessOccupied(cur, Mathf.Min(-max, 2 + recentTimes * 2), null);
        }
        if (Mathf.Min(diffs) < 0) {
            max = Mathf.Min(max, 0);
            int[] cornersToRaise = new int[] {0, 0, 0, 0, 0, 0};
            if (diffs[0] < max) cornersToRaise[comparison0.UnitCorners[0]] = Mathf.Min(2, max - diffs[0]);
            if (diffs[1] < max || diffs[2] < max) cornersToRaise[comparison0.UnitCorners[1]] = Mathf.Min(2, max - Mathf.Min(diffs[1], diffs[2]));
            if (diffs[3] < max || diffs[4] < max) cornersToRaise[comparison1.UnitCorners[1]] = Mathf.Min(2, max - Mathf.Min(diffs[3], diffs[4]));
            if (diffs[5] < max) cornersToRaise[comparison2.UnitCorners[1]] = Mathf.Min(2, max - diffs[5]);
            Terrain.Grid[cur].debugInfo += " corners " + cornersToRaise[0] + " " + cornersToRaise[1] + " " + cornersToRaise[2] + " " + cornersToRaise[3] + " " + cornersToRaise[4] + " " + cornersToRaise[5];
            RaiseCorners(cur, cornersToRaise, true);
        }
    }

    public void RaiseCorner(HexPos position, int corner, int quantity, bool raiseFloorToClamp) {
        if (Terrain.Grid[position] == null) CreateNewGround(position);
        SurfaceHeight.RaiseCorner(position, corner, quantity, raiseFloorToClamp);
    }

    public void RaiseCorners(HexPos position, int[] quantities, bool raiseFloorToClamp) {
        if (Terrain.Grid[position] == null) CreateNewGround(position);
        SurfaceHeight.RaiseCorners(position, quantities, raiseFloorToClamp);
    }
}
