using System;
using System.Text;
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

        StreamWriter file = new StreamWriter(Application.persistentDataPath 
                    + "/MySaveData.json");
        file.Write(JsonUtility.ToJson(mapData));
        file.Close();
        Debug.Log("Game data saved to " + Application.persistentDataPath + "/MySaveData.json");
    }

    public static void LoadGame() {
        if (File.Exists(Application.persistentDataPath 
                    + "/MySaveData.json")) {
            Debug.Log("Reading game data from " + Application.persistentDataPath + "/MySaveData.json");
            StreamReader file = new StreamReader(Application.persistentDataPath + "/MySaveData.json");
            string data = file.ReadToEnd();
            MapData mapData = JsonUtility.FromJson<MapData>(data);
            Debug.Log(data);
            file.Close();

            Terrain.I.PopulateTerrainFromData(mapData.land);

            // foreach (Creature.Data creature in mapData.creatures) {
            //     Instantiate(CreatureLibrary.P.BySpeciesName(creature.species),
            //             Terrain.I.CellCenter(creature.tile), Quaternion.identity, Terrain.I.transform)
            //         .DeserializeUponStart(creature);
            // }

            Debug.Log("Game data loaded!");
        } else Debug.LogError("There is no save data!");
    }
}
