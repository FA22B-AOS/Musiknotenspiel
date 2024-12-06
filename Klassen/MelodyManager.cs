using System;
using System.Collections.Generic;
using System.IO;

namespace Musiknotenspiel.Klassen
{
    internal class MelodyManager
    {
        public List<Melody> Melodies { get; set; } = new List<Melody>();
        public int MelodyCount{ get => Melodies.Count;}

        private string filePath;

        public MelodyManager(string filePath)
        {
            this.filePath = filePath;
        }

        public void AddMelody(string name, List<string> notes)
        {
            Melodies.Add(new Melody(name, notes));
        }

        public void SaveToFile()
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Name,Notes");

                foreach (var melody in Melodies)
                {
                    string notesAsString = string.Join(";", melody.Notes); 
                    writer.WriteLine($"{melody.Name},{notesAsString}");
                }
            }
        }

        public void LoadFromFile()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Die angegebene Datei wurde nicht gefunden.");

            using (StreamReader reader = new StreamReader(filePath))
            {
                Melodies.Clear();
                string? line = reader.ReadLine(); 
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',', 2); 
                    if (parts.Length == 2)
                    {
                        string name = parts[0];
                        List<string> notes = new List<string>(parts[1].Split(';')); 
                        Melodies.Add(new Melody(name, notes));
                    }
                }
            }
        }
    }

    public class Melody
    {
        public string Name { get; set; }
        public List<string> Notes { get; set; }

        public Melody() { }

        public Melody(string name, List<string> notes)
        {
            Name = name;
            Notes = notes;
        }
    }
}
