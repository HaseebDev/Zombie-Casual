using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class TestScript : MonoBehaviour
{
	public void OnSerialize()
	{
		if (this.someGameObject != null && this.someGameObject.GetComponent<ObjectIdentifier>())
		{
			this.someGameObject_id = this.someGameObject.GetComponent<ObjectIdentifier>().id;
		}
		else
		{
			this.someGameObject_id = null;
		}
		if (this.testClassArray != null)
		{
			foreach (TestClass testClass in this.testClassArray)
			{
				if (testClass.go != null && testClass.go.GetComponent<ObjectIdentifier>())
				{
					testClass.go_id = testClass.go.GetComponent<ObjectIdentifier>().id;
				}
				else
				{
					testClass.go_id = null;
				}
			}
		}
	}

	public void OnDeserialize()
	{
		ObjectIdentifier[] array = UnityEngine.Object.FindObjectsOfType(typeof(ObjectIdentifier)) as ObjectIdentifier[];
		if (!string.IsNullOrEmpty(this.someGameObject_id))
		{
			foreach (ObjectIdentifier objectIdentifier in array)
			{
				if (!string.IsNullOrEmpty(objectIdentifier.id) && objectIdentifier.id == this.someGameObject_id)
				{
					this.someGameObject = objectIdentifier.gameObject;
					break;
				}
			}
		}
		if (this.testClassArray != null)
		{
			foreach (TestClass testClass in this.testClassArray)
			{
				if (!string.IsNullOrEmpty(testClass.go_id))
				{
					foreach (ObjectIdentifier objectIdentifier2 in array)
					{
						if (!string.IsNullOrEmpty(objectIdentifier2.id) && objectIdentifier2.id == testClass.go_id)
						{
							testClass.go = objectIdentifier2.gameObject;
							break;
						}
					}
				}
			}
		}
	}

	public string testString = "Hello";

	public GameObject someGameObject;

	public string someGameObject_id;

	public TestClass testClass = new TestClass();

	public TestClass[] testClassArray = new TestClass[2];

	[DontSaveField]
	public Transform TransformThatWontBeSaved;
}
