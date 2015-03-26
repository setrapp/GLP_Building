using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataCollector : MonoBehaviour {

	[SerializeField]
	public List<DataRecord> dataRecords;

	void Start()
	{
		//Debug.Log(sizeof(RecordMetadata));
		//Debug.Log(System.Runtime.InteropServices.Marshal.SizeOf(typeof(RecordMetadata)));
	}

	/*TODO: When and how does application need to retrieve data?*/

	public void MakeRecord(KeyValuePair[] keyValues)
	{
		string data = "";
		for (int i = 0; i < keyValues.Length; i++)
		{
			data += keyValues[i].key + "=" + keyValues[i].value + "\n";
		}

		RecordMetadata metadata = new RecordMetadata();
		/*TODO: Obtain game metadata information*/

		DataRecord dataRecord = new DataRecord;
		dataRecord.data = data;
		dataRecord.metadata = metadata;
		dataRecords.Add(dataRecord);
		/*TODO: Store this in database*/
	}

	public void MakeRecord(List<KeyValuePair> keyValueList)
	{
		KeyValuePair[] keyValues = new KeyValuePair[keyValueList.Count];
		for (int i = 0; i < keyValues.Length; i++)
		{
			keyValues[i] = keyValueList[i];
		}
		MakeRecord(keyValues);
	}

	public void MakeRecord(KeyValuePair keyValue)
	{
		KeyValuePair[] keyValues = new KeyValuePair[1];
		keyValues[0] = keyValue;
		MakeRecord(keyValues);
	}
}

[System.Serializable]
public struct KeyValuePair
{
	public string key;
	public string value;
}

[System.Serializable]
public struct RecordMetadata
{
	public float deviceSessionId;
	public float currentLevelProficiency;
	public short zone1Progress;
	public short zone2Progress;
	public short zone3Progress;
	public short zone4Progress;
	public short zone5Progress;
	public short zone6Progress;
}

[System.Serializable]
public struct DataRecord
{
	public string data;
	public RecordMetadata metadata;
}
