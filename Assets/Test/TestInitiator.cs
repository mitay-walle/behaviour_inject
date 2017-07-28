﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourInject.Test
{
	public class TestInitiator : MonoBehaviour
	{
		private Context _contextTest;
		private Context _contextBase;
		private PrecomposeDependency _precomposed;

		private void Awake()
		{
			_precomposed = new PrecomposeDependency("test1");

			_contextTest = Context.Create("test")
				.RegisterDependency(_precomposed)
				.RegisterType<AutocomposeDependency>()
				.RegisterTypeAs<DependencyImpl, IDependency>();

			_contextBase = Context.Create("base")
				.SetParentContext("test")
				.RegisterDependency(new PrecomposeDependency("base"))
				.RegisterType<AutocomposeDependency>();
		}


		private IEnumerator Start()
		{
			yield return new WaitForSeconds(0.5f);
			Destroy(gameObject);
		}


		private void OnDestroy()
		{
			_contextTest.Destroy();
			_contextBase.Destroy();
			Assert.True(_precomposed.Disposed, "dispose");
		}
	}

	public interface IDependency {
		void Run();
	}


	public class DependencyImpl : IDependency
	{
		private AutocomposeDependency _dep;

		public DependencyImpl(AutocomposeDependency dep)
		{
			_dep = dep;
		}

		public void Run()
		{
			Assert.NotNull(_dep, "interfaced autocompose");
		}
	}

	public class PrecomposeDependency : IDisposable
	{
		public IEvent RecievedEvt { get; private set; }
		public string Keyword { get; private set; }
		public bool Disposed { get; private set; }

		public PrecomposeDependency(string keyword)
		{
			Keyword = keyword;
		}
		

		[InjectEvent]
		public void Handle(IEvent evt)
		{
			RecievedEvt = evt;
		}

		public void Dispose()
		{
			Disposed = true;
		}
	}

	public class AutocomposeDependency
	{
		private PrecomposeDependency _dep;

		public AutocomposeDependency(PrecomposeDependency dep)
		{
			_dep = dep;
		}

		public void Run()
		{
			Assert.NotNull(_dep, "autocompose");
		}
	}


	public class IEvent
	{ }


	public class TestEvent : IEvent
	{ }
}