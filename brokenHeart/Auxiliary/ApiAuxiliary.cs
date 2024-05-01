using System.Collections;
using System.Data;
using System.Reflection;
using brokenHeart.DB;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace brokenHeart.Auxiliary
{
    public class ApiAuxiliary
    {
        public static dynamic GetEntityPrepare(dynamic requestEntity)
        {
            List<PropertyInfo> allProperties = new List<PropertyInfo>(
                requestEntity.GetType().GetProperties()
            );
            Dictionary<PropertyInfo, PropertyInfo> relationSets =
                new Dictionary<PropertyInfo, PropertyInfo>();

            //Add all Relation-Id-List properties as keys, add the actual object-List properties as values
            foreach (
                PropertyInfo p in allProperties.Where(x =>
                    typeof(IEnumerable).IsAssignableFrom(x.PropertyType)
                )
            )
            {
                if (p.Name.EndsWith("Ids"))
                {
                    relationSets.Add(p, allProperties.Single(x => x.Name == p.Name[..^3]));
                }
            }

            foreach (KeyValuePair<PropertyInfo, PropertyInfo> relationSet in relationSets)
            {
                ICollection<int> ids = new List<int>();
                foreach (dynamic item in relationSet.Value.GetValue(requestEntity))
                {
                    ids.Add(item.Id);
                }
                relationSet.Key.SetValue(requestEntity, ids);
            }

            return requestEntity;
        }

        public static void PatchEntity(
            BrokenDbContext context,
            Type entityType,
            dynamic requestEntity,
            List<Operation> operations
        )
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    List<PropertyInfo> allProperties = entityType.GetProperties().ToList();
                    Dictionary<PropertyInfo, PropertyInfo> relationSets =
                        new Dictionary<PropertyInfo, PropertyInfo>();

                    //Add all Relation-Id-List properties as keys, add the actual object-List properties as values
                    foreach (
                        PropertyInfo p in allProperties.Where(x =>
                            typeof(IEnumerable).IsAssignableFrom(x.PropertyType)
                        )
                    )
                    {
                        if (p.Name.EndsWith("Ids"))
                        {
                            relationSets.Add(p, allProperties.Single(x => x.Name == p.Name[..^3]));
                        }
                    }

                    foreach (Operation operation in operations)
                    {
                        if (operation.value != null)
                        {
                            if (typeof(long).IsAssignableFrom(operation.value.GetType()))
                            {
                                operation.value = Convert.ToInt32(operation.value);
                            }

                            if (typeof(string).IsAssignableFrom(operation.value.GetType()))
                            {
                                var property = allProperties
                                    .Single(x =>
                                        x.Name.ToLower() == operation.path.Substring(1).ToLower()
                                    )
                                    .PropertyType;
                                if (property.Name == "Int32" || property.Name == "Decimal")
                                {
                                    operation.value = (int)
                                        new DataTable().Compute((string)operation.value, null);
                                }
                            }
                        }

                        if (operation.op == "replace")
                        {
                            foreach (
                                PropertyInfo property in allProperties.Where(x =>
                                    operation
                                        .path[1..]
                                        .Equals(x.Name, StringComparison.OrdinalIgnoreCase)
                                )
                            )
                            {
                                if (property.Name.EndsWith("Ids"))
                                {
                                    PropertyInfo relationObjects = relationSets[property];
                                    Type emptyList = typeof(List<>).MakeGenericType(
                                        relationObjects.GetValue(requestEntity).GetType()
                                    );
                                    var variable = Activator.CreateInstance(
                                        emptyList.GetGenericArguments()[0]
                                    );
                                    relationObjects.SetValue(requestEntity, variable);

                                    property.SetValue(
                                        requestEntity,
                                        (operation.value as JArray).ToObject<ICollection<int>>()
                                    );
                                }
                                else
                                {
                                    if (typeof(ICollection).IsAssignableFrom(property.PropertyType))
                                    {
                                        var value = (operation.value as JArray);
                                        dynamic setValue = value
                                            .GetType()
                                            .GetMethod(nameof(value.ToObject), new Type[0])
                                            .MakeGenericMethod(property.PropertyType)
                                            .Invoke(value, null);

                                        property.SetValue(requestEntity, setValue);
                                    }
                                    else
                                    {
                                        dynamic value;
                                        if (
                                            property.PropertyType.IsGenericType
                                            && property
                                                .PropertyType.GetGenericTypeDefinition()
                                                .Equals(typeof(Nullable<>))
                                        )
                                        {
                                            if (operation.value == null)
                                            {
                                                value = null;
                                            }
                                            else
                                            {
                                                value = Convert.ChangeType(
                                                    operation.value,
                                                    Nullable.GetUnderlyingType(
                                                        property.PropertyType
                                                    )
                                                );
                                            }
                                        }
                                        else
                                        {
                                            if (property.PropertyType.IsEnum)
                                            {
                                                value = Enum.ToObject(
                                                    property.PropertyType,
                                                    operation.value
                                                );
                                            }
                                            else
                                            {
                                                value = Convert.ChangeType(
                                                    operation.value,
                                                    property.PropertyType
                                                );
                                            }
                                        }
                                        property.SetValue(requestEntity, value);
                                    }
                                }
                            }
                        }
                        else if (operation.op == "add")
                        {
                            foreach (
                                PropertyInfo property in allProperties.Where(x =>
                                    operation
                                        .path[1..]
                                        .Equals(x.Name + "/-", StringComparison.OrdinalIgnoreCase)
                                )
                            )
                            {
                                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                {
                                    var collectionProperty = property.GetValue(requestEntity);
                                    object addValue = operation.value;

                                    typeof(ICollection<>)
                                        .MakeGenericType(
                                            property.PropertyType.GetGenericArguments()[0]
                                        )
                                        .GetMethod("Add")
                                        .Invoke(collectionProperty, new object[] { addValue });

                                    property.SetValue(requestEntity, collectionProperty);
                                }
                            }
                        }
                        else if (
                            operation.op == "remove"
                            && operation.path.Count(x => x == '/') == 2
                        )
                        {
                            foreach (
                                PropertyInfo propertyIds in allProperties.Where(x =>
                                    operation
                                        .path[1..]
                                        .StartsWith(x.Name, StringComparison.OrdinalIgnoreCase)
                                    && x.Name.EndsWith("Ids")
                                )
                            )
                            {
                                if (typeof(IEnumerable).IsAssignableFrom(propertyIds.PropertyType))
                                {
                                    PropertyInfo propertyObjects = relationSets[propertyIds];
                                    var collectionProperty = propertyObjects.GetValue(
                                        requestEntity
                                    );

                                    foreach (var item in collectionProperty)
                                    {
                                        if (
                                            item.Id
                                            == int.Parse(
                                                operation.path.Substring(
                                                    operation.path.LastIndexOf('/') + 1
                                                )
                                            )
                                        )
                                        {
                                            typeof(ICollection<>)
                                                .MakeGenericType(
                                                    propertyObjects.PropertyType.GetGenericArguments()[
                                                        0
                                                    ]
                                                )
                                                .GetMethod("Remove")
                                                .Invoke(collectionProperty, new object[] { item });
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    try
                    {
                        context.SaveChangesSimple();
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    //Add all related objects noted as ids in the API-call as full objects to the database entity
                    foreach (PropertyInfo propertyIds in relationSets.Keys)
                    {
                        var properties = relationSets[propertyIds].GetValue(requestEntity);
                        if (properties != null)
                        {
                            int previousObjectCount = relationSets[propertyIds]
                                .GetValue(requestEntity)
                                .Count;

                            foreach (
                                var item in (
                                    (IEnumerable<dynamic>)
                                        context
                                            .GetType()
                                            .GetMethod(nameof(context.Set), new Type[0])
                                            .MakeGenericMethod(
                                                relationSets[propertyIds]
                                                    .PropertyType.GetGenericArguments()[0]
                                            )
                                            .Invoke(context, null)
                                ).Where(x =>
                                    (
                                        (ICollection<int>)propertyIds.GetValue(requestEntity)
                                    ).Contains(x.Id)
                                )
                            )
                            {
                                object child = relationSets[propertyIds].GetValue(requestEntity);
                                typeof(ICollection<>)
                                    .MakeGenericType(
                                        relationSets[propertyIds]
                                            .PropertyType.GetGenericArguments()[0]
                                    )
                                    .GetMethod("Add")
                                    .Invoke(child, new object[] { item });
                                relationSets[propertyIds].SetValue(requestEntity, child);
                            }

                            if (
                                propertyIds.GetValue(requestEntity).Count
                                != (
                                    relationSets[propertyIds].GetValue(requestEntity).Count
                                    - previousObjectCount
                                )
                            )
                            {
                                throw new Exception();
                            }
                        }
                    }

                    context.Update(requestEntity);

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    // Rollback all changes
                    transaction.Rollback();

                    throw;
                }
            }
        }

        public static dynamic PostEntity(
            BrokenDbContext context,
            Type entityType,
            dynamic requestEntity
        )
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    List<PropertyInfo> allProperties = entityType.GetProperties().ToList();
                    Dictionary<PropertyInfo, PropertyInfo> relationSets =
                        new Dictionary<PropertyInfo, PropertyInfo>();

                    //Add all Relation-Id-List properties as keys, add the actual object-List properties as values
                    foreach (
                        PropertyInfo p in allProperties.Where(x =>
                            typeof(IEnumerable).IsAssignableFrom(x.PropertyType)
                        )
                    )
                    {
                        if (p.Name.EndsWith("Ids"))
                        {
                            relationSets.Add(p, allProperties.Single(x => x.Name == p.Name[..^3]));
                        }
                    }

                    //Add all related objects noted as ids in the API-call as full objects to the database entity
                    foreach (PropertyInfo propertyIds in relationSets.Keys)
                    {
                        int previousObjectCount = relationSets[propertyIds]
                            .GetValue(requestEntity)
                            .Count;
                        foreach (
                            var item in (
                                (IEnumerable<dynamic>)
                                    context
                                        .GetType()
                                        .GetMethod(nameof(context.Set), new Type[0])
                                        .MakeGenericMethod(
                                            relationSets[propertyIds]
                                                .PropertyType.GetGenericArguments()[0]
                                        )
                                        .Invoke(context, null)
                            ).Where(x =>
                                ((ICollection<int>)propertyIds.GetValue(requestEntity)).Contains(
                                    x.Id
                                )
                            )
                        )
                        {
                            object child = relationSets[propertyIds].GetValue(requestEntity);
                            typeof(ICollection<>)
                                .MakeGenericType(
                                    relationSets[propertyIds].PropertyType.GetGenericArguments()[0]
                                )
                                .GetMethod("Add")
                                .Invoke(child, new object[] { item });
                            relationSets[propertyIds].SetValue(requestEntity, child);
                        }

                        if (
                            propertyIds.GetValue(requestEntity).Count
                            != (
                                relationSets[propertyIds].GetValue(requestEntity).Count
                                - previousObjectCount
                            )
                        )
                        {
                            throw new Exception();
                        }
                    }

                    context.Add(requestEntity);

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    transaction.Commit();
                    return requestEntity;
                }
                catch (Exception)
                {
                    // Rollback all changes
                    transaction.Rollback();

                    throw;
                }
            }
        }
    }
}
