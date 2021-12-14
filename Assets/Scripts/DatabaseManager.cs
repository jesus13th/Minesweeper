using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DatabaseManager {
    private static string path = @$"{Application.persistentDataPath}\Minesweeper.json";
    private static Registros registers;

    public static void CreateFile() {
        if (!File.Exists(path))
            using (StreamWriter file = File.AppendText(path))
                file.WriteLine("{ \"registros\": [] }");
    }
    public static void InsertRegister(string difficulty, int time) {
        Registro newRegistro = new Registro { Dificultad = difficulty, Tiempo = time, Fecha = System.DateTime.Now.ToShortDateString() };
        using (StreamReader reader = new StreamReader(path))
            registers = JsonUtility.FromJson<Registros>(reader.ReadToEnd());

        using (StreamWriter writer = new StreamWriter(path)) {
            registers.registros.Add(newRegistro);
            writer.Write(JsonUtility.ToJson(registers));
        }
    }
    public static List<Registro> LoadRegistros() {
        using (StreamReader straem = new StreamReader(path))
            registers = JsonUtility.FromJson<Registros>(straem.ReadToEnd());

        return registers.registros;
    }
}
[System.Serializable]
public class Registro {
    public string Dificultad;
    public int Tiempo;
    public string Fecha;
    public override string ToString() => $"{Dificultad}, {Tiempo}, {Fecha}";
}
[System.Serializable]
public class Registros {
    public List<Registro> registros;
    public override string ToString() => string.Join(", ", registros);
}