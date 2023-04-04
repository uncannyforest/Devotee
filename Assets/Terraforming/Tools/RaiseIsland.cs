using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseIsland : Tool {
    public int slopedness = 5;
    public int islandIterations = 3;

    public GameObject flatLand;
    public GameObject slopeLand;
    public GameObject cliffLand;
    public GameObject cliffEndLand;
    public GameObject bridgeLand;

    override public void Load() {
        selector.transform.localScale = new Vector3(3, selector.transform.localScale.y, 3);
    }

    override public void Unload() {
        selector.transform.localScale = new Vector3(1, selector.transform.localScale.y, 1);
    }

    private void IncrementCompass(ref HexPos e, ref HexPos w, ref HexPos q, ref HexPos a, ref HexPos s, ref HexPos d) {
        e += HexPos.E;
        w += HexPos.W;
        q += HexPos.Q;
        a += HexPos.A;
        s += HexPos.S;
        d += HexPos.D;
    }

    override public bool Use() {
        RaiseGround.Use(Position);

        HexPos e = Position;
        HexPos w = Position;
        HexPos q = Position;
        HexPos a = Position;
        HexPos s = Position;
        HexPos d = Position;
        IncrementCompass(ref e, ref w, ref q, ref a, ref s, ref d);

        if (Terrain.Grid[e] == null) RaiseGround.Use(e);
        if (Terrain.Grid[w] == null) RaiseGround.Use(w);
        if (Terrain.Grid[q] == null) RaiseGround.Use(q);
        if (Terrain.Grid[a] == null) RaiseGround.Use(a);
        if (Terrain.Grid[s] == null) RaiseGround.Use(s);
        if (Terrain.Grid[d] == null) RaiseGround.Use(d);

        int yPos = 0;

        if (FlattenGround.GetHeightDifference(e, Position) < 0)
            yPos = Mathf.Max(yPos, RaiseGround.RaiseColumn(e, FlattenGround.GetHeightDifference(Position, e), null));
        if (FlattenGround.GetHeightDifference(w, Position) < 0)
            yPos = Mathf.Max(yPos, RaiseGround.RaiseColumn(w, FlattenGround.GetHeightDifference(Position, w), null));
        if (FlattenGround.GetHeightDifference(q, Position) < 0)
            yPos = Mathf.Max(yPos, RaiseGround.RaiseColumn(q, FlattenGround.GetHeightDifference(Position, q), null));
        if (FlattenGround.GetHeightDifference(a, Position) < 0)
            yPos = Mathf.Max(yPos, RaiseGround.RaiseColumn(a, FlattenGround.GetHeightDifference(Position, a), null));
        if (FlattenGround.GetHeightDifference(s, Position) < 0)
            yPos = Mathf.Max(yPos, RaiseGround.RaiseColumn(s, FlattenGround.GetHeightDifference(Position, s), null));
        if (FlattenGround.GetHeightDifference(d, Position) < 0)
            yPos = Mathf.Max(yPos, RaiseGround.RaiseColumn(d, FlattenGround.GetHeightDifference(Position, d), null));

        RaiseGround.MaybeRaiseOrigin(yPos);

        for (int i = 1; i <= islandIterations; i++) {
            bool changed = false;
            bool cornerE = false;
            bool cornerW = false;
            bool cornerQ = false;
            bool cornerA = false;
            bool cornerS = false;
            bool cornerD = false;
            changed |= FlattenRow(e + HexPos.W, HexPos.A, HexPos.S, HexPos.Q, i, ref cornerE, ref cornerW);
            changed |= FlattenRow(w + HexPos.Q, HexPos.S, HexPos.D, HexPos.A, i, ref cornerW, ref cornerQ);
            changed |= FlattenRow(q + HexPos.A, HexPos.D, HexPos.E, HexPos.S, i, ref cornerQ, ref cornerA);
            changed |= FlattenRow(a + HexPos.S, HexPos.E, HexPos.W, HexPos.D, i, ref cornerA, ref cornerS);
            changed |= FlattenRow(s + HexPos.D, HexPos.W, HexPos.Q, HexPos.E, i, ref cornerS, ref cornerD);
            changed |= FlattenRow(d + HexPos.E, HexPos.Q, HexPos.A, HexPos.W, i, ref cornerD, ref cornerE);
            if (!changed) break;
            IncrementCompass(ref e, ref w, ref q, ref a, ref s, ref d);
            if (cornerE) Flatten120Deg(e, HexPos.E);
            if (cornerW) Flatten120Deg(w, HexPos.Q);
            if (cornerQ) Flatten120Deg(q, HexPos.A);
            if (cornerA) Flatten120Deg(a, HexPos.S);
            if (cornerS) Flatten120Deg(s, HexPos.S);
            if (cornerD) Flatten120Deg(d, HexPos.D);
        }

        return true;
    }

    private bool FlattenRow(HexPos start, HexPos comparison1, HexPos comparison2, HexPos direction, int times,
            ref bool corner0, ref bool corner1) {
        bool changed = false;
        for (int i = 0; i < times; i++) {
            if (FlattenGround.GetHeightDifference(start + comparison1, start) > slopedness
                    || FlattenGround.GetHeightDifference(start + comparison2, start) > slopedness) {
                changed |= FlattenGround.Use(slopedness, start, start + comparison1, start + comparison2, Random.value < .1f,//1f / Terrain.I.randomSurface.Length,
                    flatLand, slopeLand, cliffLand, cliffEndLand, bridgeLand);
                if (i == 0) corner0 = true;
                if (i == times - 1) corner1 = true;
            }
            start = start + direction;
        }
        return changed;
    }

    private void Flatten120Deg(HexPos position, HexPos comparison) {
        comparison = comparison.Rotate(180);
        int height1 = FlattenGround.GetHeight(position + comparison.Rotate(-60), position);
        int height2 = FlattenGround.GetHeight(position + comparison.Rotate(60), position);
        // reference the larger one
        if (height2 < height1 - slopedness) {
            FlattenGround.Use(slopedness, position, position + comparison.Rotate(-60), position + comparison, Random.value < .1f,//1f / Terrain.I.randomSurface.Length,
                    flatLand, slopeLand, cliffLand, cliffEndLand, bridgeLand);
        } else if (height1 < height2 - slopedness) {
            FlattenGround.Use(slopedness, position, position + comparison, position + comparison.Rotate(60), Random.value < .5f,//1f / Terrain.I.randomSurface.Length,
                    flatLand, slopeLand, cliffLand, cliffEndLand, bridgeLand);
        } else {
            int height3 = FlattenGround.GetHeight(position - comparison, position);
            height3 = Mathf.Max(height3, height1 - Terrain.I.scale);
            height3 = Mathf.Min(height3, height1 + Terrain.I.scale);
            bool maybeFlip = Random.value > .5f;
            if (height1 > height3)
                FlattenGround.SetNewGround(position, cliffLand,
                    height3 - Mathf.FloorToInt((height1 - height3) / 2f),
                    comparison.Rotate(180).ToUnitRotation(),
                    height1 - height3,
                    maybeFlip);
            else if (height3 > height1)
                FlattenGround.SetNewGround(position, cliffLand,
                    height1 - Mathf.FloorToInt((height3 - height1) / 2f),
                    comparison.ToUnitRotation(),
                    height3 - height1,
                    maybeFlip);
            else
                FlattenGround.SetNewGround(position, flatLand,
                    height1,
                    60 * Random.Range(0, 6),
                    maybeFlip);
        }
    }
}
