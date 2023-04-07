using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid<T> {
    private List<List<T>> quad1 = new List<List<T>>();
    private List<List<T>> quad2 = new List<List<T>>();
    private List<List<T>> quad3 = new List<List<T>>();
    private List<List<T>> quad4 = new List<List<T>>();

    public List<List<List<T>>> Quads {
        get => new List<List<List<T>>>() { quad1, quad2, quad3, quad4 };
    }

    private T get2D(List<List<T>> list2d, int x, int y) {
        if (list2d.Count <= x) return default(T);
        List<T> list = list2d[x];
        if (list.Count <= y) return default(T);
        return list[y];
    }

    private void set2D(List<List<T>> list2d, int x, int y, T value) {
        while (list2d.Count <= x) list2d.Add(new List<T>());
        List<T> list = list2d[x];
        while (list.Count <= y) list.Add(default(T));
        list[y] = value;
    }

    public T this[HexPos pos] {
        get {
            if (pos.x >= 0) {
                if (pos.y >= 0) return get2D(quad1, pos.x, pos.y);
                else return get2D(quad4, pos.x, -1 - pos.y);
            } else {
                if (pos.y >= 0) return get2D(quad2, -1 - pos.x, pos.y);
                else return get2D(quad3, -1 - pos.x, -1 - pos.y);
            }
        }
        set {
            if (pos.x >= 0) {
                if (pos.y >= 0) set2D(quad1, pos.x, pos.y, value);
                else set2D(quad4, pos.x, -1 - pos.y, value);
            } else {
                if (pos.y >= 0) set2D(quad2, -1 - pos.x, pos.y, value);
                else set2D(quad3, -1 - pos.x, -1 - pos.y, value);
            }
        }
    }
}
