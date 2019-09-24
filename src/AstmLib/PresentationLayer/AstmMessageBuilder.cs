using System.Collections.Generic;
using AstmLib.PresentationLayer.Exceptions;
using AstmLib.Configuration;

namespace AstmLib.PresentationLayer
{
	public static class AstmMessageBuilder
	{
		public static AstmMessage[] Build(string[] data, AstmHighLevelSettings highLevelSettings)
		{
			var messages = new List<AstmMessage>();
			AstmMessage message = null;
			AstmRecord lastLeveledRecord = null;
			var state = 0;
			for (var i = 0; i < data.Length; i++)
			{
				var currentRecord = AstmRecord.Parse(data[i], highLevelSettings);
				var curSN = currentRecord.Fields[1];	// if this one is header than curSN would be ignored
				switch (state)
				{
					case 0:
						// This is initial state and there we can receive only header
						switch (currentRecord.RecordTypeId)
						{
							case AstmRecordTypeIds.Header:
								message = new AstmMessage();
								message.HeaderRecord = (AstmHeaderRecord)currentRecord;
								lastLeveledRecord = currentRecord;
								state = 1;
								break;
							default:
								throw new AstmMessageBuilderException("Expected header record", messages.ToArray());
						}
						break;
					case 1:	//Last leveled record - HEADER
						switch (currentRecord.RecordTypeId)
						{
							case AstmRecordTypeIds.Patient:
								lastLeveledRecord.AddChild(currentRecord);
								state = 4;	// last record PATIENT
								lastLeveledRecord = currentRecord;
								break;
							case AstmRecordTypeIds.Query:
								state = 3;	// last record QUERY
								lastLeveledRecord.AddChild(currentRecord);
								lastLeveledRecord = currentRecord;
								break;
							case AstmRecordTypeIds.Termination:
								message.HeaderRecord.TerminationRecord = (AstmTerminationRecord)currentRecord;
								messages.Add(message);
								state = 0;
								break;
							case AstmRecordTypeIds.Comment:
								lastLeveledRecord.AddChild(currentRecord);
								break;
							case AstmRecordTypeIds.Manufacturer:
								lastLeveledRecord.AddChild(currentRecord);
								break;
							default:
								throw new AstmMessageBuilderException($"Unexpected type of message. Expected: Patient, Query, Terminator, Comment, Manufacturer, but was record with recordTypeId={currentRecord.RecordTypeId}", messages.ToArray());
						}
						break;
					case 3:	// Last leveled record - QUERY
						switch (currentRecord.RecordTypeId)
						{
							case AstmRecordTypeIds.Termination:
								message.HeaderRecord.TerminationRecord = (AstmTerminationRecord)currentRecord;
								messages.Add(message);
								state = 0;
								break;
							case AstmRecordTypeIds.Comment:
							case AstmRecordTypeIds.Manufacturer:
								lastLeveledRecord.AddChild(currentRecord);
								break;
							case AstmRecordTypeIds.Query:
								lastLeveledRecord.Parent.AddChild(currentRecord);
								lastLeveledRecord = currentRecord;
								break;
							default:
								throw new AstmMessageBuilderException($"Unexpected type of message. Expected: Terminator, Comment, Manufacturer, but was record with recordTypeId={currentRecord.RecordTypeId}", messages.ToArray());
						}
						break;
					case 4:	// Last leveled record - PATIENT
						switch (currentRecord.RecordTypeId)
						{
							case AstmRecordTypeIds.Patient:
								// Check this one
								message.HeaderRecord.AddChild(currentRecord);
								lastLeveledRecord = currentRecord;
								break;
							case AstmRecordTypeIds.Order:
								lastLeveledRecord.AddChild(currentRecord);
								lastLeveledRecord = currentRecord;
								state = 5;
								break;
							case AstmRecordTypeIds.Comment:
							case AstmRecordTypeIds.Manufacturer:
								lastLeveledRecord.AddChild(currentRecord);
								break;
							case AstmRecordTypeIds.Termination:
								message.HeaderRecord.TerminationRecord = (AstmTerminationRecord)currentRecord;
								messages.Add(message);
								state = 0;
								break;
							default:
								throw new AstmMessageBuilderException($"Unexpected type of message. Expected: Terminator, Comment, Manufacturer, Patient, Order but was record with recordTypeId={currentRecord.RecordTypeId}", messages.ToArray());
						}
						break;
					case 5:	// Last leveled record ORDER
						switch (currentRecord.RecordTypeId)
						{
							case AstmRecordTypeIds.Order:
								lastLeveledRecord.Parent.AddChild(currentRecord);
								lastLeveledRecord = currentRecord;
								break;
							case AstmRecordTypeIds.Patient:
								lastLeveledRecord.	// last order
									Parent.			// last patient
									Parent.AddChild(currentRecord);		// header
								lastLeveledRecord = currentRecord;
								state = 4;			// last leveled record patient
								break;
							case AstmRecordTypeIds.Result:
								lastLeveledRecord.AddChild(currentRecord);
								lastLeveledRecord = currentRecord;
								state = 6;
								break;
							case AstmRecordTypeIds.Termination:
								message.HeaderRecord.TerminationRecord = (AstmTerminationRecord)currentRecord;
								messages.Add(message);
								state = 0;
								break;
							case AstmRecordTypeIds.Comment:
							case AstmRecordTypeIds.Manufacturer:
								lastLeveledRecord.AddChild(currentRecord);
								break;
							default:
								throw new AstmMessageBuilderException($"Unexpected type of message. Expected: Terminator, Comment, Manufacturer, Patient, Order, Result but was record with recordTypeId={currentRecord.RecordTypeId}", messages.ToArray());
						}
						break;
					case 6:	// Last leveled record = RESULT
						switch (currentRecord.RecordTypeId)
						{
							case AstmRecordTypeIds.Termination:
								message.HeaderRecord.TerminationRecord = (AstmTerminationRecord)currentRecord;
								messages.Add(message);
								state = 0;
								break;
							case AstmRecordTypeIds.Comment:
							case AstmRecordTypeIds.Manufacturer:
								lastLeveledRecord.AddChild(currentRecord);
								break;
							case AstmRecordTypeIds.Result:
								lastLeveledRecord.Parent.AddChild(currentRecord);
								lastLeveledRecord = currentRecord;
								break;
							case AstmRecordTypeIds.Order:
								lastLeveledRecord.	// result
									Parent.			// Order
									Parent.AddChild(currentRecord);			// Patient
								state = 5;
								lastLeveledRecord = currentRecord;
								break;
							case AstmRecordTypeIds.Patient:
								message.HeaderRecord.AddChild(currentRecord);
								lastLeveledRecord = currentRecord;
								state = 4;
								break;
							default:
								throw new AstmMessageBuilderException($"Unexpected type of message. Expected: Terminator, Comment, Manufacturer, Patient, Order, Result but was record with recordTypeId={currentRecord.RecordTypeId}", messages.ToArray());
						}
						break;
				}

				// Check Sequence number. If it was been changed (may be in addChild method) than this is sign that received SN is wrong!!!
				if (highLevelSettings.CheckSN
					&& !(currentRecord is AstmHeaderRecord)
					&& currentRecord.Fields[1] != curSN)
					throw new InvalidSNAstmMessageBuilderException(i, data[i], messages.ToArray());
			}
			return messages.ToArray();
		}
	}
}
