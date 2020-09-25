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

function GetInvalidFields(formPanel) {
    var s = 'Invalid: ';
    var form = formPanel.getForm();
    var fields = form.items;
    var i = 0;
    for (i = 0; i < fields.length; i += 1) {
        var field = fields.items[i];
        var input = form.findField(field.id);
        if (input) {
            input.validate();
            if (!input.isValid()) {
                s = s + " " + input.id;
            }
        }
    }
    if (s === 'Invalid: ') {
        s = 'Valid';
    }
    alert(s);
}
