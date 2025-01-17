﻿using LOP.Entities;
using System.Reflection;
using System.Collections.Generic;

namespace LOP.Core
{
    public class EntityManager : Script
    {
        private static EntityManager instance;
        public static EntityManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindScript<EntityManager>();
                    if(instance == null)
                    {
                        instance = Instantiate<EntityManager>();
                    }
                }
                return instance;
            }
        }
        public List<Entity> entities;
        public Dictionary<string, Type> entityDic;
        public EntityManager()
        {
            entities = new List<Entity>();
            entityDic = new Dictionary<string, Type>();
        }
        public void AddEntity(Entity entity)
        {
            if (!entities.Contains(entity))
            {
                entities.Add(entity);
            }
        }
        public void RemoveEntity(Entity entity)
        {
            if (entities.Contains(entity))
            {
                entities.Remove(entity);
            }
        }
        public override void Start()
        {
            Type entityType = typeof(Entity);
            var types = Assembly.GetCallingAssembly().GetTypes();
            foreach (var type in types)
            {
                var parent = type.BaseType;
                if (parent != null && parent.FullName == entityType.FullName)
                {
                    Console.WriteLine("Init: " + type.FullName + " As " + type.Name);
                    entityDic.Add(type.Name, type);
                }
            }
        }
        public override void Update()
        {
            foreach (var entity in entities)
            {
                entity.Update();
            }
        }
    }
}
