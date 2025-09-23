using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.Helpers;

/// <summary>
/// Custom JSON converter for IPlayer interface to handle serialization and deserialization.
/// </summary>
public class IPlayerJsonConverter : JsonConverter<IPlayer>
{
    public override void WriteJson(JsonWriter writer, IPlayer? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        // Create a JObject with the player data plus type information
        var playerObject = new JObject();

        // Add type discriminator to identify the concrete class
        playerObject["$type"] = value.GetType().AssemblyQualifiedName;

        // Serialize the actual player properties
        var playerJson = JObject.FromObject(value, serializer);

        // Merge the properties into our object
        playerObject.Merge(playerJson);

        // Write the final object
        playerObject.WriteTo(writer);
    }

    public override IPlayer ReadJson(JsonReader reader, Type objectType, IPlayer existingValue, 
        bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        // Load the JSON object
        var jsonObject = JObject.Load(reader);

        // Try to get the type discriminator
        var typeToken = jsonObject["$type"];

        if (typeToken != null)
        {
            // If we have explicit type information, use it
            var typeName = typeToken.ToString();
            var type = Type.GetType(typeName);

            if (type != null && typeof(IPlayer).IsAssignableFrom(type))
            {
                // Remove the type discriminator before deserializing
                jsonObject.Remove("$type");
                return (IPlayer)jsonObject.ToObject(type, serializer);
            }
        }

        // Fallback: Try to determine the concrete type based on properties
        // This assumes you have a concrete Player class that implements IPlayer
        var concreteType = DeterminePlayerType(jsonObject);

        if (concreteType != null)
        {
            return (IPlayer)jsonObject.ToObject(concreteType, serializer);
        }

        throw new JsonSerializationException($"Unable to determine concrete type for IPlayer. JSON: {jsonObject}");
    }

    /// <summary>
    /// Attempts to determine the concrete player type based on JSON properties
    /// Customize this method based on your actual IPlayer implementations
    /// </summary>
    private Type DeterminePlayerType(JObject jsonObject)
    {
        // Example logic - customize based on your actual concrete classes
        // This assumes you have classes like "Player", "HumanPlayer", "AIPlayer", etc.

        // Check if there's an IsHuman property to distinguish player types
        var isHumanToken = jsonObject["IsHuman"];
        if (isHumanToken != null)
        {
            bool isHuman = isHumanToken.ToObject<bool>();

            // Return appropriate concrete type based on your class hierarchy
            // You'll need to replace these with your actual class names
            if (isHuman)
            {
                return Type.GetType("Euchre.Logic.Components.HumanPlayer"); // Adjust namespace/class name
            }
            else
            {
                return Type.GetType("Euchre.Logic.Components.AutomatedPlayer"); // Adjust namespace/class name
            }
        }

        // Fallback to a default Player class
        return Type.GetType("Euchre.Logic.Components.Player"); // Adjust namespace/class name
    }
}