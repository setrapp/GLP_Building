using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FixedJointTest : MonoBehaviour {
	public FixedJoint joint;
	[SerializeField]
	public List<ContactPoint> contacts;

	void Awake()
	{
		contacts = new List<ContactPoint>();
	}

	void Update()
	{
		if (contacts.Count > 0)
			Debug.Log(contacts[0].otherCollider.gameObject.name);
		//joint.autoConfigureConnectedAnchor = false;
		//joint.anchor = new Vector3(0, 0, 1);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject != joint.connectedBody.gameObject)
		{
			joint.connectedBody.GetComponent<NavMeshAgent>().enabled = false;
			for (int i = 0; i < collision.contacts.Length; i++)
			{
				contacts.Add(collision.contacts[i]);
			}
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.collider.gameObject != gameObject)
		{
			for (int i = 0; i < collision.contacts.Length; i++)
			{
				for (int j = 0; j < contacts.Count; j++)
				{
					if (contacts[j].otherCollider == collision.contacts[i].otherCollider)
					{
						contacts.RemoveAt(j);
					}
				}
			}
		}
		
		if (contacts.Count < 1)
		{
			joint.connectedBody.GetComponent<NavMeshAgent>().enabled = true;
		}
	}
}
