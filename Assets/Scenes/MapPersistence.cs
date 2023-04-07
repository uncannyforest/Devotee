using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Linq;

[Serializable]
public struct MapData {
    public Column.Data[] land;

    public MapData(Column.Data[] land) {
        this.land = land;
    }
}

public class MapPersistence : MonoBehaviour {

    void Update() {
        if (Input.GetKeyDown("l")) SaveGame();
    }
    public static void SaveGame() {
        List<Column.Data> land = new List<Column.Data>();
        foreach (Transform child in Terrain.I.transform)
            land.Add(child.GetComponent<Column>().Serialize()); 

        MapData mapData = new MapData(land.ToArray());

        Debug.Log(JsonUtility.ToJson(mapData));

        // BinaryFormatter bf = new BinaryFormatter(); 
        // FileStream file = File.Create(Application.persistentDataPath 
        //             + "/MySaveData.dat");
        // bf.Serialize(file, mapData);
        // file.Close();
        // Debug.Log("Game data saved to " + Application.persistentDataPath + "/MySaveData.dat");
    }

    public static void LoadGame() {
        // if (File.Exists(Application.persistentDataPath 
        //             + "/MySaveData.dat")) {
        //     BinaryFormatter bf = new BinaryFormatter();
        //     FileStream file = File.Open(Application.persistentDataPath 
        //             + "/MySaveData.dat", FileMode.Open);
        //     MapData mapData = (MapData)bf.Deserialize(file);
        //     Debug.Log(mapData);
        //     file.Close();

        //     Terrain.I.PopulateTerrainFromData(mapData);

        //     foreach (Creature.Data creature in mapData.creatures) {
        //         Instantiate(CreatureLibrary.P.BySpeciesName(creature.species),
        //                 Terrain.I.CellCenter(creature.tile), Quaternion.identity, Terrain.I.transform)
        //             .DeserializeUponStart(creature);
        //     }

        //     Debug.Log("Game data loaded!");
        // } else Debug.LogError("There is no save data!");
    }
}
