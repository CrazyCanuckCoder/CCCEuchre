using System.IO;
using Newtonsoft.Json;

namespace Euchre.Logic;

public class DataPersistence
{
    /// <summary>
    /// Method to save an object to a file using JSON serialization.
    /// </summary>
    /// <typeparam name="T">The type of the instance to save.</typeparam>
    /// <param name="objectToSave">The instance of the class to save.</param>
    /// <param name="filePath">The full path to the file to save the object to.</param>
    public static void SaveToFile<T>(T objectToSave, string filePath) where T : new()
    {
        // Serialize the object to JSON format.

        string jsonText = JsonConvert.SerializeObject(objectToSave, Formatting.Indented);

        // Write the JSON to the file.

        File.WriteAllText(filePath, jsonText);
    }

    /// <summary>
    /// Method to load an object from a JSON file.
    /// </summary>
    /// <typeparam name="T">The type of the instance to load.</typeparam>
    /// <param name="filePath">The full path to the file to load the object from.</param>
    /// <returns>An instance of the object</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    public static T LoadFromFile<T>(string filePath) where T : new()
    {
        // Check if the file exists.

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file {filePath} was not found.");
        }

        // Read the JSON from the file.

        string json = File.ReadAllText(filePath);

        // Deserialize the JSON back to the object.

        T deserializedObject = JsonConvert.DeserializeObject<T>(json) ?? new();

        return deserializedObject;
    }
}
