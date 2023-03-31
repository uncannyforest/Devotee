using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlattenGround : Tool {
    public int slopedness = 3;
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

    public static int GetHeightDifference(HexPos position, HexPos adjacentPosition) {
        return GetHeight(position, adjacentPosition) - GetHeight(adjacentPosition, position);
    }

    public static int GetHeight(HexPos position, HexPos adjacentPosition) {
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

    override public bool Use() => Use(slopedness, Position, selector1.Position, selector2.Position, true,
        flatLand, slopeLand, cliffLand, cliffEndLand, bridgeLand);

    public static bool Use(int slopedness, HexPos position, HexPos position1, HexPos position2, bool considerBridge,
            GameObject flatLand, GameObject slopeLand, GameObject cliffLand, GameObject cliffEndLand, GameObject bridgeLand) {
        if (!Terrain.I.CanModTerrain(position)) return false;
        int height1 = GetHeight(position1, position);
        int height2 = GetHeight(position2, position);
        int height3 = GetHeight(position2 - position1 + position, position);
        int height4 = GetHeight(position * 2 - position1, position);
        int height5 = GetHeight(position * 2 - position2, position);
        int height6 = GetHeight(position1 - position2 + position, position);
        int diff = height2 - height1;
        bool maybeFlip = Random.value > .5f;
        if (Mathf.Abs(diff) > 2 * Terrain.I.scale) return false;
        else if (considerBridge && diff > Terrain.I.scale) {
            SetNewGround(position, bridgeLand,
                height1,
                (position2 - position1).ToUnitRotation(),
                Mathf.FloorToInt(diff / 2f),
                true);
        } else if (considerBridge && diff < -Terrain.I.scale) {
            SetNewGround(position, bridgeLand,
                height2,
                (position2 - position1).ToUnitRotation(),
                Mathf.FloorToInt(diff / -2f),
                false);
        } else if (diff > slopedness) {
            if (height2 - diff / 4f < height3 && height6 < height1 + diff / 4f)
                SetNewGround(position, cliffLand,
                    height1 - Mathf.FloorToInt(diff / 2f),
                    (position1 - position2).ToUnitRotation(),
                    diff,
                    maybeFlip);
            // else if (diff <= Terrain.I.scale / 2 && height4 <= height1 && height5 <= height1 - diff)
            //     SetNewGround(position, slopeLand,
            //         height1 - diff * 2,
            //         (position1 - position).ToUnitRotation() - (maybeFlip ? 180 : 0),
            //         diff * 2,
            //         maybeFlip);
            // else if (diff <= Terrain.I.scale / 2 && height5 >= height2 && height4 >= height2 + diff)
            //     SetNewGround(position, slopeLand,
            //         height1 - diff,
            //         (position2 - position).ToUnitRotation() - (maybeFlip ? 180 : 0),
            //         diff * 2,
            //         maybeFlip);
            else
                SetNewGround(position, cliffEndLand,
                    height1 - Mathf.FloorToInt(diff / 2f),
                    (position2 - position1).ToUnitRotation(),
                    diff,
                    false);
        } else if (diff < -slopedness) {
            if (height1 - diff / 4f < height6 && height3 < height2 + diff / 4f)
                SetNewGround(position, cliffLand,
                    height2 - Mathf.FloorToInt(-diff / 2f),
                    (position2 - position1).ToUnitRotation(),
                    -diff,
                    maybeFlip);
            // else if (-diff <= Terrain.I.scale / 2 && height5 <= height2 && height4 <= height2 + diff)
            //     SetNewGround(position, slopeLand,
            //         height2 + diff * 2,
            //         (position - position2).ToUnitRotation() - (maybeFlip ? 180 : 0),
            //         -diff * 2,
            //         maybeFlip);
            // else if (-diff <= Terrain.I.scale / 2 && height4 >= height1 && height5 >= height1 - diff)
            //     SetNewGround(position, slopeLand,
            //         height2 + diff,
            //         (position - position1).ToUnitRotation() - (maybeFlip ? 180 : 0),
            //         -diff * 2,
            //         maybeFlip);
            else
                SetNewGround(position, cliffEndLand,
                    height2 - Mathf.FloorToInt(-diff / 2f),
                    (position1 - position2).ToUnitRotation(),
                    -diff,
                    true);
        } else {
            // round down if diff == 1, up otherwise
            int height0 = Mathf.Abs(diff) <= 1 ? Mathf.Min(height1, height2) : Mathf.CeilToInt((height1 + height2) / 2f);
            if (height3 < height0 && height6 > height0) {
                int halfDiff = Mathf.Min((height0 - height3)*2, (height6 - height0)*2, Terrain.I.scale / 2);
                SetNewGround(position, cliffEndLand,
                    height0 - halfDiff * 2,
                    (position1 - position2).ToUnitRotation(),
                    halfDiff * 2,
                    false);
            } else if (height6 < height0 && height0 < height3) {
                int halfDiff = Mathf.Min((height0 - height6)*2, (height3 - height0)*2, Terrain.I.scale / 2);
                SetNewGround(position, cliffEndLand,
                    height0 - halfDiff * 2,
                    (position2 - position1).ToUnitRotation(),
                    halfDiff * 2,
                    true);
            } else if (height4 < height0 && height5 < height0) {
                diff = Mathf.Min(height0 - height4, height0 - height5, Terrain.I.scale);
                if (height3 <= height0 - diff && height6 >= height0)
                    SetNewGround(position, cliffLand,
                        height0 - diff - Mathf.FloorToInt(diff / 2f),
                        (position - position1).ToUnitRotation(),
                        diff,
                        maybeFlip);
                else if (height3 >= height0 && height6 <= height0 - diff)
                    SetNewGround(position, cliffLand,
                        height0 - diff - Mathf.FloorToInt(diff / 2f),
                        (position - position2).ToUnitRotation(),
                        diff,
                        maybeFlip);
                else
                    SetNewGround(position, slopeLand,
                        height0 - diff - Mathf.FloorToInt(diff / 2f),
                        (position1 - position2).ToUnitRotation() - (maybeFlip ? 180 : 0),
                        diff,
                        maybeFlip);
            } else if (height4 > height0 && height5 > height0) {
                diff = Mathf.Min(height4 - height0, height5 - height0, Terrain.I.scale);
                if (height3 >= height0 + diff && height6 <= height0)
                    SetNewGround(position, cliffLand,
                        height0 - Mathf.FloorToInt(diff / 2f),
                        (position1 - position).ToUnitRotation(),
                        diff,
                        maybeFlip);
                else if (height3 <= height0 && height6 >= height0 + diff)
                    SetNewGround(position, cliffLand,
                        height0 - Mathf.FloorToInt(diff / 2f),
                        (position2 - position).ToUnitRotation(),
                        diff,
                        maybeFlip);
                else
                    SetNewGround(position, slopeLand,
                        height0 - Mathf.FloorToInt(diff / 2f),
                        (position2 - position1).ToUnitRotation() - (maybeFlip ? 180 : 0),
                        diff,
                        maybeFlip);
            } else {
                SetNewGround(position, flatLand,
                    height0,
                    60 * Random.Range(0, 6),
                    maybeFlip);
            }
        }
        return true;
    }
    
    public static void SetNewGround(HexPos position, GameObject prefab, int elevationBase, int rotation, int elevationScale, bool flip) {
        if (!Terrain.I.CanModTerrain(position)) return;
        Debug.Log(elevationBase + " " + rotation + " " + elevationScale + " " + flip);

        if (Terrain.Grid[position] == null) {
            int height = elevationScale;
            Vector3 initialPosition = Quaternion.Euler(0, -30, 0) * (Vector3)(position) * Terrain.I.scale + Vector3.up * elevationBase;
            Transform column = GameObject.Instantiate(
                Terrain.I.columnPrefab,
                initialPosition,
                Quaternion.identity,
                Terrain.I.transform
            ).transform;
            column.gameObject.name = position.ToString();
            Transform surface = GameObject.Instantiate(
                prefab,
                initialPosition,
                Quaternion.Euler(0, -rotation, 0),
                column).transform;
            surface.localScale = new Vector3(Terrain.I.scale, height, flip ? -Terrain.I.scale : Terrain.I.scale);
            Terrain.Grid[position] = column;
            RaiseGround.ExtendColumn(position, 0, null);
        } else {
            Transform column = Terrain.Grid[position];
            int oldElevationBase = Mathf.FloorToInt(column.GetChild(0).position.y);
            int comingRigidbodyMove = elevationBase - oldElevationBase;
            if (comingRigidbodyMove > 0) {
                RaiseGround.RaiseColumn(position, comingRigidbodyMove, null);
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

    public static void SetNewGround(HexPos position, GameObject prefab, int elevation, int rotation, bool flip) {
        if (!Terrain.I.CanModTerrain(position)) return;
        Debug.Log(elevation + " " + rotation + " " + flip);

        if (Terrain.Grid[position] == null) {
            int height = Random.Range(1, Terrain.I.scale + 1);
            Vector3 initialPosition = Quaternion.Euler(0, -30, 0) * (Vector3)(position) * Terrain.I.scale
                + Vector3.up * (elevation - height);
            Transform column = GameObject.Instantiate(
                Terrain.I.columnPrefab,
                initialPosition,
                Quaternion.identity,
                Terrain.I.transform
            ).transform;
            column.gameObject.name = position.ToString();
            Transform surface = GameObject.Instantiate(
                prefab,
                initialPosition,
                Quaternion.Euler(0, -rotation, 0),
                column).transform;
            surface.localScale = new Vector3(Terrain.I.scale, height, flip ? -Terrain.I.scale : Terrain.I.scale);
            Terrain.Grid[position] = column;
            RaiseGround.ExtendColumn(position, 0, null);
        } else {
            Transform column = Terrain.Grid[position];
            int baseElevation = Mathf.FloorToInt(column.GetChild(0).position.y);
            int intendedHeight = elevation - baseElevation;
            int height;
            int comingRigidbodyMove = 0;
            if (intendedHeight > Terrain.I.scale) {
                height = Random.Range(1, Terrain.I.scale + 1);
                comingRigidbodyMove = intendedHeight - height;
                baseElevation += comingRigidbodyMove;
                RaiseGround.RaiseColumn(position, comingRigidbodyMove, null);
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
