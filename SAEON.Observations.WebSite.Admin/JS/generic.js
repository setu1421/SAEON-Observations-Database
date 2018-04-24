function makenewJsonForExport(object1)
{
    var myJSON = "{";
    var first = true;
	for (var i = 0; i < object1.length; i++)
    {
        if (object1[i].header !== "") {
            if (first) {
                myJSON += "\"" + object1[i].header + "\":\"" + object1[i].dataIndex + "\"";
                first = false;
            }
            else {
                myJSON += ",\"" + object1[i].header + "\":\"" + object1[i].dataIndex + "\"";

            }
        }
	}
	myJSON += "}";

	return myJSON;

}