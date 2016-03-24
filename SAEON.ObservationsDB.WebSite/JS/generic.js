function makenewJsonForExport(object1)
{
	var myJSON = "{";
	for (var i = 0; i < object1.length; i++)
	{
		myJSON += "\"" + object1[i].header + "\":\"" + object1[i].dataIndex + "\"";
		if (i != (object1.length - 1))
		{
			myJSON += ",";
		}
	}
	myJSON += "}";

	return myJSON;

}