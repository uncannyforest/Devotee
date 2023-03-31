using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlattenGround : Tool {
    public Material selectorMaterial1;
    public Material selectorMaterial2;
    public Color adjacentColor;
    public Color selectorInvalid;
    public GameObject flatLand;
    public GameObject slopeLand;
    public GameObject cliffLand;
    public GameObject cliffEndLand;
    public GameObject bridgeLand;

    private Selector selector1;
    private Selector selector2;

    void Start() {
        selector = GameObject.Find("Selector").GetComponent<Selector>();
    }

    override public void Load() {
        selector1 = GameObject.Instantiate(selector.gameObject, selector.transform.parent).GetComponent<Selector>();
        selector2 = GameObject.Instantiate(selector.gameObject, selector.transform.parent).GetComponent<Selector>();
        selector1.Position = selector.Position + HexPos.D;
        selector2.Position = selector.Position + HexPos.E;
        selector1.Color = adjacentColor;
        selector2.Color = adjacentColor;
    }

    override public void Unload() {
        GameObject.Destroy(selector1.gameObject);
        GameObject.Destroy(selector2.gameObject);
    }

    private int GetHeight(HexPos position, HexPos adjacentPosition) {
        Transform column = Terrain.Grid[position];
        if (column == null) return 0;
        int level = Mathf.FloorToInt(column.GetChild(0)
            .GetComponent<EdgeLevels>().GetLevel(adjacentPosition - position));
        return level > 0 ? level : 0;
    }

    public override void WillUpdatePos(HexPos pos) {
        if (pos == selector1.Position) {
            selector1.Position = selector2.Position;
            selector2.Position = Position;
        } else if (pos == selector2.Position) {
            selector2.Position = selector1.Position;
            selector1.Position = Position;
        } else if (pos - selector1.Position == Position - selector2.Position) {
            selector2.Position = Position;
        } else if (pos - selector2.Position == Position - selector1.Position) {
            selector1.Position = Position;
        } else {
            HexPos diff = pos - Position;
            selector1.Position += diff;
            selector2.Position += diff;
        }
        int height1 = GetHeight(selector1.Position, pos);
        int height2 = GetHeight(selector2.Position, pos);
        Debug.Log("Heights: " + height1 + ", " + height2);
        if (Mathf.Abs(height1 - height2) > 2 * Terrain.I.scale) {
            selector1.Color = selectorInvalid;
            selector2.Color = selectorInvalid;
        } else {
            selector1.Color = adjacentColor;
            selector2.Color = adjacentColor;
        }
    }

    override public bool Use() => Use(3);

    public bool Use(int slopedness) {
        if (!Terrain.I.CanModTerrain(Position)) return false;
        int height1 = GetHeight(selector1.Position, Position);
        int height2 = GetHeight(selector2.Position, Position);
        int height3 = GetHeight(selector2.Position - selector1.Position + Position, Position);
        int height4 = GetHeight(Position * 2 - selector1.Position, Position);
        int height5 = GetHeight(Position * 2 - selector2.Position, Position);
        int height6 = GetHeight(selector1.Position - selector2.Position + Position, Position);
        int diff = height2 - height1;
        bool maybeFlip = Random.value > .5f;
        Debug.Log("Heights: " + height1 + ", " + height2);
        if (Mathf.Abs(diff) > 2 * Terrain.I.scale) return false;
        else if (diff > Terrain.I.scale) {
            SetNewGround(bridgeLand,
                height1,
                (selector2.Position - selector1.Position).ToUnitRotation(),
                Mathf.FloorToInt(diff / 2f),
                true);
        } else if (diff < -Terrain.I.scale) {
            SetNewGround(bridgeLand,
                height2,
                (selector2.Position - selector1.Position).ToUnitRotation(),
                Mathf.FloorToInt(diff / -2f),
                false);
        } else if (diff > slopedness) {
            if (height2 - diff / 4f < height3 && height6 < height1 + diff / 4f)
                SetNewGround(cliffLand,
                    height1 - Mathf.FloorToInt(diff / 2f),
                    (selector1.Position - selector2.Position).ToUnitRotation(),
                    diff,
                    maybeFlip);
            // else if (diff <= Terrain.I.scale / 2 && height4 <= height1 && height5 <= height1 - diff)
            //     SetNewGround(slopeLand,
            //         height1 - diff * 2,
            //         (selector1.Position - Position).ToUnitRotation() - (maybeFlip ? 180 : 0),
            //         diff * 2,
            //         maybeFlip);
            // else if (diff <= Terrain.I.scale / 2 && height5 >= height2 && height4 >= height2 + diff)
            //     SetNewGround(slopeLand,
            //         height1 - diff,
            //         (selector2.Position - Position).ToUnitRotation() - (maybeFlip ? 180 : 0),
            //         diff * 2,
            //         maybeFlip);
            else
                SetNewGround(cliffEndLand,
                    height1 - Mathf.FloorToInt(diff / 2f),
                    (selector2.Position - selector1.Position).ToUnitRotation(),
                    diff,
                    false);
        } else if (diff < -slopedness) {
            if (height1 - diff / 4f < height6 && height3 < height2 + diff / 4f)
                SetNewGround(cliffLand,
                    height2 - Mathf.FloorToInt(-diff / 2f),
                    (selector2.Position - selector1.Position).ToUnitRotation(),
                    -diff,
                    maybeFlip);
            // else if (-diff <= Terrain.I.scale / 2 && height5 <= height2 && height4 <= height2 + diff)
            //     SetNewGround(slopeLand,
            //         height2 + diff * 2,
            //         (Position - selector2.Position).ToUnitRotation() - (maybeFlip ? 180 : 0),
            //         -diff * 2,
            //         maybeFlip);
            // else if (-diff <= Terrain.I.scale / 2 && height4 >= height1 && height5 >= height1 - diff)
            //     SetNewGround(slopeLand,
            //         height2 + diff,
            //         (Position - selector1.Position).ToUnitRotation() - (maybeFlip ? 180 : 0),
            //         -diff * 2,
            //         maybeFlip);
            else
                SetNewGround(cliffEndLand,
                    height2 - Mathf.FloorToInt(-diff / 2f),
                    (selector1.Position - selector2.Position).ToUnitRotation(),
                    -diff,
                    true);
        } else {
            // round down if diff == 1, up otherwise
            int height0 = Mathf.Abs(diff) <= 1 ? Mathf.Min(height1, height2) : Mathf.CeilToInt((height1 + height2) / 2f);
            if (height3 < height0 && height6 > height0) {
                int halfDiff = Mathf.Min((height0 - height3)*2, (height6 - height0)*2, Terrain.I.scale / 2);
                SetNewGround(cliffEndLand,
                    height0 - halfDiff * 2,
                    (selector1.Position - selector2.Position).ToUnitRotation(),
                    halfDiff * 2,
                    false);
            } else if (height6 < height0 && height0 < height3) {
                int halfDiff = Mathf.Min((height0 - height6)*2, (height3 - height0)*2, Terrain.I.scale / 2);
                SetNewGround(cliffEndLand,
                    height0 - halfDiff * 2,
                    (selector2.Position - selector1.Position).ToUnitRotation(),
                    halfDiff * 2,
                    true);
            } else if (height4 < height0 && height5 < height0) {
                diff = Mathf.Min(height0 - height4, height0 - height5, Terrain.I.scale);
                if (height3 <= height0 - diff && height6 >= height0)
                    SetNewGround(cliffLand,
                        height0 - diff - Mathf.FloorToInt(diff / 2f),
                        (Position - selector1.Position).ToUnitRotation(),
                        diff,
                        maybeFlip);
                else if (height3 >= height0 && height6 <= height0 - diff)
                    SetNewGround(cliffLand,
                        height0 - diff - Mathf.FloorToInt(diff / 2f),
                        (Position - selector2.Position).ToUnitRotation(),
                        diff,
                        maybeFlip);
                else
                    SetNewGround(slopeLand,
                        height0 - diff - Mathf.FloorToInt(diff / 2f),
                        (selector1.Position - selector2.Position).ToUnitRotation() - (maybeFlip ? 180 : 0),
                        diff,
                        maybeFlip);
            } else if (height4 > height0 && height5 > height0) {
                diff = Mathf.Min(height4 - height0, height5 - height0, Terrain.I.scale);
                if (height3 >= height0 + diff && height6 <= height0)
                    SetNewGround(cliffLand,
                        height0 - Mathf.FloorToInt(diff / 2f),
                        (selector1.Position - Position).ToUnitRotation(),
                        diff,
                        maybeFlip);
                else if (height3 <= height0 && height6 >= height0 + diff)
                    SetNewGround(cliffLand,
                        height0 - Mathf.FloorToInt(diff / 2f),
                        (selector2.Position - Position).ToUnitRotation(),
                        diff,
                        maybeFlip);
                else
                    SetNewGround(slopeLand,
                        height0 - Mathf.FloorToInt(diff / 2f),
                        (selector2.Position - selector1.Position).ToUnitRotation() - (maybeFlip ? 180 : 0),
                        diff,
                        maybeFlip);
            } else {
                SetNewGround(flatLand,
                    height0,
                    60 * Random.Range(0, 6),
                    maybeFlip);
            }
        }
        return true;
    }
    
    private void SetNewGround(GameObject prefab, int elevationBase, int rotation, int elevationScale, bool flip) {
        Debug.Log(elevationBase + " " + rotation + " " + elevationScale + " " + flip);

        if (Terrain.Grid[Position] == null) {
            int height = elevationScale;
            Vector3 initialPosition = Quaternion.Euler(0, -30, 0) * (Vector3)(Position) * Terrain.I.scale + Vector3.up * elevationBase;
            Transform column = GameObject.Instantiate(
                Terrain.I.columnPrefab,
                initialPosition,
                Quaternion.identity,
                Terrain.I.transform
            ).transform;
            column.gameObject.name = Position.ToString();
            Transform surface = GameObject.Instantiate(
                prefab,
                initialPosition,
                Quaternion.Euler(0, -rotation, 0),
                column).transform;
            surface.localScale = new Vector3(Terrain.I.scale, height, flip ? -Terrain.I.scale : Terrain.I.scale);
            Terrain.Grid[Position] = column;
            RaiseGround.ExtendColumn(Position, 0, null);
        } else {
            Transform column = Terrain.Grid[Position];
            int oldElevationBase = Mathf.FloorToInt(column.GetChild(0).position.y);
            int comingRigidbodyMove = elevationBase - oldElevationBase;
            if (comingRigidbodyMove > 0) {
                RaiseGround.RaiseColumn(Position, comingRigidbodyMove, null);
            } else if (comingRigidbodyMove < 0) {
                column.GetComponent<Rigidbody>().MovePosition(column.position + comingRigidbodyMove * Vector3Int.up);
            }

            Vector3 originalPosition = column.GetChild(0).position;
            GameObject.Destroy(column.GetChild(0).gameObject);
            Transform surface = GameObject.Instantiate(
                prefab,
                new Vector3(originalPosition.x, elevationBase - comingRigidbodyMove, originalPosition.z),
                Quaternion.Euler(0, -rotation, 0),
                column).transform;
            surface.localScale = new Vector3(Terrain.I.scale, elevationScale, flip ? -Terrain.I.scale : Terrain.I.scale);
            surface.SetAsFirstSibling();
        }

    }

    private void SetNewGround(GameObject prefab, int elevation, int rotation, bool flip) {
        Debug.Log(elevation + " " + rotation + " " + flip);

        if (Terrain.Grid[Position] == null) {
            int height = Random.Range(1, Terrain.I.scale + 1);
            Vector3 initialPosition = Quaternion.Euler(0, -30, 0) * (Vector3)(Position) * Terrain.I.scale
                + Vector3.up * (elevation - height);
            Transform column = GameObject.Instantiate(
                Terrain.I.columnPrefab,
                initialPosition,
                Quaternion.identity,
                Terrain.I.transform
            ).transform;
            column.gameObject.name = Position.ToString();
            Transform surface = GameObject.Instantiate(
                prefab,
                initialPosition,
                Quaternion.Euler(0, -rotation, 0),
                column).transform;
            surface.localScale = new Vector3(Terrain.I.scale, height, flip ? -Terrain.I.scale : Terrain.I.scale);
            Terrain.Grid[Position] = column;
            RaiseGround.ExtendColumn(Position, 0, null);
        } else {
            Transform column = Terrain.Grid[Position];
            int baseElevation = Mathf.FloorToInt(column.GetChild(0).position.y);
            int intendedHeight = elevation - baseElevation;
            int height;
            int comingRigidbodyMove = 0;
            if (intendedHeight > Terrain.I.scale) {
                height = Random.Range(1, Terrain.I.scale + 1);
                comingRigidbodyMove = intendedHeight - height;
                baseElevation += comingRigidbodyMove;
                RaiseGround.RaiseColumn(Position, comingRigidbodyMove, null);
            } else if (intendedHeight > 0) {
                height = intendedHeight;
            } else {
                height = Random.Range(1, Terrain.I.scale + 1);
                comingRigidbodyMove = intendedHeight - height;
                baseElevation += comingRigidbodyMove;
                column.GetComponent<Rigidbody>().MovePosition(column.position + comingRigidbodyMove * Vector3Int.up);
            }

            Vector3 originalPosition = column.GetChild(0).position;
            GameObject.Destroy(column.GetChild(0).gameObject);
            Transform surface = GameObject.Instantiate(
                prefab,
                new Vector3(originalPosition.x, baseElevation - comingRigidbodyMove, originalPosition.z),
                Quaternion.Euler(0, 60 * Random.Range(0, 6), 0),
                column).transform;
            surface.localScale = new Vector3(Terrain.I.scale, height, flip ? -Terrain.I.scale : Terrain.I.scale);
            surface.SetAsFirstSibling();
        }
    }
}
