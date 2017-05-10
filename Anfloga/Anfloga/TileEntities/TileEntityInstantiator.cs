using Anfloga.DataTypes;
using FlatRedBall.TileGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Anfloga.Performance;
using FlatRedBall.Graphics;
using System.Reflection;
using TMXGlueLib.DataTypes;


namespace FlatRedBall.TileEntities
{
    public static class TileEntityInstantiator
    {

        /// <summary>
        /// Creates entities from a single layer for any tile with the EntityToCreate property.
        /// </summary>
        /// <param name="mapLayer">The layer to create entities from.</param>
        /// <param name="layeredTileMap">The map which contains the mapLayer instance.</param>
        public static void CreateEntitiesFrom(MapDrawableBatch mapLayer, LayeredTileMap layeredTileMap)
        {
            var entitiesToRemove = new List<string>();

            CreateEntitiesFrom(entitiesToRemove, mapLayer, layeredTileMap.TileProperties);

            foreach (var entityToRemove in entitiesToRemove)
            {
                string remove = entityToRemove;
                mapLayer.RemoveTiles(t => t.Any(item => item.Name == "EntityToCreate" && item.Value as string == remove), layeredTileMap.TileProperties);
            }

        }

        public static void CreateEntitiesFrom(LayeredTileMap layeredTileMap)
        {
            var entitiesToRemove = new List<string>();

            foreach (var layer in layeredTileMap.MapLayers)
            {
                CreateEntitiesFrom(entitiesToRemove, layer, layeredTileMap.TileProperties);
            }
            foreach (var entityToRemove in entitiesToRemove)
            {
                string remove = entityToRemove;
                layeredTileMap.RemoveTiles(t => t.Any(item => item.Name == "EntityToCreate" && item.Value as string == remove), layeredTileMap.TileProperties);
            }
            foreach (var shapeCollection in layeredTileMap.ShapeCollections)
            {
                var polygons = shapeCollection.Polygons;
                for (int i = polygons.Count - 1; i > -1; i--)
                {
                    var polygon = polygons[i];
                    if (!string.IsNullOrEmpty(polygon.Name) && layeredTileMap.ShapeProperties.ContainsKey(polygon.Name))
                    {
                        var properties = layeredTileMap.ShapeProperties[polygon.Name];
                        var entityAddingProperty = properties.FirstOrDefault(item => item.Name == "EntityToCreate");

                        var entityType = entityAddingProperty.Value as string;
                        if (!string.IsNullOrEmpty(entityType))
                        {
                            IEntityFactory factory = GetFactory(entityType);

                            var entity = factory.CreateNew(null) as PositionedObject;

                            entity.Name = polygon.Name;
                            ApplyPropertiesTo(entity, properties, polygon.Position);
                            shapeCollection.Polygons.Remove(polygon);

                            if (entity is Math.Geometry.ICollidable)
                            {
                                var entityCollision = (entity as Math.Geometry.ICollidable).Collision;
                                entityCollision.Polygons.Add(polygon);
                                polygon.AttachTo(entity, false);
                            }

                        }
                    }
                }
            }
        }

        private static void CreateEntitiesFrom(List<string> entitiesToRemove, MapDrawableBatch layer, Dictionary<string, List<NamedValue>> propertiesDictionary)
        {
            var flatRedBallLayer = SpriteManager.Layers.FirstOrDefault(item => item.Batches.Contains(layer));

            var dictionary = layer.NamedTileOrderedIndexes;

            // layer needs its position updated:
            layer.ForceUpdateDependencies();

            foreach (var propertyList in propertiesDictionary.Values)
            {
                if (propertyList.Any(item2 => item2.Name == "EntityToCreate"))
                {
                    var tileName = propertyList.FirstOrDefault(item => item.Name.ToLowerInvariant() == "name").Value as string;

                    var entityType = propertyList.FirstOrDefault(item => item.Name == "EntityToCreate").Value as string;

                    if (!string.IsNullOrEmpty(entityType) && dictionary.ContainsKey(tileName))
                    {
                        IEntityFactory factory = GetFactory(entityType);

                        if (factory == null)
                        {
                            string message =
                                $"The factory for entity {entityType} could not be found. To create instances of this entity, " +
                                "set its 'CreatedByOtherEntities' property to true in Glue.";
                            throw new Exception(message);
                        }
                        else
                        {
                            entitiesToRemove.Add(entityType);
                            var indexList = dictionary[tileName];


                            foreach (var tileIndex in indexList)
                            {

                                var entity = factory.CreateNew(flatRedBallLayer) as PositionedObject;

                                ApplyPropertiesTo(entity, layer, tileIndex, propertyList);
                            }

                        }
                    }
                }
            }
        }

        private static void ApplyPropertiesTo(PositionedObject entity, MapDrawableBatch layer, int tileIndex, List<NamedValue> propertiesToAssign)
        {
            int vertexIndex = tileIndex * 4;
            var dimension = 
                (layer.Vertices[vertexIndex + 1].Position - layer.Vertices[vertexIndex].Position).Length();

            float dimensionHalf = dimension / 2.0f;


            float left;
            float bottom;
            layer.GetBottomLeftWorldCoordinateForOrderedTile(tileIndex, out left, out bottom);
            Microsoft.Xna.Framework.Vector3 position = new Microsoft.Xna.Framework.Vector3(left, bottom, layer.Z);

            var bottomRight = layer.Vertices[tileIndex * 4 + 1].Position;

            float xDifference = bottomRight.X - left;
            float yDifference = bottomRight.Y - bottom;

            if(yDifference != 0 || xDifference < 0)
            {
                float angle = (float)System.Math.Atan2(yDifference, xDifference);

                entity.RotationZ = angle;

            }
            
            position += entity.RotationMatrix.Right * dimensionHalf;
            position += entity.RotationMatrix.Up * dimensionHalf;

            ApplyPropertiesTo(entity, propertiesToAssign, position);
        }

        private static void ApplyPropertiesTo(PositionedObject entity, List<NamedValue> propertiesToAssign, Microsoft.Xna.Framework.Vector3 position)
        {
            if (entity != null)
            {
                entity.Position = position;
            }

            var entityType = entity.GetType();
            var lateBinder = Instructions.Reflection.LateBinder.GetInstance(entityType);

            foreach (var property in propertiesToAssign)
            {
                var valueToSet = property.Value;
                valueToSet = SetValueAccordingToType(valueToSet, property.Name, property.Type, entityType);
                try
                {
                    lateBinder.SetValue(entity, property.Name, valueToSet);
                }
                catch (InvalidCastException e)
                {
                    string assignedType = valueToSet.GetType().ToString() ?? "unknown type";
                    assignedType = GetFriendlyNameForType(assignedType);

                    string expectedType = "unknown type";
                    object outValue;
                    if (lateBinder.TryGetValue(entity, property.Name, out outValue) && outValue != null)
                    {
                        expectedType = outValue.GetType().ToString();
                        expectedType = GetFriendlyNameForType(expectedType);
                    }

                    // This means that the property exists but is of a different type. 
                    string message = $"Attempted to assign the property {property.Name} " +
                        $"to a value of type {assignedType} but expected {expectedType}. " +
                        $"Check the property type in your TMX and make sure it matches the type on the entity.";
                    throw new Exception(message, e);
                }
                catch (Exception e)
                {
                    // Since this code indiscriminately tries to set properties, it may set properties which don't
                    // actually exist. Therefore, we tolerate failures.

                }
            }
        }


        private static string GetFriendlyNameForType(string type)
        {
            switch (type)
            {
                case "System.String": return "string";
                case "System.Single": return "float";

            }
            return type;
        }


        private static object SetValueAccordingToType(object valueToSet, string valueName, string valueType, Type entityType)
        {
            if (valueType == "bool")
            {
                bool boolValue = false;

                if (bool.TryParse((string)valueToSet, out boolValue))
                {
                    valueToSet = boolValue;
                }
            }
            else if (valueType == "float")
            {
                float floatValue;

                if (float.TryParse((string)valueToSet, out floatValue))
                {
                    valueToSet = floatValue;
                }
            }
            else if (valueType == "int")
            {
                int intValue;

                if (int.TryParse((string)valueToSet, out intValue))
                {
                    valueToSet = intValue;
                }
            }
            else if (valueName == "CurrentState")
            {
                // Since it's part of the class, it uses the "+" separator
                var enumTypeName = entityType.FullName + "+VariableState";
                var enumType = typesInThisAssembly.FirstOrDefault(item => item.FullName == enumTypeName);

                valueToSet = Enum.Parse(enumType, (string)valueToSet);
            }
            return valueToSet;
        }


        private static void AssignCustomPropertyTo(PositionedObject entity, NamedValue property)
        {
            throw new NotImplementedException();
        }


        static Type[] typesInThisAssembly;
        private static IEntityFactory GetFactory(string entityType)
        {
            if (typesInThisAssembly == null)
            {
#if WINDOWS_8 || UWP
                var assembly = typeof(TileEntityInstantiator).GetTypeInfo().Assembly;
                typesInThisAssembly = assembly.DefinedTypes.Select(item=>item.AsType()).ToArray();

#else
                var assembly = Assembly.GetExecutingAssembly();
                typesInThisAssembly = assembly.GetTypes();
#endif
            }


#if WINDOWS_8 || UWP
            var filteredTypes =
                typesInThisAssembly.Where(t => t.GetInterfaces().Contains(typeof(IEntityFactory))
                            && t.GetConstructors().Any(c=>c.GetParameters().Count() == 0));
#else
            var filteredTypes =
                typesInThisAssembly.Where(t => t.GetInterfaces().Contains(typeof(IEntityFactory))
                            && t.GetConstructor(Type.EmptyTypes) != null);
#endif

            var factories = filteredTypes
                .Select(
                    t =>
                    {
#if WINDOWS_8 || UWP
                        var propertyInfo = t.GetProperty("Self");
#else
                        var propertyInfo = t.GetProperty("Self");
#endif
                        var value = propertyInfo.GetValue(null, null);
                        return value as IEntityFactory;
                    }).ToList();


            var factory = factories.FirstOrDefault(item =>
            {
                var type = item.GetType();
                var methodInfo = type.GetMethod("CreateNew", new[] { typeof(Layer) });
                var returntypeString = methodInfo.ReturnType.Name;

                return entityType == returntypeString || entityType.EndsWith("\\" + returntypeString);
            });
            return factory;
        }
    }
}
