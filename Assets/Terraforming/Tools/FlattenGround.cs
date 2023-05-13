using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlattenGround : Tool {
    public int[] heightDiffs = new int[] {0, 4, 8, 12, 8, 4};
    public Color selectorPartial;

    private int corner = 0;

    override public void Load() {
        UpdatePos(selector.Position + HexPos.E);
    }

    override public void Unload() {
        selector.Position = selector.Position;
    }

    public static int MathMid(int a, int b, int c) {
        int lower = Mathf.Min(a, b);
        int higher = Mathf.Max(a, b);
        return c < lower ? lower : c > higher ? higher : c;
    }

    public override void UpdatePos(HexPos pos) {
        HexPos diff = pos - selector.Position;
        if (diff == HexPos.zero || diff == new HexPos(2, 0)) return;
        int rot = diff.ToUnitRotation() / 60;
        int change = (rot - corner + 6) % 6;
        if (change < 2) {
            selector.Position = pos;
            corner = (corner + (change == 0 ? 1 : 5)) % 6;
        } else {
            selector.Position = selector.Position;
            corner = (corner + (change < 4 ? 1 : 5)) % 6;
        }
        selector.transform.position += Quaternion.Euler(0, corner * -60 - 30, 0) * new Vector3(Terrain.I.scale, 0, 0);
        Debug.Log("Rot: " + rot + " Corner: " + corner);
        UpdateColor();
    }

    public void UpdateColor() {
        HexPos pos1 = selector.Position + HexPos.D.Rotate(corner * 60);
        HexPos pos2 = selector.Position + HexPos.E.Rotate(corner * 60);
        int cannotMod = (Terrain.I.CanModTerrain(selector.Position) ? 0 : 2)
                + (Terrain.I.CanModTerrain(pos1) ? 0 : 3)
                + (Terrain.I.CanModTerrain(pos2) ? 0 : 4);
        if (cannotMod >= 5) {
            selector.Color = terraformer.selectorInvalid;
        } else {
            int myCorner = cannotMod == 2 ? Int32.MaxValue : SurfaceHeight.GetHeight(selector.Position, corner) ?? 0;
            int corner1 = cannotMod == 3 ? Int32.MaxValue : SurfaceHeight.GetHeight(pos1, (corner + 2) % 6) ?? 0;
            int corner2 = cannotMod == 4 ? Int32.MaxValue : SurfaceHeight.GetHeight(pos2, (corner + 4) % 6) ?? 0;
            int minHeight = Mathf.Min(myCorner, corner1, corner2);
            int overheight0 = cannotMod == 2 || SurfaceHeight.GetMaxHeightExcept(selector.Position, corner) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
            int overheight1 = cannotMod == 3 || SurfaceHeight.GetMaxHeightExcept(pos1, (corner + 2) % 6) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
            int overheight2 = cannotMod == 4 || SurfaceHeight.GetMaxHeightExcept(pos2, (corner + 4) % 6) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
            if (overheight0 + overheight1 + overheight2 == 0) selector.Color = terraformer.selectorReady;
            else if (overheight0 + overheight1 + overheight2 == 1) selector.Color = selectorPartial;
            else if (cannotMod > 0) selector.Color = terraformer.selectorInvalid;
            else {
                minHeight = MathMid(myCorner, corner1, corner2);
                overheight0 = SurfaceHeight.GetMaxHeightExcept(selector.Position, corner) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
                overheight1 = SurfaceHeight.GetMaxHeightExcept(pos1, (corner + 2) % 6) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
                overheight2 = SurfaceHeight.GetMaxHeightExcept(pos2, (corner + 4) % 6) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
                if (overheight0 + overheight1 + overheight2 > 0) selector.Color = terraformer.selectorInvalid;
                else selector.Color = selectorPartial;
            }
        }
    }

    public override bool Use() {
        HexPos pos1 = selector.Position + HexPos.D.Rotate(corner * 60);
        HexPos pos2 = selector.Position + HexPos.E.Rotate(corner * 60);
        int cannotMod = (Terrain.I.CanModTerrain(selector.Position) ? 0 : 2)
                + (Terrain.I.CanModTerrain(pos1) ? 0 : 3)
                + (Terrain.I.CanModTerrain(pos2) ? 0 : 4);
        if (cannotMod >= 5) return false;
        int myCorner = cannotMod == 2 ? Int32.MaxValue : SurfaceHeight.GetHeight(selector.Position, corner) ?? 0;
        int corner1 = cannotMod == 3 ? Int32.MaxValue : SurfaceHeight.GetHeight(pos1, (corner + 2) % 6) ?? 0;
        int corner2 = cannotMod == 4 ? Int32.MaxValue : SurfaceHeight.GetHeight(pos2, (corner + 4) % 6) ?? 0;
        int minHeight = Mathf.Min(myCorner, corner1, corner2);
        bool modCorner0, modCorner1, modCorner2;
        int overheight0 = cannotMod == 2 || SurfaceHeight.GetMaxHeightExcept(selector.Position, corner) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
        int overheight1 = cannotMod == 3 || SurfaceHeight.GetMaxHeightExcept(pos1, (corner + 2) % 6) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
        int overheight2 = cannotMod == 4 || SurfaceHeight.GetMaxHeightExcept(pos2, (corner + 4) % 6) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
        if (overheight0 + overheight1 + overheight2 <= 1) {
            modCorner0 = overheight0 == 0;
            modCorner1 = overheight1 == 0;
            modCorner2 = overheight2 == 0;
        } else if (cannotMod > 0) {
            return false;
        } else {
            modCorner0 = overheight0 != 0;
            modCorner1 = overheight1 != 0;
            modCorner2 = overheight2 != 0;
            minHeight = MathMid(myCorner, corner1, corner2);
            overheight0 = SurfaceHeight.GetMaxHeightExcept(selector.Position, corner) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
            overheight1 = SurfaceHeight.GetMaxHeightExcept(pos1, (corner + 2) % 6) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
            overheight2 = SurfaceHeight.GetMaxHeightExcept(pos2, (corner + 4) % 6) >= minHeight + Terrain.I.scale * 2 ? 1 : 0;
            if (overheight0 + overheight1 + overheight2 > 0) return false; // all three two far from each other
        }
        if ((modCorner0 && myCorner > minHeight) || (modCorner1 && corner1 > minHeight) || (modCorner2 && corner2 > minHeight)) {
            if (modCorner0 && myCorner > minHeight) SetNewGround(selector.Position, corner, minHeight);
            if (modCorner1 && corner1 > minHeight) SetNewGround(pos1, (corner + 2) % 6, minHeight);
            if (modCorner2 && corner2 > minHeight) SetNewGround(pos2, (corner + 4) % 6, minHeight);
        } else {
            if (modCorner0) SetNewGround(selector.Position, corner, minHeight + 1);
            if (modCorner1) SetNewGround(pos1, (corner + 2) % 6, minHeight + 1);
            if (modCorner2) SetNewGround(pos2, (corner + 4) % 6, minHeight + 1);
        }
        UpdateColor();
        return true;
    }
    
    public void SetNewGround(HexPos position, int corner, int cornerHeight) {
        if (!Terrain.I.CanModTerrain(position)) return;

        if (Terrain.Grid[position] == null) {
            if (cornerHeight == 0) return;
            Column column = Column.Instantiate(position, 0, new int[] {1, 1, 1, 1, 1, 1});
            column.Surface.SetCornerReturningDiff(corner, Mathf.RoundToInt(cornerHeight + 1));
        } else {
            Column column = Terrain.Grid[position];
            int[] heights = column.Heights;
            int[] newHeights = new int[6];
            newHeights[corner] = cornerHeight;
            for (int i = 1; i < 6; i++) {
                newHeights[(corner + i) % 6] = Mathf.Max(heights[(corner + i) % 6], cornerHeight - heightDiffs[i]);
            }
            column.SetHeights(newHeights, false);
        }
    }
}
