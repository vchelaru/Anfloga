using Anfloga.Entities;
using System;
using FlatRedBall.Math;
using FlatRedBall.Graphics;
using Anfloga.Performance;

namespace Anfloga.Factories
{
	public class WorldObjectEntityFactory : IEntityFactory
	{
		public static WorldObjectEntity CreateNew ()
		{
			return CreateNew(null);
		}
		public static WorldObjectEntity CreateNew (Layer layer)
		{
			if (string.IsNullOrEmpty(mContentManagerName))
			{
				throw new System.Exception("You must first initialize the factory to use it. You can either add PositionedObjectList of type WorldObjectEntity (the most common solution) or call Initialize in custom code");
			}
			WorldObjectEntity instance = null;
			instance = new WorldObjectEntity(mContentManagerName, false);
			instance.AddToManagers(layer);
			if (mScreenListReference != null)
			{
				mScreenListReference.Add(instance);
			}
			if (EntitySpawned != null)
			{
				EntitySpawned(instance);
			}
			return instance;
		}
		
		public static void Initialize (FlatRedBall.Math.PositionedObjectList<WorldObjectEntity> listFromScreen, string contentManager)
		{
			mContentManagerName = contentManager;
			mScreenListReference = listFromScreen;
		}
		
		public static void Destroy ()
		{
			mContentManagerName = null;
			mScreenListReference = null;
			mPool.Clear();
			EntitySpawned = null;
		}
		
		private static void FactoryInitialize ()
		{
			const int numberToPreAllocate = 20;
			for (int i = 0; i < numberToPreAllocate; i++)
			{
				WorldObjectEntity instance = new WorldObjectEntity(mContentManagerName, false);
				mPool.AddToPool(instance);
			}
		}
		
		/// <summary>
		/// Makes the argument objectToMakeUnused marked as unused.  This method is generated to be used
		/// by generated code.  Use Destroy instead when writing custom code so that your code will behave
		/// the same whether your Entity is pooled or not.
		/// </summary>
		public static void MakeUnused (WorldObjectEntity objectToMakeUnused)
		{
			MakeUnused(objectToMakeUnused, true);
		}
		
		/// <summary>
		/// Makes the argument objectToMakeUnused marked as unused.  This method is generated to be used
		/// by generated code.  Use Destroy instead when writing custom code so that your code will behave
		/// the same whether your Entity is pooled or not.
		/// </summary>
		public static void MakeUnused (WorldObjectEntity objectToMakeUnused, bool callDestroy)
		{
			if (callDestroy)
			{
				objectToMakeUnused.Destroy();
			}
		}
		
		
			static string mContentManagerName;
			static PositionedObjectList<WorldObjectEntity> mScreenListReference;
			static PoolList<WorldObjectEntity> mPool = new PoolList<WorldObjectEntity>();
			public static Action<WorldObjectEntity> EntitySpawned;
			object IEntityFactory.CreateNew ()
			{
				return WorldObjectEntityFactory.CreateNew();
			}
			object IEntityFactory.CreateNew (Layer layer)
			{
				return WorldObjectEntityFactory.CreateNew(layer);
			}
			public static FlatRedBall.Math.PositionedObjectList<WorldObjectEntity> ScreenListReference
			{
				get
				{
					return mScreenListReference;
				}
				set
				{
					mScreenListReference = value;
				}
			}
			static WorldObjectEntityFactory mSelf;
			public static WorldObjectEntityFactory Self
			{
				get
				{
					if (mSelf == null)
					{
						mSelf = new WorldObjectEntityFactory();
					}
					return mSelf;
				}
			}
	}
}
